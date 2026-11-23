using System.ComponentModel.DataAnnotations;

namespace SMProfile.WebAPI.DTOs;

public record AlbumFisicoInicialDTO(
    [Required] int IdGrupo,
    [Required, MaxLength(100)] string Versao
);

public record AlbumFisicoCreateDTO(
    [Required] int IdComeback,
    [Required] int IdGrupo,
    [Required(ErrorMessage = "A versão do álbum é obrigatória."), MaxLength(100)]
    string Versao
);

public record AlbumFisicoUpdateDTO(
    [Required] int IdGrupo,
    [Required(ErrorMessage = "A versão do álbum é obrigatória."), MaxLength(100)]
    string Versao
);

public record AlbumFisicoResponseDTO(
    int IdAlbumFisico,
    int IdComeback,
    string TituloComeback,
    int IdGrupo,
    string NomeGrupo,
    string Versao
);