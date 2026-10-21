using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Domain.Entidades;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

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
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CareEvent), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CareEvent>> GetById(int id)
    {
        return Ok(await _servico.ObterPorIdAsync(id));
    }

    [HttpGet("animal/{animalId:int}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByAnimalId(int animalId)
    {
        return Ok(await _servico.ListarPorAnimalIdAsync(animalId));
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByStatus(string status)
    {
        return Ok(await _servico.ListarPorStatusAsync(status));
    }

    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByType(string type)
    {
        return Ok(await _servico.ListarPorTypeAsync(type));
    }

    [HttpGet("animal/{animalId:int}/status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByAnimalIdAndStatus(int animalId, string status)
    {
        return Ok(await _servico.ListarPorAnimalIdEStatusAsync(animalId, status));
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetOverdue()
    {
        return Ok(await _servico.ListarAtrasadosAsync());
    }

    [HttpPost]
    [ProducesResponseType(typeof(CareEvent), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CareEvent>> Create(CareEvent careEvent)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarCareEvent");
        activity?.SetTag("careEvent.petId", careEvent.PetId);
        activity?.SetTag("careEvent.type", careEvent.Type);

        HttpContext.Items["recurso"] = "care_event";

        var criado = await _servico.CriarAsync(
            careEvent.PetId, careEvent.Type, careEvent.Title, careEvent.Description,
            careEvent.ScheduledDate, careEvent.CompletedDate, careEvent.Status,
            careEvent.Priority, careEvent.Notes);

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
    public async Task<IActionResult> Update(int id, CareEvent updatedCareEvent)
    {
        await _servico.AtualizarAsync(
            id, updatedCareEvent.PetId, updatedCareEvent.Type, updatedCareEvent.Title,
            updatedCareEvent.Description, updatedCareEvent.ScheduledDate, updatedCareEvent.CompletedDate,
            updatedCareEvent.Status, updatedCareEvent.Priority, updatedCareEvent.Notes, updatedCareEvent.IsActive);

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