using DotNet.Api.Infraestrutura.Observabilidade;
using Microsoft.AspNetCore.Authorization;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CareEventResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CareEventResponse>> GetById(int id)
    {
        return Ok(await _servico.ObterPorIdAsync(id));
    }

    [HttpGet("animal/{animalId:int}")]
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetByAnimalId(int animalId)
    {
        return Ok(await _servico.ListarPorAnimalIdAsync(animalId));
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetByStatus(string status)
    {
        return Ok(await _servico.ListarPorStatusAsync(status));
    }

    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetByType(string type)
    {
        return Ok(await _servico.ListarPorTypeAsync(type));
    }

    [HttpGet("animal/{animalId:int}/status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetByAnimalIdAndStatus(int animalId, string status)
    {
        return Ok(await _servico.ListarPorAnimalIdEStatusAsync(animalId, status));
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<CareEventResponse>), 200)]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetOverdue()
    {
        return Ok(await _servico.ListarAtrasadosAsync());
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
}