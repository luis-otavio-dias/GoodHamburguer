using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GoodHamburguer.Models;

[Table("Sanduiches")]
public class Sanduiche
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(10, ErrorMessage = "O nome do sanduíche deve conter no máximo 10 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public decimal Preco { get; set; }


}