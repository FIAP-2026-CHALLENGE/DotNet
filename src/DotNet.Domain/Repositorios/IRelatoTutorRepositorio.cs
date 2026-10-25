using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;

namespace DotNet.Domain.Repositorios;

public interface IRelatoTutorRepositorio
{
    Task<RelatoTutor?> ObterPorIdAsync(string id);

    Task<ResultadoPaginado<RelatoTutor>> ListarPaginadoAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao);

    Task AdicionarAsync(RelatoTutor relato);

    Task AtualizarAsync(RelatoTutor relato);

    Task RemoverAsync(RelatoTutor relato);
}