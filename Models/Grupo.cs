using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMProfile.WebAPI.Models;

[Table("Grupo")]
public class Grupo
{
    [Key]
    public int IdGrupo { get; set; }

    [Required, MaxLength(50)]
    public string NomeGrupo { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string TipoGrupo { get; set; } = string.Empty;

    [Required]
    public string Membros { get; set; } = string.Empty;

    [Required]
    public DateOnly DataDebut { get; set; }

    [Required]
    public string Descricao { get; set; } = string.Empty;

    public bool AtivoGrupo { get; set; } = true;

    public ICollection<AlbumFisico> AlbunsFisicos { get; set; } = new List<AlbumFisico>();
}