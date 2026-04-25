using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GoodHamburguer.Models;

[Table("Acompanhamentos")]
public class Acompanhamento
{
    [Key]
    public int Id {get; set; }

    [Required]
    [MaxLength(20, ErrorMessage = "O nome do acompanhamento deve conter no máximo 20 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public decimal Preco { get; set; }
}