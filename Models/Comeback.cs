using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMProfile.WebAPI.Models;

[Table("Comeback")]
public class Comeback
{
    [Key]
    public int IdComeback { get; set; }

    [Required, MaxLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string TitleTrack { get; set; } = string.Empty;

    [Required]
    public DateOnly DataLancamento { get; set; }

    [Required]
    public string MusicVideo { get; set; } = string.Empty;

    [Required]
    public string Capa { get; set; } = string.Empty;

    public ICollection<AlbumFisico> AlbunsFisicos { get; set; } = new List<AlbumFisico>();
}