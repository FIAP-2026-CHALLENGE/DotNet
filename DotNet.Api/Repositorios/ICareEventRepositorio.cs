using DotNet.Api.Models;

namespace DotNet.Api.Repositorios;

public interface ICareEventRepositorio
{
    Task<IEnumerable<CareEvent>> ObterTodosAsync();

    Task<CareEvent?> ObterPorIdAsync(int id);

    Task<IEnumerable<CareEvent>> ObterPorAnimalIdAsync(int animalId);

    Task<IEnumerable<CareEvent>> ObterPorStatusAsync(string status);

    Task<IEnumerable<CareEvent>> ObterPorTypeAsync(string type);

    Task<IEnumerable<CareEvent>> ObterPorAnimalIdEStatusAsync(int animalId, string status);

    Task<IEnumerable<CareEvent>> ObterAtrasadosAsync();

    Task AdicionarAsync(CareEvent careEvent);

    Task RemoverAsync(CareEvent careEvent);

    Task SalvarAsync();
}