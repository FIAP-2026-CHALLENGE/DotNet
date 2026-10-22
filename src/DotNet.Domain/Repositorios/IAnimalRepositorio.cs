using DotNet.Domain.Entidades;
using DotNet.Domain.Consultas;

namespace DotNet.Domain.Repositorios;

public interface IAnimalRepositorio
{
    Task<Animal?> ObterPorIdAsync(int id);

    Task<Animal?> ObterPorRgaAsync(string rga);

    Task<bool> ExisteAsync(int id);

    Task<bool> RgaExisteAsync(string rga, int? idParaIgnorar = null);

    Task<ResultadoPaginado<Animal>> ListarPaginadoAsync(FiltroAnimal filtro, ParametrosPaginacao paginacao);
    Task AdicionarAsync(Animal animal);

    Task RemoverAsync(Animal animal);

    Task SalvarAsync();
}