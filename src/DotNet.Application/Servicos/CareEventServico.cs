using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;

namespace DotNet.Application.Servicos;

public class CareEventServico : ICareEventServico
{
    private readonly ICareEventRepositorio _careEventRepositorio;
    private readonly IAnimalRepositorio _animalRepositorio;

    public CareEventServico(ICareEventRepositorio careEventRepositorio, IAnimalRepositorio animalRepositorio)
    {
        _careEventRepositorio = careEventRepositorio;
        _animalRepositorio = animalRepositorio;
    }

    public async Task<ResultadoPaginado<CareEventResponse>> ListarAsync(FiltroCareEvent filtro, ParametrosPaginacao paginacao)
    {
        if (filtro.AnimalId is int animalId && !await _animalRepositorio.ExisteAsync(animalId))
        {
            throw new RecursoNaoEncontradoException($"Animal com Id {animalId} não foi encontrado.");
        }

        var status = filtro.Status?.Trim().ToUpper();

        if (!string.IsNullOrEmpty(status) && !CareEvent.AllowedStatuses.Contains(status))
        {
            throw new RegraDeNegocioException("O status deve ser PENDING, COMPLETED, OVERDUE ou CANCELED.");
        }

        var type = filtro.Type?.Trim().ToUpper();

        if (!string.IsNullOrEmpty(type) && !CareEvent.AllowedTypes.Contains(type))
        {
            throw new RegraDeNegocioException("Tipo de evento de cuidado inválido.");
        }

        var resultado = await _careEventRepositorio.ListarPaginadoAsync(
            filtro with { Status = status, Type = type },
            paginacao);

        return resultado.Mapear(CareEventResponse.De);
    }

    public async Task<CareEventResponse> ObterPorIdAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id);

        return CareEventResponse.De(careEvent ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado."));
    }

    public async Task<CareEventResponse> CriarAsync(CriarCareEventRequest request)
    {
        var (petId, type, title, scheduledDate, status, priority, description, completedDate, notes) = request;

        if (!await _animalRepositorio.ExisteAsync(petId))
        {
            throw new RegraDeNegocioException("O animal informado não existe.");
        }

        var careEvent = CareEvent.Criar(petId, type, title, description, scheduledDate, completedDate, status, priority, notes);

        await _careEventRepositorio.AdicionarAsync(careEvent);

        return CareEventResponse.De(careEvent);
    }

    public async Task AtualizarAsync(int id, AtualizarCareEventRequest request)
    {
        var (petId, type, title, scheduledDate, status, priority, description, completedDate, notes, isActive) = request;

        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado.");

        if (!await _animalRepositorio.ExisteAsync(petId))
        {
            throw new RegraDeNegocioException("O animal informado não existe.");
        }

        careEvent.Atualizar(petId, type, title, description, scheduledDate, completedDate, status, priority, notes, isActive);

        await _careEventRepositorio.SalvarAsync();
    }

    public async Task ConcluirAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado.");

        careEvent.Concluir();

        await _careEventRepositorio.SalvarAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado.");

        await _careEventRepositorio.RemoverAsync(careEvent);
    }
}