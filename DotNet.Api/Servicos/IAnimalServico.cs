using DotNet.Api.Models;

namespace DotNet.Api.Servicos;

public interface IAnimalServico
{
    Task<IEnumerable<Animal>> ListarAsync();

    Task<Animal> ObterPorIdAsync(int id);

    Task<IEnumerable<Animal>> ListarPorResponsavelIdAsync(int responsavelId);

    Task<IEnumerable<Animal>> ListarPorSpeciesAsync(string species);

    Task<IEnumerable<Animal>> ListarPorBreedAsync(string breed);

    Task<Animal> ObterPorRgaAsync(string rga);

    Task<Animal> CriarAsync(
        int responsavelId, string name, string nickname, string species,
        string breed, DateTime birthDate, decimal weight, string sex, string rga);

    Task AtualizarAsync(
        int id, int responsavelId, string name, string nickname, string species,
        string breed, DateTime birthDate, decimal weight, string sex, string rga, bool isActive);

    Task RemoverAsync(int id);
}