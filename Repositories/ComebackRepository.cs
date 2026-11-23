using Microsoft.EntityFrameworkCore;
using SMProfile.WebAPI.Context;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Repositories;

public class ComebackRepository : IComebackRepository
{
    private readonly SMProfileDbContext _context;
    public ComebackRepository(SMProfileDbContext context) => _context = context;

    public async Task<List<Comeback>> ListarComFiltros(int? idGrupo, string? titulo, DateOnly? dataLancamento)
    {
        var query = _context.Comebacks
            .Include(c => c.AlbunsFisicos)
                .ThenInclude(af => af.Grupo)
            .AsQueryable();

        if (idGrupo.HasValue)
            query = query.Where(c => c.AlbunsFisicos.Any(af => af.IdGrupo == idGrupo.Value));

        if (!string.IsNullOrWhiteSpace(titulo))
            query = query.Where(c => c.Titulo.Contains(titulo));

        if (dataLancamento.HasValue)
            query = query.Where(c => c.DataLancamento == dataLancamento.Value);

        return await query.OrderByDescending(c => c.DataLancamento).ToListAsync();
    }

    public async Task<Comeback?> BuscarPorId(int id) =>
        await _context.Comebacks
            .Include(c => c.AlbunsFisicos)
                .ThenInclude(af => af.Grupo)
            .FirstOrDefaultAsync(c => c.IdComeback == id);

    public async Task Cadastrar(Comeback comeback)
    {
        _context.Comebacks.Add(comeback);
        await _context.SaveChangesAsync();
    }

    public async Task Atualizar(Comeback comeback)
    {
        _context.Comebacks.Update(comeback);
        await _context.SaveChangesAsync();
    }

    public async Task Deletar(Comeback comeback)
    {
        _context.Comebacks.Remove(comeback);
        await _context.SaveChangesAsync();
    }
}