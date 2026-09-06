using DotNet.Api.Excecoes;
using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Api.Models;
using DotNet.Api.Servicos;
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
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAll()
    {
        return Ok(await _servico.ListarAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Animal), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Animal>> GetById(int id)
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

    [HttpGet("responsavel/{responsavelId:int}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetByResponsavelId(int responsavelId)
    {
        try
        {
            return Ok(await _servico.ListarPorResponsavelIdAsync(responsavelId));
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("species/{species}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetBySpecies(string species)
    {
        return Ok(await _servico.ListarPorSpeciesAsync(species));
    }

    [HttpGet("breed/{breed}")]
    [ProducesResponseType(typeof(IEnumerable<Animal>), 200)]
    public async Task<ActionResult<IEnumerable<Animal>>> GetByBreed(string breed)
    {
        return Ok(await _servico.ListarPorBreedAsync(breed));
    }

    [HttpGet("rga/{rga}")]
    [ProducesResponseType(typeof(Animal), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Animal>> GetByRga(string rga)
    {
        try
        {
            return Ok(await _servico.ObterPorRgaAsync(rga));
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Animal), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Animal>> Create(Animal animal)
    {
        using var activity = AplicacaoMetricas.ActivitySource.StartActivity("CriarAnimal");
        activity?.SetTag("animal.responsavelId", animal.ResponsavelId);
        activity?.SetTag("animal.species", animal.Species);

        try
        {
            var criado = await _servico.CriarAsync(
                animal.ResponsavelId, animal.Name, animal.Nickname, animal.Species,
                animal.Breed, animal.BirthDate, animal.Weight, animal.Sex, animal.Rga);

            AplicacaoMetricas.CadastrosRealizados.Add(1,
                new KeyValuePair<string, object?>("recurso", "animal"),
                new KeyValuePair<string, object?>("status", "sucesso"));

            _logger.LogInformation(
                "Animal {AnimalId} ({Nome}) criado com sucesso para Responsavel {ResponsavelId}.",
                criado.Id, criado.Name, criado.ResponsavelId);

            return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
        }
        catch (Exception ex) when (ex is ArgumentException or RegraDeNegocioException)
        {
            AplicacaoMetricas.CadastrosRealizados.Add(1,
                new KeyValuePair<string, object?>("recurso", "animal"),
                new KeyValuePair<string, object?>("status", "falha"));

            _logger.LogWarning(ex, "Falha ao criar Animal: {Motivo}", ex.Message);

            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Animal updatedAnimal)
    {
        try
        {
            await _servico.AtualizarAsync(
                id, updatedAnimal.ResponsavelId, updatedAnimal.Name, updatedAnimal.Nickname,
                updatedAnimal.Species, updatedAnimal.Breed, updatedAnimal.BirthDate,
                updatedAnimal.Weight, updatedAnimal.Sex, updatedAnimal.Rga, updatedAnimal.IsActive);

            _logger.LogInformation("Animal {AnimalId} atualizado com sucesso.", id);

            return NoContent();
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex) when (ex is ArgumentException or RegraDeNegocioException)
        {
            _logger.LogWarning(ex, "Falha ao atualizar Animal {AnimalId}: {Motivo}", id, ex.Message);

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

            _logger.LogInformation("Animal {AnimalId} removido com sucesso.", id);

            return NoContent();
        }
        catch (RecursoNaoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
    }
}