using DotNet.Api.Excecoes;
using DotNet.Api.Models;
using DotNet.Api.Servicos;
using Microsoft.AspNetCore.Mvc;
using DotNet.Api.Infraestrutura.Observabilidade;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/responsaveis")]
[Produces("application/json")]
public class ResponsaveisController : ControllerBase
{
    private readonly IResponsavelServico _servico;

    public ResponsaveisController(IResponsavelServico servico)
    {
        _servico = servico;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Responsavel>), 200)]
    public async Task<ActionResult<IEnumerable<Responsavel>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetById(int id)
    {
        try
        {
            return Ok(await _servico.ObterPorIdAsync(id));
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetByCpf(string cpf)
    {
        try
        {
            return Ok(await _servico.ObterPorCpfAsync(cpf));
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Responsavel), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Responsavel>> Create(Responsavel responsavel)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarResponsavel");
        activity?.SetTag("responsavel.cpf", responsavel.Cpf);

        try
        {
            var criado = await _servico.CriarAsync(responsavel.Name, responsavel.Email, responsavel.Phone, responsavel.Cpf);

            AplicacaoMetricas.CadastrosRealizados.Add(1,
                new KeyValuePair<string, object?>("recurso", "responsavel"),
                new KeyValuePair<string, object?>("status", "sucesso"));

            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }
        catch (Exception ex) when (ex is ArgumentException or RegraDeNegocioException)
        {
            AplicacaoMetricas.CadastrosRealizados.Add(1,
                new KeyValuePair<string, object?>("recurso", "responsavel"),
                new KeyValuePair<string, object?>("status", "falha"));

            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Responsavel updatedResponsavel)
    {
        try
        {
            await _servico.AtualizarAsync(
                id, updatedResponsavel.Name, updatedResponsavel.Email,
                updatedResponsavel.Phone, updatedResponsavel.Cpf, updatedResponsavel.IsActive);

            return NoContent();
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex) when (ex is ArgumentException or RegraDeNegocioException)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _servico.RemoverAsync(id);

            return NoContent();
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }
}