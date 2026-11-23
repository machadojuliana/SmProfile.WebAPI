using Microsoft.EntityFrameworkCore;
using SMProfile.WebAPI.Context;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Repositories;

public class GrupoRepository : IGrupoRepository
{
    private readonly SMProfileDbContext _context;
    public GrupoRepository(SMProfileDbContext context) => _context = context;

    public async Task<List<Grupo>> ListarTodos() =>
        await _context.Grupos.OrderBy(g => g.NomeGrupo).ToListAsync();

    public async Task<Grupo?> BuscarPorId(int id) =>
        await _context.Grupos.FirstOrDefaultAsync(g => g.IdGrupo == id);

    public async Task<bool> ExisteNome(string nomeGrupo, int? ignorarId = null) =>
        await _context.Grupos.AnyAsync(g =>
            g.NomeGrupo.ToLower() == nomeGrupo.ToLower() &&
            (!ignorarId.HasValue || g.IdGrupo != ignorarId.Value));

    public async Task<DateOnly?> ObterDataPrimeiroComeback(int idGrupo) =>
        await _context.AlbunsFisicos
            .Where(af => af.IdGrupo == idGrupo && af.Comeback != null)
            .Select(af => (DateOnly?)af.Comeback!.DataLancamento)
            .MinAsync();

    public async Task Cadastrar(Grupo grupo)
    {
        _context.Grupos.Add(grupo);
        await _context.SaveChangesAsync();
    }

    public async Task Atualizar(Grupo grupo)
    {
        _context.Grupos.Update(grupo);
        await _context.SaveChangesAsync();
    }

    public async Task Deletar(Grupo grupo)
    {
        _context.Grupos.Remove(grupo);
        await _context.SaveChangesAsync();
    }
}