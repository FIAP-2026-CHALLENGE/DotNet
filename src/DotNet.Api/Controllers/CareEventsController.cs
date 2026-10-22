using DotNet.Api.Infraestrutura.Observabilidade;
using Microsoft.AspNetCore.Authorization;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Mvc;
using DotNet.Domain.Consultas;
using DotNet.Api.Hateoas;

namespace DotNet.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/care-events")]
[Produces("application/json")]
public class CareEventsController : ControllerBase
{
    private readonly ICareEventServico _servico;
    private readonly ILogger<CareEventsController> _logger;

    public CareEventsController(ICareEventServico servico, ILogger<CareEventsController> logger)
    {
        _servico = servico;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ColecaoPaginada<Recurso<CareEventResponse>>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ColecaoPaginada<Recurso<CareEventResponse>>>> GetAll(
        [FromQuery] FiltroCareEvent filtro,
        [FromQuery] ParametrosPaginacao paginacao)
    {
        var resultado = (await _servico.ListarAsync(filtro, paginacao)).Mapear(ComLinks);

        return Ok(ColecaoPaginada<Recurso<CareEventResponse>>.De(resultado, LinksPaginacao.Criar(Request, resultado)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Recurso<CareEventResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Recurso<CareEventResponse>>> GetById(int id)
    {
        return Ok(ComLinks(await _servico.ObterPorIdAsync(id)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CareEventResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CareEventResponse>> Create(CriarCareEventRequest request)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarCareEvent");
        activity?.SetTag("careEvent.petId", request.PetId);
        activity?.SetTag("careEvent.type", request.Type);

        HttpContext.Items["recurso"] = "care_event";

        var criado = await _servico.CriarAsync(request);

        AplicacaoMetricas.CadastrosRealizados.Add(1,
            new KeyValuePair<string, object?>("recurso", "care_event"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation(
            "CareEvent {CareEventId} ({Tipo}) criado com sucesso para Animal {AnimalId}.",
            criado.Id, criado.Type, criado.PetId);

        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, AtualizarCareEventRequest request)
    {
        await _servico.AtualizarAsync(id, request);

        _logger.LogInformation("CareEvent {CareEventId} atualizado com sucesso.", id);

        return NoContent();
    }

    [HttpPatch("{id:int}/complete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Complete(int id)
    {
        await _servico.ConcluirAsync(id);

        _logger.LogInformation("CareEvent {CareEventId} concluído com sucesso.", id);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await _servico.RemoverAsync(id);

        _logger.LogInformation("CareEvent {CareEventId} removido com sucesso.", id);

        return NoContent();
    }

    private Recurso<CareEventResponse> ComLinks(CareEventResponse careEvent)
    {
        var self = Url.Action(nameof(GetById), new { id = careEvent.Id })!;

        return new Recurso<CareEventResponse>(careEvent,
        [
            new("self", self, "GET"),
            new("update", self, "PUT"),
            new("complete", Url.Action(nameof(Complete), new { id = careEvent.Id })!, "PATCH"),
            new("delete", self, "DELETE"),
            new("animal", Url.Action("GetById", "Animais", new { id = careEvent.PetId })!, "GET")
        ]);
    }
}