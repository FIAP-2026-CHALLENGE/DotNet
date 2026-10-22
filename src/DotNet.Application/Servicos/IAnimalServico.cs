using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public interface IAnimalServico
{
    Task<ResultadoPaginado<AnimalResponse>> ListarAsync(FiltroAnimal filtro, ParametrosPaginacao paginacao);

    Task<AnimalResponse> ObterPorIdAsync(int id);

    Task<AnimalResponse> ObterPorRgaAsync(string rga);

    Task<AnimalResponse> CriarAsync(CriarAnimalRequest request);

    Task AtualizarAsync(int id, AtualizarAnimalRequest request);

    Task RemoverAsync(int id);
}