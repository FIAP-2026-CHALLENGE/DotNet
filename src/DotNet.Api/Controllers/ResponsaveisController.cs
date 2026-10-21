using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Domain.Entidades;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/responsaveis")]
[Produces("application/json")]
public class ResponsaveisController : ControllerBase
{
    private readonly IResponsavelServico _servico;
    private readonly ILogger<ResponsaveisController> _logger;

    public ResponsaveisController(IResponsavelServico servico, ILogger<ResponsaveisController> logger)
    {
        _servico = servico;
        _logger = logger;
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
        return Ok(await _servico.ObterPorIdAsync(id));
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetByCpf(string cpf)
    {
        return Ok(await _servico.ObterPorCpfAsync(cpf));
    }

    [HttpPost]
    [ProducesResponseType(typeof(Responsavel), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Responsavel>> Create(Responsavel responsavel)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarResponsavel");
        activity?.SetTag("responsavel.cpf", responsavel.Cpf);

        HttpContext.Items["recurso"] = "responsavel";

        var criado = await _servico.CriarAsync(responsavel.Name, responsavel.Email, responsavel.Phone, responsavel.Cpf);

        AplicacaoMetricas.CadastrosRealizados.Add(1,
            new KeyValuePair<string, object?>("recurso", "responsavel"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation("Responsavel {ResponsavelId} criado com sucesso (CPF {Cpf}).", criado.Id, criado.Cpf);

        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Responsavel updatedResponsavel)
    {
        await _servico.AtualizarAsync(
            id, updatedResponsavel.Name, updatedResponsavel.Email,
            updatedResponsavel.Phone, updatedResponsavel.Cpf, updatedResponsavel.IsActive);

        _logger.LogInformation("Responsavel {ResponsavelId} atualizado com sucesso.", id);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await _servico.RemoverAsync(id);

        _logger.LogInformation("Responsavel {ResponsavelId} removido com sucesso.", id);

        return NoContent();
    }
}