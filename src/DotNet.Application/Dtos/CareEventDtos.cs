using DotNet.Domain.Entidades;

namespace DotNet.Application.Dtos;

public record CriarCareEventRequest(
    int PetId, string Type, string Title, DateTime ScheduledDate,
    string Status, string Priority,
    string? Description = null, DateTime? CompletedDate = null, string? Notes = null);

public record AtualizarCareEventRequest(
    int PetId, string Type, string Title, DateTime ScheduledDate,
    string Status, string Priority,
    string? Description = null, DateTime? CompletedDate = null, string? Notes = null,
    bool IsActive = true);

public record CareEventResponse(
    int Id, int PetId, string Type, string Title, string? Description,
    DateTime ScheduledDate, DateTime? CompletedDate, string Status, string Priority,
    string? Notes, DateTime CreatedAt, bool IsActive)
{
    public static CareEventResponse De(CareEvent c) =>
        new(c.Id, c.PetId, c.Type, c.Title, c.Description, c.ScheduledDate,
            c.CompletedDate, c.Status, c.Priority, c.Notes, c.CreatedAt, c.IsActive);
}
