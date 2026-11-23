using System.ComponentModel.DataAnnotations;

namespace SMProfile.WebAPI.DTOs;

public class ComebackCreateFormDTO
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A title track é obrigatória.")]
    public string TitleTrack { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
    public DateOnly DataLancamento { get; set; }

    [Required(ErrorMessage = "O link do Music Video é obrigatório.")]
    public string MusicVideo { get; set; } = string.Empty;

    public IFormFile? CapaArquivo { get; set; }
    public string? CapaUrl { get; set; }

    [Required(ErrorMessage = "Informe ao menos um álbum físico e grupo (JSON).")]
    public string AlbunsFisicosJson { get; set; } = string.Empty;
}

public class ComebackUpdateFormDTO
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A title track é obrigatória.")]
    public string TitleTrack { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
    public DateOnly DataLancamento { get; set; }

    [Required(ErrorMessage = "O link do Music Video é obrigatório.")]
    public string MusicVideo { get; set; } = string.Empty;

    public IFormFile? CapaArquivo { get; set; }
    public string? CapaUrl { get; set; }
}

public record ComebackResponseDTO(
    int IdComeback,
    string Titulo,
    string TitleTrack,
    DateOnly DataLancamento,
    string MusicVideo,
    string Capa,
    List<AlbumFisicoResponseDTO> AlbunsFisicos
);