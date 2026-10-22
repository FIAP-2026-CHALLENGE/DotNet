using DotNet.Domain.Entidades;
using DotNet.Domain.Consultas;

namespace DotNet.Domain.Repositorios;

public interface ICareEventRepositorio
{
    Task<CareEvent?> ObterPorIdAsync(int id);

    Task<ResultadoPaginado<CareEvent>> ListarPaginadoAsync(FiltroCareEvent filtro, ParametrosPaginacao paginacao);

    Task AdicionarAsync(CareEvent careEvent);

    Task RemoverAsync(CareEvent careEvent);

    Task SalvarAsync();
}