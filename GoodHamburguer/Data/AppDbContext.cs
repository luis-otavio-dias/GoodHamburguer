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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sanduiche>(s => 
        {
            s.HasKey(x => x.Id);
            s.Property(x => x.Nome).IsRequired().HasMaxLength(10);
            s.Property(x => x.Preco).IsRequired();

            s.HasData(
                new Sanduiche { Id = 1, Nome = "X Burguer", Preco = 5.00m },
                new Sanduiche { Id = 2, Nome = "X Egg", Preco = 4.50m },
                new Sanduiche { Id = 3, Nome = "X Bacon", Preco = 7.00m }
            );
        });

        modelBuilder.Entity<Acompanhamento>(a =>
        {
            a.HasKey(x => x.Id);
            a.Property(x => x.Nome).IsRequired().HasMaxLength(20);
            a.Property(x => x.Preco).IsRequired();

            a.HasData(
                new Acompanhamento { Id = 1, Nome = "Batata Frita", Preco = 2.00m },
                new Acompanhamento { Id = 2, Nome = "Refrigerante", Preco = 2.50m }
            );
        });
    }
}