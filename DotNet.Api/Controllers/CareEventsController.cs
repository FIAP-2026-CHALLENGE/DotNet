using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/care-events")]
public class CareEventsController : ControllerBase
{
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

    [HttpGet]
    public ActionResult<IEnumerable<CareEvent>> GetAll()
    {
        return Ok(InMemoryDatabase.CareEvents);
    }

    [HttpGet("{id:int}")]
    public ActionResult<CareEvent> GetById(int id)
    {
        var careEvent = InMemoryDatabase.CareEvents.FirstOrDefault(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        return Ok(careEvent);
    }

    [HttpGet("pet/{petId:int}")]
    public ActionResult<IEnumerable<CareEvent>> GetByPetId(int petId)
    {
        var petExists = InMemoryDatabase.Pets.Any(p => p.Id == petId);

        if (!petExists)
        {
            return NotFound("Pet not found.");
        }

        var events = InMemoryDatabase.CareEvents
            .Where(e => e.PetId == petId)
            .ToList();

        return Ok(events);
    }

    [HttpGet("status/{status}")]
    public ActionResult<IEnumerable<CareEvent>> GetByStatus(string status)
    {
        var normalizedStatus = status.ToUpper();

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            return BadRequest("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        var events = InMemoryDatabase.CareEvents
            .Where(e => e.Status.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(events);
    }

    [HttpGet("type/{type}")]
    public ActionResult<IEnumerable<CareEvent>> GetByType(string type)
    {
        var normalizedType = type.ToUpper();

        if (!AllowedTypes.Contains(normalizedType))
        {
            return BadRequest("Invalid care event type.");
        }

        var events = InMemoryDatabase.CareEvents
            .Where(e => e.Type.Equals(normalizedType, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(events);
    }

    [HttpGet("pet/{petId:int}/status/{status}")]
    public ActionResult<IEnumerable<CareEvent>> GetByPetIdAndStatus(int petId, string status)
    {
        var petExists = InMemoryDatabase.Pets.Any(p => p.Id == petId);

        if (!petExists)
        {
            return NotFound("Pet not found.");
        }

        var normalizedStatus = status.ToUpper();

        if (!AllowedStatuses.Contains(normalizedStatus))
        {
            return BadRequest("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        var events = InMemoryDatabase.CareEvents
            .Where(e => e.PetId == petId &&
                        e.Status.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(events);
    }

    [HttpGet("overdue")]
    public ActionResult<IEnumerable<CareEvent>> GetOverdue()
    {
        var events = InMemoryDatabase.CareEvents
            .Where(e => e.Status.Equals("OVERDUE", StringComparison.OrdinalIgnoreCase) ||
                        e.ScheduledDate.Date < DateTime.UtcNow.Date &&
                        !e.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase) &&
                        !e.Status.Equals("CANCELED", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(events);
    }

    [HttpPost]
    public ActionResult<CareEvent> Create(CareEvent careEvent)
    {
        var petExists = InMemoryDatabase.Pets.Any(p => p.Id == careEvent.PetId);

        if (!petExists)
        {
            return BadRequest("PetId does not exist.");
        }

        var validationError = ValidateCareEvent(careEvent);

        if (validationError is not null)
        {
            return BadRequest(validationError);
        }

        careEvent.Id = InMemoryDatabase.CareEvents.Any()
            ? InMemoryDatabase.CareEvents.Max(e => e.Id) + 1
            : 1;

        careEvent.Type = careEvent.Type.ToUpper();
        careEvent.Status = careEvent.Status.ToUpper();
        careEvent.Priority = careEvent.Priority.ToUpper();
        careEvent.CreatedAt = DateTime.UtcNow;
        careEvent.IsActive = true;

        InMemoryDatabase.CareEvents.Add(careEvent);

        return CreatedAtAction(nameof(GetById), new { id = careEvent.Id }, careEvent);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, CareEvent updatedCareEvent)
    {
        var careEvent = InMemoryDatabase.CareEvents.FirstOrDefault(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        var petExists = InMemoryDatabase.Pets.Any(p => p.Id == updatedCareEvent.PetId);

        if (!petExists)
        {
            return BadRequest("PetId does not exist.");
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

        return NoContent();
    }

    [HttpPatch("{id:int}/complete")]
    public IActionResult Complete(int id)
    {
        var careEvent = InMemoryDatabase.CareEvents.FirstOrDefault(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        if (careEvent.Status.Equals("CANCELED", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Canceled events cannot be completed.");
        }

        careEvent.Status = "COMPLETED";
        careEvent.CompletedDate = DateTime.UtcNow;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var careEvent = InMemoryDatabase.CareEvents.FirstOrDefault(e => e.Id == id);

        if (careEvent is null)
        {
            return NotFound();
        }

        InMemoryDatabase.CareEvents.Remove(careEvent);

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
            return "PetId, type, title, status and priority are required.";
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