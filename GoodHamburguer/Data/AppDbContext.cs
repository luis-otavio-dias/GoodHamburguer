using Microsoft.EntityFrameworkCore;
using GoodHamburguer.Models;

namespace GoodHamburguer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Sanduiche> Sanduiches { get; set; } = null!;
    public DbSet<Acompanhamento> Acompanhamentos { get; set; } = null!;
    public DbSet<Pedido> Pedidos { get; set; } = null!;
}