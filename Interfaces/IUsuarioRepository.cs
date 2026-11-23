using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmail(string email);
    Task<Usuario?> BuscarPorId(int id);
}