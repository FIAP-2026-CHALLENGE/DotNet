using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public interface ICareEventServico
{
    Task<ResultadoPaginado<CareEventResponse>> ListarAsync(FiltroCareEvent filtro, ParametrosPaginacao paginacao);

    Task<CareEventResponse> ObterPorIdAsync(int id);

    Task<CareEventResponse> CriarAsync(CriarCareEventRequest request);

    Task AtualizarAsync(int id, AtualizarCareEventRequest request);

    Task ConcluirAsync(int id);

    Task RemoverAsync(int id);
}