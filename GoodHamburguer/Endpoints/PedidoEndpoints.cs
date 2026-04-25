using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Data;
using GoodHamburguer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using GoodHamburguer.DTOs;

namespace GoodHamburguer.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Pedido");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var pedidos = await db.Pedidos
                .Include(pedido => pedido.Sanduiche)
                .Include(pedido => pedido.Acompanhamento)
                .Select(pedido => new PedidoResponseDTO(
                    pedido.Id,
                    new SanduicheResponseDTO(pedido.Sanduiche.Nome, pedido.Sanduiche.Preco),
                    pedido.Acompanhamento.Select(a => new AcompanhamentoResponseDTO(a.Nome, a.Preco)).ToList(),
                    pedido.Total
                ))
                .ToListAsync();
            return pedidos;
        })
        .WithName("GetAllPedidos");

        group.MapGet("/{id}", async Task<Results<Ok<PedidoResponseDTO>, NotFound>> (int id, AppDbContext db) =>
        {
            var pedido = await db.Pedidos
                .Include(pedido => pedido.Sanduiche)
                .Include(pedido => pedido.Acompanhamento)
                .Where(pedido => pedido.Id == id)
                .Select(pedido => new PedidoResponseDTO(
                    pedido.Id,
                    new SanduicheResponseDTO(pedido.Sanduiche.Nome, pedido.Sanduiche.Preco),
                    pedido.Acompanhamento.Select(a => new AcompanhamentoResponseDTO(a.Nome, a.Preco)).ToList(),
                    pedido.Total
                ))
                .FirstOrDefaultAsync();

                return pedido is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(pedido);
        })
        .WithName("GetPedidoById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Pedido pedido, AppDbContext db) =>
        {
            var affected = await db.Pedidos
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, pedido.Id)
                    .SetProperty(m => m.SanduicheId, pedido.SanduicheId)
                    .SetProperty(m => m.Total, pedido.Total)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdatePedido");

        group.MapPost("/", async (PedidoCreateRequestDTO dto, AppDbContext db) =>
        {
            var sanduiche = await db.Sanduiches.FindAsync(dto.SanduicheId);
            if (sanduiche == null)
            {
                return Results.NotFound($"Sanduíche com ID {dto.SanduicheId} não encontrado.");
            }

            var acompanhamentos = new List<Acompanhamento>();
            foreach (var acompanhamentoId in dto.AcompanhamentoIds)
            {
                var acompanhamento = await db.Acompanhamentos.FindAsync(acompanhamentoId);
                if (acompanhamento == null)
                {
                    return Results.NotFound($"Acompanhamento com ID {acompanhamentoId} não encontrado.");
                } else
                {
                    acompanhamentos.Add(acompanhamento);
                }
            }

            var pedido = new Pedido
            {
                SanduicheId = dto.SanduicheId,
                AcompanhamentoIds = dto.AcompanhamentoIds,
                Sanduiche = sanduiche,
                Acompanhamento = acompanhamentos
            };

            pedido.AtualizarTotal();

            db.Pedidos.Add(pedido);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Pedido/{pedido.Id}",pedido);
        })
        .WithName("CreatePedido");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Pedidos
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeletePedido");
    }
}
