using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace GoodHamburguer.Models;

[Table("Pedidos")]
public class Pedido
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SanduicheId { get; set; }

    [NotMapped]
    public List<int> AcompanhamentoIds { get; set; } = [];

    public decimal Total { get; set; }

    [ForeignKey("SanduicheId")]
    public Sanduiche Sanduiche { get; set; } = null!;

    public ICollection<Acompanhamento> Acompanhamento { get; set; } = [];

    public decimal CalcularTotal()
    {
        decimal total = Sanduiche.Preco;

        var refrigerante = Acompanhamento.FirstOrDefault(a => a.Nome == "Refrigerante");
        var batata = Acompanhamento.FirstOrDefault(a => a.Nome == "Batata Frita");

        if (Acompanhamento.Count == 2 && batata != null && refrigerante != null)
        {   
            total += batata.Preco + refrigerante.Preco; // Preço total sem desconto
            total -= total * 0.2m; // 2 acompanhamentos, desconto de 20% no total
        }
        else if (Acompanhamento.Count == 1 && refrigerante != null)
        {
            total += refrigerante.Preco; // 1 acompanhamento, preço do sanduíche + preço do refrigerante
            total -= total * 0.15m; // 1 acompanhamento, desconto de 15% no total
        }
        else if (Acompanhamento.Count == 1 && batata != null)
        {
            total += batata.Preco; // 1 acompanhamento, preço do sanduíche + preço da batata
            total -= total * 0.1m; // 1 acompanhamento, desconto de 10% no total
        }
        
        return total;
    }

    public void AtualizarTotal()
    {   
        Total = CalcularTotal();
    }

    public void AdicionarAcompanhamento(Acompanhamento acompanhamento)
    {
        if (Acompanhamento.Count >= 2)
        {
            throw new InvalidOperationException("Não é possível adicionar mais de 2 acompanhamentos.");
        }

        if (Acompanhamento.Any(a => a.Id == acompanhamento.Id))
        {
            throw new InvalidOperationException($"O acompanhamento '{acompanhamento.Nome}' já foi adicionado ao pedido.");
        }

        Acompanhamento.Add(acompanhamento);
        AcompanhamentoIds.Add(acompanhamento.Id);
    }

   
}