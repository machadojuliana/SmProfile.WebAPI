using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMProfile.WebAPI.DTOs;
using SMProfile.WebAPI.Interfaces;
using SMProfile.WebAPI.Models;

namespace SMProfile.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GruposController : ControllerBase
{
    private readonly IGrupoRepository _grupoRepository;

    public GruposController(IGrupoRepository grupoRepository) => _grupoRepository = grupoRepository;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListarTodos()
    {
        var grupos = await _grupoRepository.ListarTodos();
        var response = grupos.Select(g => new GrupoResponseDTO(
            g.IdGrupo, g.NomeGrupo, g.TipoGrupo, g.Membros, g.DataDebut, g.Descricao));
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var grupo = await _grupoRepository.BuscarPorId(id);
        if (grupo is null) return NotFound(new { mensagem = "Grupo não encontrado." });

        return Ok(new GrupoResponseDTO(
            grupo.IdGrupo, grupo.NomeGrupo, grupo.TipoGrupo, grupo.Membros, grupo.DataDebut, grupo.Descricao));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Cadastrar([FromBody] GrupoCreateUpdateDTO dto)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        if (dto.DataDebut.Year < 1995 || dto.DataDebut > hoje)
            return BadRequest(new { mensagem = "A data de debut deve estar entre 1995 e a data atual." });

        if (await _grupoRepository.ExisteNome(dto.NomeGrupo))
            return BadRequest(new { mensagem = "Já existe um grupo cadastrado com este nome." });

        var grupo = new Grupo
        {
            NomeGrupo = dto.NomeGrupo.Trim(),
            TipoGrupo = dto.TipoGrupo.Trim(),
            Membros = dto.Membros.Trim(),
            DataDebut = dto.DataDebut,
            Descricao = dto.Descricao.Trim()
        };

        await _grupoRepository.Cadastrar(grupo);
        return CreatedAtAction(nameof(BuscarPorId), new { id = grupo.IdGrupo },
            new GrupoResponseDTO(grupo.IdGrupo, grupo.NomeGrupo, grupo.TipoGrupo, grupo.Membros, grupo.DataDebut, grupo.Descricao));
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Atualizar(int id, [FromBody] GrupoCreateUpdateDTO dto)
    {
        var grupo = await _grupoRepository.BuscarPorId(id);
        if (grupo is null) return NotFound(new { mensagem = "Grupo não encontrado." });

        var hoje = DateOnly.FromDateTime(DateTime.Now);
        if (dto.DataDebut.Year < 1995 || dto.DataDebut > hoje)
            return BadRequest(new { mensagem = "A data de debut deve estar entre 1995 e a data atual." });

        if (await _grupoRepository.ExisteNome(dto.NomeGrupo, id))
            return BadRequest(new { mensagem = "Já existe outro grupo cadastrado com este nome." });

        var primeiroComeback = await _grupoRepository.ObterDataPrimeiroComeback(id);
        if (primeiroComeback.HasValue && dto.DataDebut > primeiroComeback.Value)
            return BadRequest(new { mensagem = $"A data de debut não pode ser posterior ao primeiro comeback do grupo ({primeiroComeback.Value:dd/MM/yyyy})." });

        grupo.NomeGrupo = dto.NomeGrupo.Trim();
        grupo.TipoGrupo = dto.TipoGrupo.Trim();
        grupo.Membros = dto.Membros.Trim();
        grupo.DataDebut = dto.DataDebut;
        grupo.Descricao = dto.Descricao.Trim();

        await _grupoRepository.Atualizar(grupo);
        return Ok(new { mensagem = "Grupo atualizado com sucesso." });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Deletar(int id)
    {
        var grupo = await _grupoRepository.BuscarPorId(id);
        if (grupo is null) return NotFound(new { mensagem = "Grupo não encontrado." });

        await _grupoRepository.Deletar(grupo);
        return Ok(new { mensagem = "Grupo desativado com sucesso." });
    }
}