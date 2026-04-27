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
                .Include(pedido => pedido.Acompanhamentos)
                .Select(pedido => new PedidoResponseDTO(
                    pedido.Id,
                    new SanduicheResponseDTO(pedido.Sanduiche.Id, pedido.Sanduiche.Nome, pedido.Sanduiche.Preco),
                    pedido.Acompanhamentos.Select(a => new AcompanhamentoResponseDTO(a.Id, a.Nome, a.Preco)).ToList(),
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
                .Include(pedido => pedido.Acompanhamentos)
                .Where(pedido => pedido.Id == id)
                .Select(pedido => new PedidoResponseDTO(
                    pedido.Id,
                    new SanduicheResponseDTO(pedido.Sanduiche.Id, pedido.Sanduiche.Nome, pedido.Sanduiche.Preco),
                    pedido.Acompanhamentos.Select(a => new AcompanhamentoResponseDTO(a.Id, a.Nome, a.Preco)).ToList(),
                    pedido.Total
                ))
                .FirstOrDefaultAsync();

                return pedido is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(pedido);
        })
        .WithName("GetPedidoById");

        group.MapPut("/{id}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (int id, PedidoCreateRequestDTO dto, AppDbContext db) =>
        {
            var pedidoToUpdate = await db.Pedidos
                .Include(p => p.Acompanhamentos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedidoToUpdate is null)
            {
                return TypedResults.NotFound();
            }

            var sanduiche = await db.Sanduiches.FindAsync(dto.SanduicheId);
            if (sanduiche == null)
            {
                return TypedResults.BadRequest($"Sanduíche com ID {dto.SanduicheId} não encontrado.");
            }
            pedidoToUpdate.SanduicheId = sanduiche.Id;
            pedidoToUpdate.Sanduiche = sanduiche;

            pedidoToUpdate.Acompanhamentos.Clear();
            try
            {
                foreach (var acompanhamentoId in dto.AcompanhamentoIds)
                {
                    var acompanhamento = await db.Acompanhamentos.FindAsync(acompanhamentoId);
                    if (acompanhamento == null)
                    {
                        return TypedResults.BadRequest($"Acompanhamento com ID {acompanhamentoId} não encontrado.");
                    }
                    pedidoToUpdate.AdicionarAcompanhamento(acompanhamento);
                }
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }

            pedidoToUpdate.AtualizarTotal();
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .WithName("UpdatePedido");

        group.MapPost("/", async (PedidoCreateRequestDTO dto, AppDbContext db) =>
        {
            var sanduiche = await db.Sanduiches.FindAsync(dto.SanduicheId);
            if (sanduiche == null)
            {
                return Results.NotFound($"Sanduíche com ID {dto.SanduicheId} não encontrado.");
            }

            var pedido = new Pedido
            {
                SanduicheId = dto.SanduicheId,
                Sanduiche = sanduiche
            };

            try
            {
                foreach (var acompanhamentoId in dto.AcompanhamentoIds)
                {
                    var acompanhamento = await db.Acompanhamentos.FindAsync(acompanhamentoId);
                    if (acompanhamento == null)
                    {
                        return Results.NotFound($"Acompanhamento com ID {acompanhamentoId} não encontrado.");
                    }
                    
                    pedido.AdicionarAcompanhamento(
                        acompanhamento
                    );
                }
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }

            pedido.AtualizarTotal();

            db.Pedidos.Add(pedido);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Pedido/{pedido.Id}",pedido);
        })
        .WithName("CreatePedido");

        group.MapDelete("/{id}", async Task<Results<NoContent, NotFound>> (int id, AppDbContext db) =>
        {
            var pedido = await db.Pedidos
                .Include(p => p.Acompanhamentos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido is null)
            {
                return TypedResults.NotFound();
            }

            db.Pedidos.Remove(pedido);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("DeletePedido");
    }
}
