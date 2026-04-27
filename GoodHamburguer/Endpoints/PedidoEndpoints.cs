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
                .Select(pedido => MapToResponseDTO(pedido))
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
                .Select(pedido => MapToResponseDTO(pedido))
                .FirstOrDefaultAsync();

                return pedido is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(pedido);
        })
        .WithName("GetPedidoById");

        group.MapPut("/{id}", async Task<IResult> (int id, PedidoCreateRequestDTO dto, AppDbContext db) =>
        {
            var pedidoToUpdate = await db.Pedidos
                .Include(p => p.Acompanhamentos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedidoToUpdate is null)
            {
                return Results.NotFound();
            }

            var error = await PopulatePedidoFromDTO(pedidoToUpdate, dto, db);
            if (error != null)
            {
                return error;
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdatePedido");

        group.MapPost("/", async Task<IResult> (PedidoCreateRequestDTO dto, AppDbContext db) =>
        {
            var pedido = new Pedido();
            var error = await PopulatePedidoFromDTO(pedido, dto, db);
            if (error != null)
            {
                return error;
            }

            db.Pedidos.Add(pedido);
            await db.SaveChangesAsync();

            var responseDto = MapToResponseDTO(pedido);
            return Results.Created($"/api/Pedido/{pedido.Id}", responseDto);
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

    private static PedidoResponseDTO MapToResponseDTO(Pedido pedido)
    {
        decimal subtotal = pedido.Sanduiche.Preco + pedido.Acompanhamentos.Sum(a => a.Preco);
        decimal total = pedido.Total;
        decimal desconto = (total - subtotal) * -1;


        return new PedidoResponseDTO(
            Id: pedido.Id,
            Sanduiche: new SanduicheResponseDTO(pedido.Sanduiche.Id, pedido.Sanduiche.Nome, pedido.Sanduiche.Preco),
            Acompanhamentos: pedido.Acompanhamentos
                .Select(a => new AcompanhamentoResponseDTO(a.Id, a.Nome, a.Preco))
                .ToList(),
            Subtotal: subtotal,
            Desconto: desconto,
            Total: pedido.Total
        );
    }


    private static async Task<IResult?> PopulatePedidoFromDTO(Pedido pedido, PedidoCreateRequestDTO dto, AppDbContext db)
    {
        var sanduiche = await db.Sanduiches.FindAsync(dto.SanduicheId);
        if (sanduiche == null)
        {
            return Results.NotFound($"Sanduíche com ID {dto.SanduicheId} não encontrado.");
        }

        pedido.SanduicheId = dto.SanduicheId;
        pedido.Sanduiche = sanduiche;

        pedido.Acompanhamentos.Clear();

        var errorAcompanhamentos = await ProcessAcompanhamentosAsync(pedido, dto.AcompanhamentoIds, db);
        if (errorAcompanhamentos != null)
        {
            return errorAcompanhamentos;
        }

        pedido.AtualizarTotal();
        return null;
    }

    private static async Task<IResult?> ProcessAcompanhamentosAsync(Pedido pedido, List<int> acompanhamentoIds, AppDbContext db)
    {
        var acompanhamentos = await db.Acompanhamentos
            .Where(a => acompanhamentoIds.Contains(a.Id))
            .ToListAsync();

        var idsEncontrados = acompanhamentos.Select(a => a.Id).ToList();

        var idsInvalidos = acompanhamentoIds
            .Where(id => !idsEncontrados.Contains(id))
            .Distinct()
            .ToList();

        if (idsInvalidos.Count > 0)
        {
            return Results.NotFound($"Acompanhamentos com IDs {string.Join(", ", idsInvalidos)} não encontrados.");
        }
        
        try
        {
            foreach (var acompanhamentoId in acompanhamentoIds)
            {
                var acompanhamento = acompanhamentos.First(a => a.Id == acompanhamentoId);
                pedido.AdicionarAcompanhamento(acompanhamento);
            }
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }

        return null;
    }
}
