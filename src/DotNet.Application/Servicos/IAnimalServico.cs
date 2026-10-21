using DotNet.Application.Dtos;

namespace DotNet.Application.Servicos;

public interface IAnimalServico
{
    Task<IEnumerable<AnimalResponse>> ListarAsync();

    Task<AnimalResponse> ObterPorIdAsync(int id);

    Task<IEnumerable<AnimalResponse>> ListarPorResponsavelIdAsync(int responsavelId);

    Task<IEnumerable<AnimalResponse>> ListarPorSpeciesAsync(string species);

    Task<IEnumerable<AnimalResponse>> ListarPorBreedAsync(string breed);

    Task<AnimalResponse> ObterPorRgaAsync(string rga);

    Task<AnimalResponse> CriarAsync(CriarAnimalRequest request);

    Task AtualizarAsync(int id, AtualizarAnimalRequest request);

    Task RemoverAsync(int id);
}