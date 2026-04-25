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
        return Sanduiche.Preco + Acompanhamento.Sum(a => a.Preco); // Exemplo de cálculo, considerando os preços dos itens
    }

    public void AtualizarTotal()
    {
        Total = CalcularTotal();
    }


   
}