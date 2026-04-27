using GoodHamburguer.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburguer.Data;

public class AppDbContext : DbContext
{
    public DbSet<Pedido> Pedidos { get; set; } = null!;
    public DbSet<Sanduiche> Sanduiches { get; set; } = null!;
    public DbSet<Acompanhamento> Acompanhamentos { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relação Um-para-Muitos: Sanduiche -> Pedido
        // Impede a exclusão de um Sanduiche se ele estiver referenciado em qualquer Pedido.
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Sanduiche)
            .WithMany() // Um Sanduiche pode estar em muitos Pedidos
            .OnDelete(DeleteBehavior.Restrict);


        // Relação Muitos-para-Muitos: Pedido -> Acompanhamento
        // Impede a exclusão de um Acompanhamento se ele estiver referenciado em qualquer Pedido
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Acompanhamentos)
            .WithMany() // Um Acompanhamento pode estar em muitos Pedidos
            .UsingEntity(
                r => r.HasOne(typeof(Acompanhamento)).WithMany().OnDelete(DeleteBehavior.Restrict),
                l => l.HasOne(typeof(Pedido)).WithMany().OnDelete(DeleteBehavior.Cascade)
            );

        // Dados iniciais (Seeding) para Sanduíches
        modelBuilder.Entity<Sanduiche>().HasData(
            new Sanduiche { Id = 1, Nome = "X Burguer", Preco = 5.00m },
            new Sanduiche { Id = 2, Nome = "X Egg", Preco = 4.50m },
            new Sanduiche { Id = 3, Nome = "X Bacon", Preco = 7.00m }
        );

        // Dados iniciais (Seeding) para Acompanhamentos
        modelBuilder.Entity<Acompanhamento>().HasData(
            new Acompanhamento { Id = 1, Nome = "Batata Frita", Preco = 2.00m },
            new Acompanhamento { Id = 2, Nome = "Refrigerante", Preco = 2.50m }
        );
    }
}