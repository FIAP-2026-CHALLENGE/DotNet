using DotNet.Api.Excecoes;
using DotNet.Api.Models;
using DotNet.Api.Repositorios;

namespace DotNet.Api.Servicos;

public class AnimalServico : IAnimalServico
{
    private readonly IAnimalRepositorio _animalRepositorio;
    private readonly IResponsavelRepositorio _responsavelRepositorio;

    public AnimalServico(IAnimalRepositorio animalRepositorio, IResponsavelRepositorio responsavelRepositorio)
    {
        _animalRepositorio = animalRepositorio;
        _responsavelRepositorio = responsavelRepositorio;
    }

    public async Task<IEnumerable<Animal>> ListarAsync()
    {
        return await _animalRepositorio.ObterTodosAsync();
    }

    public async Task<Animal> ObterPorIdAsync(int id)
    {
        var animal = await _animalRepositorio.ObterPorIdAsync(id);

        return animal ?? throw new RecursoNaoEncontradoException($"Animal com Id {id} não foi encontrado.");
    }

    public async Task<IEnumerable<Animal>> ListarPorResponsavelIdAsync(int responsavelId)
    {
        if (!await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RecursoNaoEncontradoException($"Responsavel com Id {responsavelId} não foi encontrado.");
        }

        return await _animalRepositorio.ObterPorResponsavelIdAsync(responsavelId);
    }

    public async Task<IEnumerable<Animal>> ListarPorSpeciesAsync(string species)
    {
        return await _animalRepositorio.ObterPorSpeciesAsync(species.Trim().ToUpper());
    }

    public async Task<IEnumerable<Animal>> ListarPorBreedAsync(string breed)
    {
        return await _animalRepositorio.ObterPorBreedAsync(breed);
    }

    public async Task<Animal> ObterPorRgaAsync(string rga)
    {
        var animal = await _animalRepositorio.ObterPorRgaAsync(rga);

        return animal ?? throw new RecursoNaoEncontradoException($"Animal com RGA {rga} não foi encontrado.");
    }

    public async Task<Animal> CriarAsync(
        int responsavelId, string name, string nickname, string species,
        string breed, DateTime birthDate, decimal weight, string sex, string rga)
    {
        if (!await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RegraDeNegocioException("ResponsavelId does not exist.");
        }

        if (await _animalRepositorio.RgaExisteAsync(rga))
        {
            throw new RegraDeNegocioException("RGA already registered.");
        }

        var animal = Animal.Criar(responsavelId, name, nickname, species, breed, birthDate, weight, sex, rga);

        await _animalRepositorio.AdicionarAsync(animal);

        return animal;
    }

    public async Task AtualizarAsync(
        int id, int responsavelId, string name, string nickname, string species,
        string breed, DateTime birthDate, decimal weight, string sex, string rga, bool isActive)
    {
        var animal = await _animalRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Animal com Id {id} não foi encontrado.");

        if (!await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RegraDeNegocioException("ResponsavelId does not exist.");
        }

        if (await _animalRepositorio.RgaExisteAsync(rga, id))
        {
            throw new RegraDeNegocioException("RGA already registered by another animal.");
        }

        animal.Atualizar(responsavelId, name, nickname, species, breed, birthDate, weight, sex, rga, isActive);

        await _animalRepositorio.SalvarAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var animal = await _animalRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Animal com Id {id} não foi encontrado.");

        await _animalRepositorio.RemoverAsync(animal);
    }
}