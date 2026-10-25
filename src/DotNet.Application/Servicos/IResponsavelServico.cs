using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public interface IResponsavelServico
{
    Task<ResultadoPaginado<ResponsavelResponse>> ListarAsync(FiltroResponsavel filtro, ParametrosPaginacao paginacao);

    Task<ResponsavelResponse> ObterPorIdAsync(int id);

    Task<ResponsavelResponse> ObterPorCpfAsync(string cpf);

    Task<ResponsavelResponse> CriarAsync(CriarResponsavelRequest request);

    Task AtualizarAsync(int id, AtualizarResponsavelRequest request);

    Task RemoverAsync(int id);
}