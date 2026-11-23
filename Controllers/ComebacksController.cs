using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMProfile.WebAPI.DTOs;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;
using System.Text.Json;

namespace SMProfile.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComebacksController : ControllerBase
{
    private readonly IComebackRepository _comebackRepository;
    private readonly IGrupoRepository _grupoRepository;
    private readonly IImageService _imageService;

    public ComebacksController(
        IComebackRepository comebackRepository,
        IGrupoRepository grupoRepository,
        IImageService imageService)
    {
        _comebackRepository = comebackRepository;
        _grupoRepository = grupoRepository;
        _imageService = imageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] int? idGrupo,
        [FromQuery] string? titulo,
        [FromQuery] DateOnly? dataLancamento)
    {
        var comebacks = await _comebackRepository.ListarComFiltros(idGrupo, titulo, dataLancamento);
        return Ok(comebacks.Select(MapearComeback));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var comeback = await _comebackRepository.BuscarPorId(id);
        if (comeback is null) return NotFound(new { mensagem = "Comeback não encontrado." });

        return Ok(MapearComeback(comeback));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromForm] ComebackCreateFormDTO dto)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        if (dto.DataLancamento.Year < 1995 || dto.DataLancamento > hoje)
            return BadRequest(new { mensagem = "A data de lançamento deve ser a partir de 1995 e não pode ser futura." });

        string? urlCapa = dto.CapaUrl;
        if (dto.CapaArquivo is not null)
            urlCapa = await _imageService.UploadImageAsync(dto.CapaArquivo);

        if (string.IsNullOrWhiteSpace(urlCapa))
            return BadRequest(new { mensagem = "A imagem de capa do comeback é obrigatória (envie o arquivo ou a URL)." });

        List<AlbumFisicoInicialDTO>? albunsIniciais;
        try
        {
            albunsIniciais = JsonSerializer.Deserialize<List<AlbumFisicoInicialDTO>>(
                dto.AlbunsFisicosJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return BadRequest(new { mensagem = "Formato inválido para AlbunsFisicosJson." });
        }

        if (albunsIniciais is null || albunsIniciais.Count == 0)
            return BadRequest(new { mensagem = "Cada comeback deve estar associado a pelo menos um grupo e álbum físico." });

        var versoesDuplicadas = albunsIniciais
            .GroupBy(a => a.Versao.Trim().ToLower())
            .Any(g => g.Count() > 1);
        if (versoesDuplicadas)
            return BadRequest(new { mensagem = "O nome da versão de um álbum físico não pode se repetir dentro do mesmo comeback." });

        var entidadesAlbuns = new List<AlbumFisico>();
        foreach (var item in albunsIniciais)
        {
            var grupo = await _grupoRepository.BuscarPorId(item.IdGrupo);
            if (grupo is null)
                return BadRequest(new { mensagem = $"Grupo ID {item.IdGrupo} não encontrado." });

            if (grupo.DataDebut > dto.DataLancamento)
                return BadRequest(new { mensagem = $"A data de debut do grupo '{grupo.NomeGrupo}' ({grupo.DataDebut:dd/MM/yyyy}) é posterior à data deste comeback." });

            entidadesAlbuns.Add(new AlbumFisico
            {
                IdGrupo = item.IdGrupo,
                Versao = item.Versao.Trim()
            });
        }

        var comeback = new Comeback
        {
            Titulo = dto.Titulo.Trim(),
            TitleTrack = dto.TitleTrack.Trim(),
            DataLancamento = dto.DataLancamento,
            MusicVideo = dto.MusicVideo.Trim(),
            Capa = urlCapa,
            AlbunsFisicos = entidadesAlbuns
        };

        await _comebackRepository.Cadastrar(comeback);
        var criado = await _comebackRepository.BuscarPorId(comeback.IdComeback);
        return CreatedAtAction(nameof(BuscarPorId), new { id = comeback.IdComeback }, MapearComeback(criado!));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromForm] ComebackUpdateFormDTO dto)
    {
        var comeback = await _comebackRepository.BuscarPorId(id);
        if (comeback is null) return NotFound(new { mensagem = "Comeback não encontrado." });

        var hoje = DateOnly.FromDateTime(DateTime.Now);
        if (dto.DataLancamento.Year < 1995 || dto.DataLancamento > hoje)
            return BadRequest(new { mensagem = "A data de lançamento deve ser a partir de 1995 e não pode ser futura." });

        foreach (var album in comeback.AlbunsFisicos)
        {
            if (album.Grupo is not null && album.Grupo.DataDebut > dto.DataLancamento)
                return BadRequest(new { mensagem = $"A nova data de lançamento é anterior ao debut do grupo '{album.Grupo.NomeGrupo}'." });
        }

        if (dto.CapaArquivo is not null)
        {
            var novaCapa = await _imageService.UploadImageAsync(dto.CapaArquivo);
            if (!string.IsNullOrWhiteSpace(novaCapa))
                comeback.Capa = novaCapa;
        }
        else if (!string.IsNullOrWhiteSpace(dto.CapaUrl))
        {
            comeback.Capa = dto.CapaUrl;
        }

        if (string.IsNullOrWhiteSpace(comeback.Capa))
            return BadRequest(new { mensagem = "A capa do comeback é obrigatória." });

        comeback.Titulo = dto.Titulo.Trim();
        comeback.TitleTrack = dto.TitleTrack.Trim();
        comeback.DataLancamento = dto.DataLancamento;
        comeback.MusicVideo = dto.MusicVideo.Trim();

        await _comebackRepository.Atualizar(comeback);
        return Ok(new { mensagem = "Comeback atualizado com sucesso." });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Deletar(int id)
    {
        var comeback = await _comebackRepository.BuscarPorId(id);
        if (comeback is null) return NotFound(new { mensagem = "Comeback não encontrado." });

        await _comebackRepository.Deletar(comeback);
        return Ok(new { mensagem = "Comeback excluído com sucesso." });
    }

    private static ComebackResponseDTO MapearComeback(Comeback c) => new(
        c.IdComeback,
        c.Titulo,
        c.TitleTrack,
        c.DataLancamento,
        c.MusicVideo,
        c.Capa,
        c.AlbunsFisicos.Select(af => new AlbumFisicoResponseDTO(
            af.IdAlbumFisico,
            af.IdComeback,
            c.Titulo,
            af.IdGrupo,
            af.Grupo?.NomeGrupo ?? string.Empty,
            af.Versao
        )).ToList()
    );
}