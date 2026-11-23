using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Interfaces;

public interface IGrupoRepository
{
    Task<List<Grupo>> ListarTodos();
    Task<Grupo?> BuscarPorId(int id);
    Task<bool> ExisteNome(string nomeGrupo, int? ignorarId = null);
    Task<DateOnly?> ObterDataPrimeiroComeback(int idGrupo);
    Task Cadastrar(Grupo grupo);
    Task Atualizar(Grupo grupo);
    Task Deletar(Grupo grupo);
}