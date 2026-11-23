using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Interfaces;

public interface IComebackRepository
{
    Task<List<Comeback>> ListarComFiltros(int? idGrupo, string? titulo, DateOnly? dataLancamento);
    Task<Comeback?> BuscarPorId(int id);
    Task Cadastrar(Comeback comeback);
    Task Atualizar(Comeback comeback);
    Task Deletar(Comeback comeback);
}