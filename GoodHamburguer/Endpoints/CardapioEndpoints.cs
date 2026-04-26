using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Data;
using GoodHamburguer.Models;
using GoodHamburguer.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
namespace GoodHamburguer.Endpoints;


public static class CardapioEndpoints
{
    public static void MapCardapioEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Cardapio");

        group.MapGet("/", async (AppDbContext db) =>
        {   
            var sanduiches = await db.Sanduiches
                .Select(s => new SanduicheResponseDTO(s.Id, s.Nome, s.Preco))
                .ToListAsync();
            
            var acompanhamentos = await db.Acompanhamentos
                .Select(a => new AcompanhamentoResponseDTO(a.Id,a.Nome, a.Preco))
                .ToListAsync();
            
            var cardapio = new CardapioResponseDTO(sanduiches, acompanhamentos);

            return TypedResults.Ok(cardapio);
        })
        .WithName("GetCardapio");
    }
}