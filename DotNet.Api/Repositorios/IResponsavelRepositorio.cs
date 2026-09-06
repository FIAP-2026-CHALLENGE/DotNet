using DotNet.Api.Models;

namespace DotNet.Api.Repositorios;

public interface IResponsavelRepositorio
{
    Task<IEnumerable<Responsavel>> ObterTodosAsync();

    Task<Responsavel?> ObterPorIdAsync(int id);

    Task<Responsavel?> ObterPorCpfAsync(string cpf);

    Task<bool> ExisteAsync(int id);

    Task<bool> CpfExisteAsync(string cpf, int? idParaIgnorar = null);

    Task AdicionarAsync(Responsavel responsavel);

    Task RemoverAsync(Responsavel responsavel);

    Task SalvarAsync();
}