using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMProfile.WebAPI.Models;

[Table("Usuario")]
public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required, MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Senha { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}