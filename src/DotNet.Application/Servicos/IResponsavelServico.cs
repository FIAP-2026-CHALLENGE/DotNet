using DotNet.Application.Dtos;

namespace DotNet.Application.Servicos;

public interface IResponsavelServico
{
    Task<IEnumerable<ResponsavelResponse>> ListarAsync();

    Task<ResponsavelResponse> ObterPorIdAsync(int id);

    Task<ResponsavelResponse> ObterPorCpfAsync(string cpf);

    Task<ResponsavelResponse> CriarAsync(CriarResponsavelRequest request);

    Task AtualizarAsync(int id, AtualizarResponsavelRequest request);

    Task RemoverAsync(int id);
}