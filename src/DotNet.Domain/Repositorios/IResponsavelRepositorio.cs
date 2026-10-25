using DotNet.Domain.Entidades;
using DotNet.Domain.Consultas;

namespace DotNet.Domain.Repositorios;

public interface IResponsavelRepositorio
{
    Task<Responsavel?> ObterPorIdAsync(int id);

    Task<Responsavel?> ObterPorCpfAsync(string cpf);

    Task<bool> ExisteAsync(int id);

    Task<bool> CpfExisteAsync(string cpf, int? idParaIgnorar = null);

    Task<ResultadoPaginado<Responsavel>> ListarPaginadoAsync(FiltroResponsavel filtro, ParametrosPaginacao paginacao);

    Task AdicionarAsync(Responsavel responsavel);

    Task RemoverAsync(Responsavel responsavel);

    Task SalvarAsync();
}