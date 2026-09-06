namespace DotNet.Api.Models;

public class CareEvent
{
    public static readonly string[] AllowedTypes =
    {
        "VACCINE", "DEWORMING", "MEDICATION", "CHECKUP",
        "RETURN", "EXAM", "GROOMING", "SURGERY", "OTHER"
    };

    public static readonly string[] AllowedStatuses =
    {
        "PENDING", "COMPLETED", "OVERDUE", "CANCELED"
    };

    public static readonly string[] AllowedPriorities =
    {
        "LOW", "MEDIUM", "HIGH", "CRITICAL"
    };

    public int Id { get; set; }
    public int PetId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public static CareEvent Criar(
        int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes)
    {
        var (normalizedType, normalizedStatus, normalizedPriority) =
            ValidarDados(petId, type, title, status, priority, scheduledDate);

        if (normalizedStatus == "COMPLETED" && completedDate is null)
        {
            completedDate = DateTime.UtcNow;
        }

        return new CareEvent
        {
            PetId = petId,
            Type = normalizedType,
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            ScheduledDate = scheduledDate,
            CompletedDate = completedDate,
            Status = normalizedStatus,
            Priority = normalizedPriority,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Atualizar(
        int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes, bool isActive)
    {
        var (normalizedType, normalizedStatus, normalizedPriority) =
            ValidarDados(petId, type, title, status, priority, scheduledDate);

        PetId = petId;
        Type = normalizedType;
        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ScheduledDate = scheduledDate;
        CompletedDate = completedDate;
        Status = normalizedStatus;
        Priority = normalizedPriority;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        IsActive = isActive;
    }

    /// <summary>
    /// Regra de transição de estado: um evento cancelado nunca pode ser concluído.
    /// </summary>
    public void Concluir()
    {
        if (Status == "CANCELED")
        {
            throw new InvalidOperationException("Canceled events cannot be completed.");
        }

        Status = "COMPLETED";
        CompletedDate = DateTime.UtcNow;
    }

    private static (string Type, string Status, string Priority) ValidarDados(
        int petId, string type, string title, string status, string priority, DateTime scheduledDate)
    {
        if (petId <= 0 || string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(title) ||
            string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(priority))
        {
            throw new ArgumentException("AnimalId, type, title, status and priority are required.");
        }

        var normalizedType = type.Trim().ToUpper();
        var normalizedStatus = status.Trim().ToUpper();
        var normalizedPriority = priority.Trim().ToUpper();

        if (!AllowedTypes.Contains(normalizedType))
            throw new ArgumentException("Type must be VACCINE, DEWORMING, MEDICATION, CHECKUP, RETURN, EXAM, GROOMING, SURGERY or OTHER.", nameof(type));

        if (!AllowedStatuses.Contains(normalizedStatus))
            throw new ArgumentException("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.", nameof(status));

        if (!AllowedPriorities.Contains(normalizedPriority))
            throw new ArgumentException("Priority must be LOW, MEDIUM, HIGH or CRITICAL.", nameof(priority));

        if (scheduledDate == default)
            throw new ArgumentException("ScheduledDate is required.", nameof(scheduledDate));

        return (normalizedType, normalizedStatus, normalizedPriority);
    }
}