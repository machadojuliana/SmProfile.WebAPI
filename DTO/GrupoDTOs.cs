using System.ComponentModel.DataAnnotations;

namespace SMProfile.WebAPI.DTOs;

public record GrupoCreateUpdateDTO(
    [Required(ErrorMessage = "O nome do grupo é obrigatório.")]
    [MaxLength(50)]
    string NomeGrupo,

    [Required(ErrorMessage = "O tipo do grupo é obrigatório.")]
    [MaxLength(20)]
    string TipoGrupo,

    [Required(ErrorMessage = "Os membros são obrigatórios.")]
    string Membros,

    [Required(ErrorMessage = "A data de debut é obrigatória.")]
    DateOnly DataDebut,

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    string Descricao
);

public record GrupoResponseDTO(
    int IdGrupo,
    string NomeGrupo,
    string TipoGrupo,
    string Membros,
    DateOnly DataDebut,
    string Descricao
);