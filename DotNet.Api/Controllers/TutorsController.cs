using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/tutors")]
public class TutorsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Tutor>> GetAll()
    {
        return Ok(InMemoryDatabase.Tutors);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Tutor> GetById(int id)
    {
        var tutor = InMemoryDatabase.Tutors.FirstOrDefault(t => t.Id == id);

        if (tutor is null)
        {
            return NotFound();
        }

        return Ok(tutor);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<Tutor> GetByCpf(string cpf)
    {
        var tutor = InMemoryDatabase.Tutors.FirstOrDefault(t => t.Cpf == cpf);

        if (tutor is null)
        {
            return NotFound();
        }

        return Ok(tutor);
    }

    [HttpPost]
    public ActionResult<Tutor> Create(Tutor tutor)
    {
        if (string.IsNullOrWhiteSpace(tutor.Name) ||
            string.IsNullOrWhiteSpace(tutor.Email) ||
            string.IsNullOrWhiteSpace(tutor.Phone) ||
            string.IsNullOrWhiteSpace(tutor.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        tutor.Id = InMemoryDatabase.Tutors.Any()
            ? InMemoryDatabase.Tutors.Max(t => t.Id) + 1
            : 1;

        tutor.CreatedAt = DateTime.UtcNow;
        tutor.IsActive = true;

        InMemoryDatabase.Tutors.Add(tutor);

        return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, tutor);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, Tutor updatedTutor)
    {
        var tutor = InMemoryDatabase.Tutors.FirstOrDefault(t => t.Id == id);

        if (tutor is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedTutor.Name) ||
            string.IsNullOrWhiteSpace(updatedTutor.Email) ||
            string.IsNullOrWhiteSpace(updatedTutor.Phone) ||
            string.IsNullOrWhiteSpace(updatedTutor.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        tutor.Name = updatedTutor.Name;
        tutor.Email = updatedTutor.Email;
        tutor.Phone = updatedTutor.Phone;
        tutor.Cpf = updatedTutor.Cpf;
        tutor.IsActive = updatedTutor.IsActive;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var tutor = InMemoryDatabase.Tutors.FirstOrDefault(t => t.Id == id);

        if (tutor is null)
        {
            return NotFound();
        }

        InMemoryDatabase.Tutors.Remove(tutor);

        return NoContent();
    }
}