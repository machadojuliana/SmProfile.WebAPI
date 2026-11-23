using System.ComponentModel.DataAnnotations;

namespace SMProfile.WebAPI.DTOs;

public record LoginDTO(
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    string Senha
);

public record TokenResponseDTO(string Token, DateTime ExpiraEm, string Nome, string Email);