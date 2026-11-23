using Microsoft.EntityFrameworkCore;
using SMProfile.WebAPI.Context;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Repositories;

public class AlbumFisicoRepository : IAlbumFisicoRepository
{
    private readonly SMProfileDbContext _context;
    public AlbumFisicoRepository(SMProfileDbContext context) => _context = context;

    public async Task<List<AlbumFisico>> PesquisarPorVersao(string? versao)
    {
        var query = _context.AlbunsFisicos
            .Include(af => af.Comeback)
            .Include(af => af.Grupo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(versao))
            query = query.Where(af => af.Versao.Contains(versao));

        return await query.ToListAsync();
    }

    public async Task<List<AlbumFisico>> ListarPorComeback(int idComeback) =>
        await _context.AlbunsFisicos
            .Include(af => af.Comeback)
            .Include(af => af.Grupo)
            .Where(af => af.IdComeback == idComeback)
            .ToListAsync();

    public async Task<AlbumFisico?> BuscarPorId(int id) =>
        await _context.AlbunsFisicos
            .Include(af => af.Comeback)
            .Include(af => af.Grupo)
            .FirstOrDefaultAsync(af => af.IdAlbumFisico == id);

    public async Task<bool> ExisteVersaoNoComeback(int idComeback, string versao, int? ignorarId = null) =>
        await _context.AlbunsFisicos.AnyAsync(af =>
            af.IdComeback == idComeback &&
            af.Versao.ToLower() == versao.ToLower() &&
            (!ignorarId.HasValue || af.IdAlbumFisico != ignorarId.Value));

    public async Task<int> ContarPorComeback(int idComeback) =>
        await _context.AlbunsFisicos.CountAsync(af => af.IdComeback == idComeback);

    public async Task Cadastrar(AlbumFisico album)
    {
        _context.AlbunsFisicos.Add(album);
        await _context.SaveChangesAsync();
    }

    public async Task Atualizar(AlbumFisico album)
    {
        _context.AlbunsFisicos.Update(album);
        await _context.SaveChangesAsync();
    }

    public async Task Deletar(AlbumFisico album)
    {
        _context.AlbunsFisicos.Remove(album);
        await _context.SaveChangesAsync();
    }
}