using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedSpecies =
    {
        "DOG",
        "CAT"
    };

    public PetsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pet>>> GetAll()
    {
        var pets = await _context.Pets
            .AsNoTracking()
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pet>> GetById(int id)
    {
        var pet = await _context.Pets
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        return Ok(pet);
    }

    [HttpGet("tutor/{tutorId:int}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByTutorId(int tutorId)
    {
        var tutorExists = await _context.Tutors
            .CountAsync(t => t.Id == tutorId) > 0;

        if (!tutorExists)
        {
            return NotFound("Tutor not found.");
        }

        var pets = await _context.Pets
            .AsNoTracking()
            .Where(p => p.TutorId == tutorId)
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("species/{species}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetBySpecies(string species)
    {
        var normalizedSpecies = species.ToUpper();

        var pets = await _context.Pets
            .AsNoTracking()
            .Where(p => p.Species == normalizedSpecies)
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("breed/{breed}")]
    public async Task<ActionResult<IEnumerable<Pet>>> GetByBreed(string breed)
    {
        var normalizedBreed = breed.ToLower();

        var pets = await _context.Pets
            .AsNoTracking()
            .Where(p => p.Breed.ToLower() == normalizedBreed)
            .ToListAsync();

        return Ok(pets);
    }

    [HttpGet("rga/{rga}")]
    public async Task<ActionResult<Pet>> GetByRga(string rga)
    {
        var pet = await _context.Pets
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Rga == rga);

        if (pet is null)
        {
            return NotFound();
        }

        return Ok(pet);
    }

    [HttpPost]
    public async Task<ActionResult<Pet>> Create(Pet pet)
    {
        var tutorExists = await _context.Tutors
            .CountAsync(t => t.Id == pet.TutorId) > 0;

        if (!tutorExists)
        {
            return BadRequest("TutorId does not exist.");
        }

        var validationError = ValidatePet(pet);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var rgaAlreadyExists = !string.IsNullOrWhiteSpace(pet.Rga) &&
            await _context.Pets.CountAsync(p => p.Rga == pet.Rga) > 0;

        if (rgaAlreadyExists)
        {
            return BadRequest("RGA already registered.");
        }

        pet.Id = 0;
        pet.Species = pet.Species.ToUpper();
        pet.Sex = pet.Sex.ToUpper();
        pet.CreatedAt = DateTime.UtcNow;
        pet.IsActive = true;

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Pet updatedPet)
    {
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        var tutorExists = await _context.Tutors
            .CountAsync(t => t.Id == updatedPet.TutorId) > 0;

        if (!tutorExists)
        {
            return BadRequest("TutorId does not exist.");
        }

        var validationError = ValidatePet(updatedPet);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        var rgaAlreadyExists = !string.IsNullOrWhiteSpace(updatedPet.Rga) &&
            await _context.Pets.CountAsync(p => p.Rga == updatedPet.Rga && p.Id != id) > 0;

        if (rgaAlreadyExists)
        {
            return BadRequest("RGA already registered by another pet.");
        }

        pet.TutorId = updatedPet.TutorId;
        pet.Name = updatedPet.Name;
        pet.Nickname = updatedPet.Nickname;
        pet.Species = updatedPet.Species.ToUpper();
        pet.Breed = updatedPet.Breed;
        pet.BirthDate = updatedPet.BirthDate;
        pet.Weight = updatedPet.Weight;
        pet.Sex = updatedPet.Sex.ToUpper();
        pet.Rga = updatedPet.Rga;
        pet.IsActive = updatedPet.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _context.Pets
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        _context.Pets.Remove(pet);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidatePet(Pet pet)
    {
        if (pet.TutorId <= 0 ||
            string.IsNullOrWhiteSpace(pet.Name) ||
            string.IsNullOrWhiteSpace(pet.Species) ||
            string.IsNullOrWhiteSpace(pet.Breed) ||
            string.IsNullOrWhiteSpace(pet.Sex) ||
            pet.Weight <= 0)
        {
            return "TutorId, name, species, breed, sex and valid weight are required.";
        }

        var normalizedSpecies = pet.Species.ToUpper();

        if (!AllowedSpecies.Contains(normalizedSpecies))
        {
            return "Species must be DOG or CAT in this MVP version.";
        }

        return null;
    }
}