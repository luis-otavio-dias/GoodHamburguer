using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Data;
using GoodHamburguer.Models;
using Microsoft.AspNetCore.Http.HttpResults;
namespace GoodHamburguer.Endpoints;

public static class SanduicheEndpoints
{
    public static void MapSanduicheEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Sanduiche");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Sanduiches.ToListAsync();
        })
        .WithName("GetAllSanduiches");

        group.MapGet("/{id}", async Task<Results<Ok<Sanduiche>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Sanduiches.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Sanduiche model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSanduicheById");
    }
}