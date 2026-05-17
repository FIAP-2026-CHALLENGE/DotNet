using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/tutors")]
[Produces("application/json")]
public class TutorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TutorsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Tutor>), 200)]
    public async Task<ActionResult<IEnumerable<Tutor>>> GetAll()
    {
        var tutors = await _context.Tutors
            .AsNoTracking()
            .ToListAsync();

        return Ok(tutors);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Tutor), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Tutor>> GetById(int id)
    {
        var tutor = await _context.Tutors
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tutor is null)
        {
            return NotFound();
        }

        return Ok(tutor);
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(Tutor), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Tutor>> GetByCpf(string cpf)
    {
        var tutor = await _context.Tutors
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Cpf == cpf);

        if (tutor is null)
        {
            return NotFound();
        }

        return Ok(tutor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Tutor), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Tutor>> Create(Tutor tutor)
    {
        if (string.IsNullOrWhiteSpace(tutor.Name) ||
            string.IsNullOrWhiteSpace(tutor.Email) ||
            string.IsNullOrWhiteSpace(tutor.Phone) ||
            string.IsNullOrWhiteSpace(tutor.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        var cpfAlreadyExists = await _context.Tutors
            .CountAsync(t => t.Cpf == tutor.Cpf) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered.");
        }

        tutor.Id = 0;
        tutor.CreatedAt = DateTime.UtcNow;
        tutor.IsActive = true;

        _context.Tutors.Add(tutor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, tutor);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Tutor updatedTutor)
    {
        var tutor = await _context.Tutors
            .FirstOrDefaultAsync(t => t.Id == id);

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

        var cpfAlreadyExists = await _context.Tutors
            .CountAsync(t => t.Cpf == updatedTutor.Cpf && t.Id != id) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered by another tutor.");
        }

        tutor.Name = updatedTutor.Name;
        tutor.Email = updatedTutor.Email;
        tutor.Phone = updatedTutor.Phone;
        tutor.Cpf = updatedTutor.Cpf;
        tutor.IsActive = updatedTutor.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var tutor = await _context.Tutors
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tutor is null)
        {
            return NotFound();
        }

        _context.Tutors.Remove(tutor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}