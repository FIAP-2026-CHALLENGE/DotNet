using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public class AnimalServico : IAnimalServico
{
    private readonly IAnimalRepositorio _animalRepositorio;
    private readonly IResponsavelRepositorio _responsavelRepositorio;

    public AnimalServico(IAnimalRepositorio animalRepositorio, IResponsavelRepositorio responsavelRepositorio)
    {
        _animalRepositorio = animalRepositorio;
        _responsavelRepositorio = responsavelRepositorio;
    }

    public async Task<ResultadoPaginado<AnimalResponse>> ListarAsync(FiltroAnimal filtro, ParametrosPaginacao paginacao)
    {
        if (filtro.ResponsavelId is int responsavelId && !await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RecursoNaoEncontradoException($"Responsavel com Id {responsavelId} não foi encontrado.");
        }

        var resultado = await _animalRepositorio.ListarPaginadoAsync(filtro, paginacao);

        return resultado.Mapear(AnimalResponse.De);
    }

    public async Task<AnimalResponse> ObterPorIdAsync(int id)
    {
        var animal = await _animalRepositorio.ObterPorIdAsync(id);

        return AnimalResponse.De(animal ?? throw new RecursoNaoEncontradoException($"Animal com Id {id} não foi encontrado."));
    }

    public async Task<AnimalResponse> ObterPorRgaAsync(string rga)
    {
        var animal = await _animalRepositorio.ObterPorRgaAsync(rga);

        return AnimalResponse.De(animal ?? throw new RecursoNaoEncontradoException($"Animal com RGA {rga} não foi encontrado."));
    }

    public async Task<AnimalResponse> CriarAsync(CriarAnimalRequest request)
    {
        var (responsavelId, name, species, breed, birthDate, weight, sex, nickname, rga) = request;

        if (!await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RegraDeNegocioException("O responsável informado não existe.");
        }

        if (await _animalRepositorio.RgaExisteAsync(rga))
        {
            throw new RegraDeNegocioException("RGA já cadastrado.");
        }

        var animal = Animal.Criar(responsavelId, name, nickname, species, breed, birthDate, weight, sex, rga);

        await _animalRepositorio.AdicionarAsync(animal);

        return AnimalResponse.De(animal);
    }

    public async Task AtualizarAsync(int id, AtualizarAnimalRequest request)
    {
        var (responsavelId, name, species, breed, birthDate, weight, sex, nickname, rga, isActive) = request;

        var animal = await _animalRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Animal com Id {id} não foi encontrado.");

        if (!await _responsavelRepositorio.ExisteAsync(responsavelId))
        {
            throw new RegraDeNegocioException("O responsável informado não existe.");
        }

        if (await _animalRepositorio.RgaExisteAsync(rga, id))
        {
            throw new RegraDeNegocioException("RGA já cadastrado em outro animal.");
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