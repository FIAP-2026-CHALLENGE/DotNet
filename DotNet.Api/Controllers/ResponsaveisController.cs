using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/responsaveis")]
[Produces("application/json")]
public class ResponsaveisController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResponsaveisController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Responsavel>), 200)]
    public async Task<ActionResult<IEnumerable<Responsavel>>> GetAll()
    {
        var responsaveis = await _context.Responsaveis
            .AsNoTracking()
            .ToListAsync();

        return Ok(responsaveis);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetById(int id)
    {
        var responsavel = await _context.Responsaveis
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        return Ok(responsavel);
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(Responsavel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Responsavel>> GetByCpf(string cpf)
    {
        var responsavel = await _context.Responsaveis
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Cpf == cpf);

        if (responsavel is null)
        {
            return NotFound();
        }

        return Ok(responsavel);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Responsavel), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Responsavel>> Create(Responsavel responsavel)
    {
        if (string.IsNullOrWhiteSpace(responsavel.Name) ||
            string.IsNullOrWhiteSpace(responsavel.Email) ||
            string.IsNullOrWhiteSpace(responsavel.Phone) ||
            string.IsNullOrWhiteSpace(responsavel.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        var cpfAlreadyExists = await _context.Responsaveis
            .CountAsync(r => r.Cpf == responsavel.Cpf) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered.");
        }

        responsavel.Id = 0;
        responsavel.CreatedAt = DateTime.UtcNow;
        responsavel.IsActive = true;

        _context.Responsaveis.Add(responsavel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = responsavel.Id }, responsavel);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, Responsavel updatedResponsavel)
    {
        var responsavel = await _context.Responsaveis
            .FirstOrDefaultAsync(r => r.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updatedResponsavel.Name) ||
            string.IsNullOrWhiteSpace(updatedResponsavel.Email) ||
            string.IsNullOrWhiteSpace(updatedResponsavel.Phone) ||
            string.IsNullOrWhiteSpace(updatedResponsavel.Cpf))
        {
            return BadRequest("Name, email, phone and CPF are required.");
        }

        var cpfAlreadyExists = await _context.Responsaveis
            .CountAsync(r => r.Cpf == updatedResponsavel.Cpf && r.Id != id) > 0;

        if (cpfAlreadyExists)
        {
            return BadRequest("CPF already registered by another responsavel.");
        }

        responsavel.Name = updatedResponsavel.Name;
        responsavel.Email = updatedResponsavel.Email;
        responsavel.Phone = updatedResponsavel.Phone;
        responsavel.Cpf = updatedResponsavel.Cpf;
        responsavel.IsActive = updatedResponsavel.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var responsavel = await _context.Responsaveis
            .FirstOrDefaultAsync(r => r.Id == id);

        if (responsavel is null)
        {
            return NotFound();
        }

        _context.Responsaveis.Remove(responsavel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}