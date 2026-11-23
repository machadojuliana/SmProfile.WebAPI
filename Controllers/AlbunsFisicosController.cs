using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMProfile.WebAPI.DTOs;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Controllers;

[Route("api/albuns-fisicos")]
[ApiController]
public class AlbunsFisicosController : ControllerBase
{
    private readonly IAlbumFisicoRepository _albumRepository;
    private readonly IComebackRepository _comebackRepository;
    private readonly IGrupoRepository _grupoRepository;

    public AlbunsFisicosController(
        IAlbumFisicoRepository albumRepository,
        IComebackRepository comebackRepository,
        IGrupoRepository grupoRepository)
    {
        _albumRepository = albumRepository;
        _comebackRepository = comebackRepository;
        _grupoRepository = grupoRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Pesquisar([FromQuery] string? versao)
    {
        var albuns = await _albumRepository.PesquisarPorVersao(versao);
        return Ok(albuns.Select(MapearAlbum));
    }

    [HttpGet("comeback/{idComeback:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorComeback(int idComeback)
    {
        var albuns = await _albumRepository.ListarPorComeback(idComeback);
        return Ok(albuns.Select(MapearAlbum));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] AlbumFisicoCreateDTO dto)
    {
        var comeback = await _comebackRepository.BuscarPorId(dto.IdComeback);
        if (comeback is null) return NotFound(new { mensagem = "Comeback não encontrado." });

        var grupo = await _grupoRepository.BuscarPorId(dto.IdGrupo);
        if (grupo is null) return NotFound(new { mensagem = "Grupo não encontrado." });

        if (grupo.DataDebut > comeback.DataLancamento)
            return BadRequest(new { mensagem = "A data de debut do grupo não pode ser posterior ao lançamento do comeback." });

        if (await _albumRepository.ExisteVersaoNoComeback(dto.IdComeback, dto.Versao))
            return BadRequest(new { mensagem = "Já existe um álbum físico com esta versão neste comeback." });

        var album = new AlbumFisico
        {
            IdComeback = dto.IdComeback,
            IdGrupo = dto.IdGrupo,
            Versao = dto.Versao.Trim()
        };

        await _albumRepository.Cadastrar(album);
        var criado = await _albumRepository.BuscarPorId(album.IdAlbumFisico);
        return Created(string.Empty, MapearAlbum(criado!));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AlbumFisicoUpdateDTO dto)
    {
        var album = await _albumRepository.BuscarPorId(id);
        if (album is null) return NotFound(new { mensagem = "Álbum físico não encontrado." });

        var grupo = await _grupoRepository.BuscarPorId(dto.IdGrupo);
        if (grupo is null) return NotFound(new { mensagem = "Grupo não encontrado." });

        if (album.Comeback is not null && grupo.DataDebut > album.Comeback.DataLancamento)
            return BadRequest(new { mensagem = "A data de debut do grupo não pode ser posterior ao lançamento do comeback." });

        if (await _albumRepository.ExisteVersaoNoComeback(album.IdComeback, dto.Versao, id))
            return BadRequest(new { mensagem = "Já existe outra versão com este nome neste comeback." });

        album.IdGrupo = dto.IdGrupo;
        album.Versao = dto.Versao.Trim();

        await _albumRepository.Atualizar(album);
        return Ok(new { mensagem = "Álbum físico atualizado com sucesso." });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Deletar(int id)
    {
        var album = await _albumRepository.BuscarPorId(id);
        if (album is null) return NotFound(new { mensagem = "Álbum físico não encontrado." });

        var totalNoComeback = await _albumRepository.ContarPorComeback(album.IdComeback);
        if (totalNoComeback <= 1)
            return BadRequest(new { mensagem = "Não é possível excluir o único álbum físico vinculado a este comeback." });

        await _albumRepository.Deletar(album);
        return Ok(new { mensagem = "Álbum físico excluído com sucesso." });
    }

    private static AlbumFisicoResponseDTO MapearAlbum(AlbumFisico af) => new(
        af.IdAlbumFisico,
        af.IdComeback,
        af.Comeback?.Titulo ?? string.Empty,
        af.IdGrupo,
        af.Grupo?.NomeGrupo ?? string.Empty,
        af.Versao
    );
}