using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Dtos;

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

    public async Task<IEnumerable<CareEventResponse>> ListarAsync()
    {
        return (await _careEventRepositorio.ObterTodosAsync()).Select(CareEventResponse.De).ToList();
    }

    public async Task<CareEventResponse> ObterPorIdAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id);

        return CareEventResponse.De(careEvent ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado."));
    }

    public async Task<IEnumerable<CareEventResponse>> ListarPorAnimalIdAsync(int animalId)
    {
        if (!await _animalRepositorio.ExisteAsync(animalId))
        {
            throw new RecursoNaoEncontradoException($"Animal com Id {animalId} não foi encontrado.");
        }

        return (await _careEventRepositorio.ObterPorAnimalIdAsync(animalId)).Select(CareEventResponse.De).ToList();
    }

    public async Task<IEnumerable<CareEventResponse>> ListarPorStatusAsync(string status)
    {
        var normalizedStatus = status.Trim().ToUpper();

        if (!CareEvent.AllowedStatuses.Contains(normalizedStatus))
        {
            throw new RegraDeNegocioException("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        return (await _careEventRepositorio.ObterPorStatusAsync(normalizedStatus)).Select(CareEventResponse.De).ToList();
    }

    public async Task<IEnumerable<CareEventResponse>> ListarPorTypeAsync(string type)
    {
        var normalizedType = type.Trim().ToUpper();

        if (!CareEvent.AllowedTypes.Contains(normalizedType))
        {
            throw new RegraDeNegocioException("Invalid care event type.");
        }

        return (await _careEventRepositorio.ObterPorTypeAsync(normalizedType)).Select(CareEventResponse.De).ToList();
    }

    public async Task<IEnumerable<CareEventResponse>> ListarPorAnimalIdEStatusAsync(int animalId, string status)
    {
        if (!await _animalRepositorio.ExisteAsync(animalId))
        {
            throw new RecursoNaoEncontradoException($"Animal com Id {animalId} não foi encontrado.");
        }

        var normalizedStatus = status.Trim().ToUpper();

        if (!CareEvent.AllowedStatuses.Contains(normalizedStatus))
        {
            throw new RegraDeNegocioException("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        return (await _careEventRepositorio.ObterPorAnimalIdEStatusAsync(animalId, normalizedStatus)).Select(CareEventResponse.De).ToList();
    }

    public async Task<IEnumerable<CareEventResponse>> ListarAtrasadosAsync()
    {
        return (await _careEventRepositorio.ObterAtrasadosAsync()).Select(CareEventResponse.De).ToList();
    }

    public async Task<CareEventResponse> CriarAsync(CriarCareEventRequest request)
    {
        var (petId, type, title, scheduledDate, status, priority, description, completedDate, notes) = request;

        if (!await _animalRepositorio.ExisteAsync(petId))
        {
            throw new RegraDeNegocioException("AnimalId does not exist.");
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
            throw new RegraDeNegocioException("AnimalId does not exist.");
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