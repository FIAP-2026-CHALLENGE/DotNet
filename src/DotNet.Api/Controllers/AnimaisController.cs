using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/animais")]
[Produces("application/json")]
public class AnimaisController : ControllerBase
{
    private readonly IAnimalServico _servico;
    private readonly ILogger<AnimaisController> _logger;

    public AnimaisController(IAnimalServico servico, ILogger<AnimaisController> logger)
    {
        _servico = servico;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AnimalResponse>), 200)]
    public async Task<ActionResult<IEnumerable<AnimalResponse>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AnimalResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AnimalResponse>> GetById(int id)
    {
        return Ok(await _servico.ObterPorIdAsync(id));
    }

    [HttpGet("responsavel/{responsavelId:int}")]
    [ProducesResponseType(typeof(IEnumerable<AnimalResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<AnimalResponse>>> GetByResponsavelId(int responsavelId)
    {
        return Ok(await _servico.ListarPorResponsavelIdAsync(responsavelId));
    }

    [HttpGet("species/{species}")]
    [ProducesResponseType(typeof(IEnumerable<AnimalResponse>), 200)]
    public async Task<ActionResult<IEnumerable<AnimalResponse>>> GetBySpecies(string species)
    {
        return Ok(await _servico.ListarPorSpeciesAsync(species));
    }

    [HttpGet("breed/{breed}")]
    [ProducesResponseType(typeof(IEnumerable<AnimalResponse>), 200)]
    public async Task<ActionResult<IEnumerable<AnimalResponse>>> GetByBreed(string breed)
    {
        return Ok(await _servico.ListarPorBreedAsync(breed));
    }

    [HttpGet("rga/{rga}")]
    [ProducesResponseType(typeof(AnimalResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AnimalResponse>> GetByRga(string rga)
    {
        return Ok(await _servico.ObterPorRgaAsync(rga));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AnimalResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AnimalResponse>> Create(CriarAnimalRequest request)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarAnimal");
        activity?.SetTag("animal.responsavelId", request.ResponsavelId);
        activity?.SetTag("animal.species", request.Species);

        HttpContext.Items["recurso"] = "animal";

        var criado = await _servico.CriarAsync(request);

        AplicacaoMetricas.CadastrosRealizados.Add(1,
            new KeyValuePair<string, object?>("recurso", "animal"),
            new KeyValuePair<string, object?>("status", "sucesso"));

        _logger.LogInformation(
            "Animal {AnimalId} ({Nome}) criado com sucesso para Responsavel {ResponsavelId}.",
            criado.Id, criado.Name, criado.ResponsavelId);

        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, AtualizarAnimalRequest request)
    {
        await _servico.AtualizarAsync(id, request);

        _logger.LogInformation("Animal {AnimalId} atualizado com sucesso.", id);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await _servico.RemoverAsync(id);

        _logger.LogInformation("Animal {AnimalId} removido com sucesso.", id);

        return NoContent();
    }
}