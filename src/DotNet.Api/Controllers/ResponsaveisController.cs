using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Application.Dtos;
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
    [ProducesResponseType(typeof(IEnumerable<ResponsavelResponse>), 200)]
    public async Task<ActionResult<IEnumerable<ResponsavelResponse>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResponsavelResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ResponsavelResponse>> GetById(int id)
    {
        return Ok(await _servico.ObterPorIdAsync(id));
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(ResponsavelResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ResponsavelResponse>> GetByCpf(string cpf)
    {
        return Ok(await _servico.ObterPorCpfAsync(cpf));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponsavelResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ResponsavelResponse>> Create(CriarResponsavelRequest request)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarResponsavel");
        activity?.SetTag("responsavel.cpf", request.Cpf);

        HttpContext.Items["recurso"] = "responsavel";

        var criado = await _servico.CriarAsync(request);

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
    public async Task<IActionResult> Update(int id, AtualizarResponsavelRequest request)
    {
        await _servico.AtualizarAsync(id, request);

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