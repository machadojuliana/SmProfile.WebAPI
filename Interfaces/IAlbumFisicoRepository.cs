using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Interfaces;

public interface IAlbumFisicoRepository
{
    Task<List<AlbumFisico>> PesquisarPorVersao(string? versao);
    Task<List<AlbumFisico>> ListarPorComeback(int idComeback);
    Task<AlbumFisico?> BuscarPorId(int id);
    Task<bool> ExisteVersaoNoComeback(int idComeback, string versao, int? ignorarId = null);
    Task<int> ContarPorComeback(int idComeback);
    Task Cadastrar(AlbumFisico album);
    Task Atualizar(AlbumFisico album);
    Task Deletar(AlbumFisico album);
}