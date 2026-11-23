using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMProfile.WebAPI.Models;

[Table("AlbumFisico")]
public class AlbumFisico
{
    [Key]
    public int IdAlbumFisico { get; set; }

    [Required]
    public int IdComeback { get; set; }

    [Required]
    public int IdGrupo { get; set; }

    [Required, MaxLength(100)]
    public string Versao { get; set; } = string.Empty;

    [ForeignKey(nameof(IdComeback))]
    public Comeback? Comeback { get; set; }

    [ForeignKey(nameof(IdGrupo))]
    public Grupo? Grupo { get; set; }
}