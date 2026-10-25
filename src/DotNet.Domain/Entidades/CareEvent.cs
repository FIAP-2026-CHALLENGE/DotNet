namespace DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;

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
            throw new RegraDeNegocioException("Eventos cancelados não podem ser concluídos.");
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
            throw new RegraDeNegocioException("AnimalId, tipo, título, status e prioridade são obrigatórios.");
        }

        var normalizedType = type.Trim().ToUpper();
        var normalizedStatus = status.Trim().ToUpper();
        var normalizedPriority = priority.Trim().ToUpper();

        if (!AllowedTypes.Contains(normalizedType))
            throw new RegraDeNegocioException("O tipo deve ser VACCINE, DEWORMING, MEDICATION, CHECKUP, RETURN, EXAM, GROOMING, SURGERY ou OTHER.");

        if (!AllowedStatuses.Contains(normalizedStatus))
            throw new RegraDeNegocioException("O status deve ser PENDING, COMPLETED, OVERDUE ou CANCELED.");

        if (!AllowedPriorities.Contains(normalizedPriority))
            throw new RegraDeNegocioException("A prioridade deve ser LOW, MEDIUM, HIGH ou CRITICAL.");

        if (scheduledDate == default)
            throw new RegraDeNegocioException("A data agendada é obrigatória.");

        return (normalizedType, normalizedStatus, normalizedPriority);
    }
}