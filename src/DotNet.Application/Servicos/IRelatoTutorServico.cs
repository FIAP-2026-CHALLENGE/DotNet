using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public interface IRelatoTutorServico
{
    Task<ResultadoPaginado<RelatoTutorResponse>> ListarAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao);

    Task<RelatoTutorResponse> ObterPorIdAsync(string id);

    Task<RelatoTutorResponse> CriarAsync(CriarRelatoTutorRequest request);

    Task AtualizarAsync(string id, AtualizarRelatoTutorRequest request);

    Task RemoverAsync(string id);
}
