using DotNet.Api.Hateoas;
using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using DotNet.Domain.Consultas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

/// <summary>
/// CRUD dos relatos do tutor, persistidos no MongoDB.
/// </summary>
[Authorize]
[ApiController]
[Route("api/relatos")]
[Produces("application/json")]
public class RelatosController : ControllerBase
{
    private readonly IRelatoTutorServico _servico;
    private readonly ILogger<RelatosController> _logger;

    public RelatosController(IRelatoTutorServico servico, ILogger<RelatosController> logger)
    {
        _servico = servico;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ColecaoPaginada<Recurso<RelatoTutorResponse>>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ColecaoPaginada<Recurso<RelatoTutorResponse>>>> GetAll(
        [FromQuery] FiltroRelatoTutor filtro,
        [FromQuery] ParametrosPaginacao paginacao)
    {
        var resultado = (await _servico.ListarAsync(filtro, paginacao)).Mapear(ComLinks);

        return Ok(ColecaoPaginada<Recurso<RelatoTutorResponse>>.De(resultado, LinksPaginacao.Criar(Request, resultado)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Recurso<RelatoTutorResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Recurso<RelatoTutorResponse>>> GetById(string id)
    {
        return Ok(ComLinks(await _servico.ObterPorIdAsync(id)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(RelatoTutorResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RelatoTutorResponse>> Create(CriarRelatoTutorRequest request)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarRelatoTutor");
        activity?.SetTag("relato.animalId", request.AnimalId);

        HttpContext.Items["recurso"] = "relato_tutor";

        var criado = await _servico.CriarAsync(request);

        AplicacaoMetricas.CadastrosRealizados.Add(1,
            new KeyValuePair<string, object?>("recurso", "relato_tutor"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation(
            "Relato {RelatoId} criado com sucesso para Animal {AnimalId}.",
            criado.Id, criado.AnimalId);

        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, AtualizarRelatoTutorRequest request)
    {
        await _servico.AtualizarAsync(id, request);

        _logger.LogInformation("Relato {RelatoId} atualizado com sucesso.", id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {
        await _servico.RemoverAsync(id);

        _logger.LogInformation("Relato {RelatoId} removido com sucesso.", id);

        return NoContent();
    }

    private Recurso<RelatoTutorResponse> ComLinks(RelatoTutorResponse relato)
    {
        var self = Url.Action(nameof(GetById), new { id = relato.Id })!;

        return new Recurso<RelatoTutorResponse>(relato,
        [
            new("self", self, "GET"),
            new("update", self, "PUT"),
            new("delete", self, "DELETE"),
            new("animal", Url.Action("GetById", "Animais", new { id = relato.AnimalId })!, "GET")
        ]);
    }
}
