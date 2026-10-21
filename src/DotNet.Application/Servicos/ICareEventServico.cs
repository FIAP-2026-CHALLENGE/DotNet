using DotNet.Application.Dtos;

namespace DotNet.Application.Servicos;

public interface ICareEventServico
{
    Task<IEnumerable<CareEventResponse>> ListarAsync();

    Task<CareEventResponse> ObterPorIdAsync(int id);

    Task<IEnumerable<CareEventResponse>> ListarPorAnimalIdAsync(int animalId);

    Task<IEnumerable<CareEventResponse>> ListarPorStatusAsync(string status);

    Task<IEnumerable<CareEventResponse>> ListarPorTypeAsync(string type);

    Task<IEnumerable<CareEventResponse>> ListarPorAnimalIdEStatusAsync(int animalId, string status);

    Task<IEnumerable<CareEventResponse>> ListarAtrasadosAsync();

    Task<CareEventResponse> CriarAsync(CriarCareEventRequest request);

    Task AtualizarAsync(int id, AtualizarCareEventRequest request);

    Task ConcluirAsync(int id);

    Task RemoverAsync(int id);
}