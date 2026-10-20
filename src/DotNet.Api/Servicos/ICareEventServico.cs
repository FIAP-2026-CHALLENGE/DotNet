using DotNet.Api.Models;

namespace DotNet.Api.Servicos;

public interface ICareEventServico
{
    Task<IEnumerable<CareEvent>> ListarAsync();

    Task<CareEvent> ObterPorIdAsync(int id);

    Task<IEnumerable<CareEvent>> ListarPorAnimalIdAsync(int animalId);

    Task<IEnumerable<CareEvent>> ListarPorStatusAsync(string status);

    Task<IEnumerable<CareEvent>> ListarPorTypeAsync(string type);

    Task<IEnumerable<CareEvent>> ListarPorAnimalIdEStatusAsync(int animalId, string status);

    Task<IEnumerable<CareEvent>> ListarAtrasadosAsync();

    Task<CareEvent> CriarAsync(
        int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes);

    Task AtualizarAsync(
        int id, int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes, bool isActive);

    Task ConcluirAsync(int id);

    Task RemoverAsync(int id);
}