using DotNet.Api.Models;

namespace DotNet.Api.Servicos;

public interface IResponsavelServico
{
    Task<IEnumerable<Responsavel>> ListarAsync();

    Task<Responsavel> ObterPorIdAsync(int id);

    Task<Responsavel> ObterPorCpfAsync(string cpf);

    Task<Responsavel> CriarAsync(string name, string email, string phone, string cpf);

    Task AtualizarAsync(int id, string name, string email, string phone, string cpf, bool isActive);

    Task RemoverAsync(int id);
}