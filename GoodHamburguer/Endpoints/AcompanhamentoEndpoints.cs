using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Data;
using GoodHamburguer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace GoodHamburguer.Endpoints;


public static class AcompanhamentoEndpoints
{
    public static void MapAcompanhamentoEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Acompanhamento");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Acompanhamentos.ToListAsync();
        })
        .WithName("GetAllAcompanhamentos");

        group.MapGet("/{id}", async Task<Results<Ok<Acompanhamento>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Acompanhamentos.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Acompanhamento model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetAcompanhamentoById");
    }
}