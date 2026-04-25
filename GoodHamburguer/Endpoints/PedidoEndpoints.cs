using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Data;
using GoodHamburguer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace GoodHamburguer.Endpoints;

public static class PedidoEndpoints
{
    public static void MapPedidoEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Pedido");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Pedidos.ToListAsync();
        })
        .WithName("GetAllPedidos");

        group.MapGet("/{id}", async Task<Results<Ok<Pedido>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Pedidos.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Pedido model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
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

        group.MapPost("/", async (Pedido pedido, AppDbContext db) =>
        {
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
