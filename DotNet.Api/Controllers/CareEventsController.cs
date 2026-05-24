using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/care-events")]
[Produces("application/json")]
public class CareEventsController : ControllerBase
{
    private readonly AppDbContext _context;

    private static readonly string[] AllowedTypes =
    {
        "VACCINE",
        "DEWORMING",
        "MEDICATION",
        "CHECKUP",
        "RETURN",
        "EXAM",
        "GROOMING",
        "SURGERY",
        "OTHER"
    };

    private static readonly string[] AllowedStatuses =
    {
        "PENDING",
        "COMPLETED",
        "OVERDUE",
        "CANCELED"
    };

    private static readonly string[] AllowedPriorities =
    {
        "LOW",
        "MEDIUM",
        "HIGH",
        "CRITICAL"
    };

    public CareEventsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetAll()
    {
        var events = await _context.CareEvents
            .AsNoTracking()
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CareEvent), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<CareEvent>> GetById(int id)
    {
        var careEvent = await _context.CareEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        return Ok(careEvent);
    }

    [HttpGet("animal/{animalId:int}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByAnimalId(int animalId)
    {
        var animalExists = await _context.Animais
            .CountAsync(a => a.Id == animalId) > 0;

        if (!animalExists)
        {
            return NotFound("Animal not found.");
        }

        var events = await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.PetId == animalId)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByStatus(string status)
    {
        var normalizedStatus = status.ToUpper();

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            return BadRequest("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        var events = await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.Status == normalizedStatus)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByType(string type)
    {
        var normalizedType = type.ToUpper();

        if (!AllowedTypes.Contains(normalizedType))
        {
            return BadRequest("Invalid care event type.");
        }

        var events = await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.Type == normalizedType)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("animal/{animalId:int}/status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetByAnimalIdAndStatus(int animalId, string status)
    {
        var animalExists = await _context.Animais
            .CountAsync(a => a.Id == animalId) > 0;

        if (!animalExists)
        {
            return NotFound("Animal not found.");
        }

        var normalizedStatus = status.ToUpper();

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            return BadRequest("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        var events = await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.PetId == animalId && e.Status == normalizedStatus)
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<CareEvent>), 200)]
    public async Task<ActionResult<IEnumerable<CareEvent>>> GetOverdue()
    {
        var today = DateTime.UtcNow.Date;

        var events = await _context.CareEvents
            .AsNoTracking()
            .Where(e =>
                e.Status == "OVERDUE" ||
                e.ScheduledDate.Date < today &&
                e.Status != "COMPLETED" &&
                e.Status != "CANCELED")
            .ToListAsync();

        return Ok(events);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CareEvent), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<CareEvent>> Create(CareEvent careEvent)
    {
        var animalExists = await _context.Animais
            .CountAsync(a => a.Id == careEvent.PetId) > 0;

        if (!animalExists)
        {
            return BadRequest("AnimalId does not exist.");
        }

        var validationError = ValidateCareEvent(careEvent);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        careEvent.Id = 0;
        careEvent.Type = careEvent.Type.ToUpper();
        careEvent.Status = careEvent.Status.ToUpper();
        careEvent.Priority = careEvent.Priority.ToUpper();
        careEvent.CreatedAt = DateTime.UtcNow;
        careEvent.IsActive = true;

        _context.CareEvents.Add(careEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = careEvent.Id }, careEvent);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, CareEvent updatedCareEvent)
    {
        var careEvent = await _context.CareEvents
            .FirstOrDefaultAsync(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        var animalExists = await _context.Animais
            .CountAsync(a => a.Id == updatedCareEvent.PetId) > 0;

        if (!animalExists)
        {
            return BadRequest("AnimalId does not exist.");
        }

        var validationError = ValidateCareEvent(updatedCareEvent);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        careEvent.PetId = updatedCareEvent.PetId;
        careEvent.Type = updatedCareEvent.Type.ToUpper();
        careEvent.Title = updatedCareEvent.Title;
        careEvent.Description = updatedCareEvent.Description;
        careEvent.ScheduledDate = updatedCareEvent.ScheduledDate;
        careEvent.CompletedDate = updatedCareEvent.CompletedDate;
        careEvent.Status = updatedCareEvent.Status.ToUpper();
        careEvent.Priority = updatedCareEvent.Priority.ToUpper();
        careEvent.Notes = updatedCareEvent.Notes;
        careEvent.IsActive = updatedCareEvent.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:int}/complete")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Complete(int id)
    {
        var careEvent = await _context.CareEvents
            .FirstOrDefaultAsync(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        if (careEvent.Status == "CANCELED")
        {
            return BadRequest("Canceled events cannot be completed.");
        }

        careEvent.Status = "COMPLETED";
        careEvent.CompletedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var careEvent = await _context.CareEvents
            .FirstOrDefaultAsync(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        _context.CareEvents.Remove(careEvent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static string? ValidateCareEvent(CareEvent careEvent)
    {
        if (careEvent.PetId <= 0 ||
            string.IsNullOrWhiteSpace(careEvent.Type) ||
            string.IsNullOrWhiteSpace(careEvent.Title) ||
            string.IsNullOrWhiteSpace(careEvent.Status) ||
            string.IsNullOrWhiteSpace(careEvent.Priority))
        {
            return "AnimalId, type, title, status and priority are required.";
        }

        var normalizedType = careEvent.Type.ToUpper();
        var normalizedStatus = careEvent.Status.ToUpper();
        var normalizedPriority = careEvent.Priority.ToUpper();

        if (!AllowedTypes.Contains(normalizedType))
        {
            return "Type must be VACCINE, DEWORMING, MEDICATION, CHECKUP, RETURN, EXAM, GROOMING, SURGERY or OTHER.";
        }

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            return "Status must be PENDING, COMPLETED, OVERDUE or CANCELED.";
        }

        if (!AllowedPriorities.Contains(normalizedPriority))
        {
            return "Priority must be LOW, MEDIUM, HIGH or CRITICAL.";
        }

        if (careEvent.ScheduledDate == default)
        {
            return "ScheduledDate is required.";
        }

        if (normalizedStatus == "COMPLETED" && careEvent.CompletedDate is null)
        {
            careEvent.CompletedDate = DateTime.UtcNow;
        }

        return null;
    }
}