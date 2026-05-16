using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/pets")]
public class PetsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Pet>> GetAll()
    {
        return Ok(InMemoryDatabase.Pets);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Pet> GetById(int id)
    {
        var pet = InMemoryDatabase.Pets.FirstOrDefault(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        return Ok(pet);
    }

    [HttpGet("tutor/{tutorId:int}")]
    public ActionResult<IEnumerable<Pet>> GetByTutorId(int tutorId)
    {
        var tutorExists = InMemoryDatabase.Tutors.Any(t => t.Id == tutorId);

        if (!tutorExists)
        {
            return NotFound("Tutor not found.");
        }

        var pets = InMemoryDatabase.Pets
            .Where(p => p.TutorId == tutorId)
            .ToList();

        return Ok(pets);
    }

    [HttpGet("species/{species}")]
    public ActionResult<IEnumerable<Pet>> GetBySpecies(string species)
    {
        var pets = InMemoryDatabase.Pets
            .Where(p => p.Species.Equals(species, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(pets);
    }

    [HttpGet("breed/{breed}")]
    public ActionResult<IEnumerable<Pet>> GetByBreed(string breed)
    {
        var pets = InMemoryDatabase.Pets
            .Where(p => p.Breed.Equals(breed, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(pets);
    }

    [HttpGet("rga/{rga}")]
    public ActionResult<Pet> GetByRga(string rga)
    {
        var pet = InMemoryDatabase.Pets.FirstOrDefault(p => p.Rga == rga);

        if (pet is null)
        {
            return NotFound();
        }

        return Ok(pet);
    }

    [HttpPost]
    public ActionResult<Pet> Create(Pet pet)
    {
        var tutorExists = InMemoryDatabase.Tutors.Any(t => t.Id == pet.TutorId);

        if (!tutorExists)
        {
            return BadRequest("TutorId does not exist.");
        }

        if (string.IsNullOrWhiteSpace(pet.Name) ||
            string.IsNullOrWhiteSpace(pet.Species) ||
            string.IsNullOrWhiteSpace(pet.Breed) ||
            string.IsNullOrWhiteSpace(pet.Sex) ||
            pet.Weight <= 0)
        {
            return BadRequest("Name, species, breed, sex and valid weight are required.");
        }

        var allowedSpecies = new[] { "DOG", "CAT" };

        if (!allowedSpecies.Contains(pet.Species.ToUpper()))
        {
            return BadRequest("Species must be DOG or CAT in this MVP version.");
        }

        pet.Id = InMemoryDatabase.Pets.Any()
            ? InMemoryDatabase.Pets.Max(p => p.Id) + 1
            : 1;

        pet.Species = pet.Species.ToUpper();
        pet.Sex = pet.Sex.ToUpper();
        pet.CreatedAt = DateTime.UtcNow;
        pet.IsActive = true;

        InMemoryDatabase.Pets.Add(pet);

        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Pet updatedPet)
    {
        var pet = InMemoryDatabase.Pets.FirstOrDefault(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        var tutorExists = InMemoryDatabase.Tutors.Any(t => t.Id == updatedPet.TutorId);

        if (!tutorExists)
        {
            return BadRequest("TutorId does not exist.");
        }

        if (string.IsNullOrWhiteSpace(updatedPet.Name) ||
            string.IsNullOrWhiteSpace(updatedPet.Species) ||
            string.IsNullOrWhiteSpace(updatedPet.Breed) ||
            string.IsNullOrWhiteSpace(updatedPet.Sex) ||
            updatedPet.Weight <= 0)
        {
            return BadRequest("Name, species, breed, sex and valid weight are required.");
        }

        var allowedSpecies = new[] { "DOG", "CAT" };

        if (!allowedSpecies.Contains(updatedPet.Species.ToUpper()))
        {
            return BadRequest("Species must be DOG or CAT in this MVP version.");
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

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var pet = InMemoryDatabase.Pets.FirstOrDefault(p => p.Id == id);

        if (pet is null)
        {
            return NotFound();
        }

        InMemoryDatabase.Pets.Remove(pet);

        return NoContent();
    }
}