using DotNet.Api.Excecoes;
using DotNet.Api.Models;
using DotNet.Api.Repositorios;

namespace DotNet.Api.Servicos;

public class CareEventServico : ICareEventServico
{
    private readonly ICareEventRepositorio _careEventRepositorio;
    private readonly IAnimalRepositorio _animalRepositorio;

    public CareEventServico(ICareEventRepositorio careEventRepositorio, IAnimalRepositorio animalRepositorio)
    {
        _careEventRepositorio = careEventRepositorio;
        _animalRepositorio = animalRepositorio;
    }

    public async Task<IEnumerable<CareEvent>> ListarAsync()
    {
        return await _careEventRepositorio.ObterTodosAsync();
    }

    public async Task<CareEvent> ObterPorIdAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id);

        return careEvent ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado.");
    }

    public async Task<IEnumerable<CareEvent>> ListarPorAnimalIdAsync(int animalId)
    {
        if (!await _animalRepositorio.ExisteAsync(animalId))
        {
            throw new RecursoNaoEncontradoException($"Animal com Id {animalId} não foi encontrado.");
        }

        return await _careEventRepositorio.ObterPorAnimalIdAsync(animalId);
    }

    public async Task<IEnumerable<CareEvent>> ListarPorStatusAsync(string status)
    {
        var normalizedStatus = status.Trim().ToUpper();

        if (!CareEvent.AllowedStatuses.Contains(normalizedStatus))
        {
            throw new RegraDeNegocioException("Status must be PENDING, COMPLETED, OVERDUE or CANCELED.");
        }

        return await _careEventRepositorio.ObterPorStatusAsync(normalizedStatus);
    }

    public async Task<IEnumerable<CareEvent>> ListarPorTypeAsync(string type)
    {
        var normalizedType = type.Trim().ToUpper();

        if (!CareEvent.AllowedTypes.Contains(normalizedType))
        {
            throw new RegraDeNegocioException("Invalid care event type.");
        }

        return await _careEventRepositorio.ObterPorTypeAsync(normalizedType);
    }

    public async Task<IEnumerable<CareEvent>> ListarPorAnimalIdEStatusAsync(int animalId, string status)
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

        return await _careEventRepositorio.ObterPorAnimalIdEStatusAsync(animalId, normalizedStatus);
    }

    public async Task<IEnumerable<CareEvent>> ListarAtrasadosAsync()
    {
        return await _careEventRepositorio.ObterAtrasadosAsync();
    }

    public async Task<CareEvent> CriarAsync(
        int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes)
    {
        if (!await _animalRepositorio.ExisteAsync(petId))
        {
            throw new RegraDeNegocioException("AnimalId does not exist.");
        }

        var careEvent = CareEvent.Criar(petId, type, title, description, scheduledDate, completedDate, status, priority, notes);

        await _careEventRepositorio.AdicionarAsync(careEvent);

        return careEvent;
    }

    public async Task AtualizarAsync(
        int id, int petId, string type, string title, string? description, DateTime scheduledDate,
        DateTime? completedDate, string status, string priority, string? notes, bool isActive)
    {
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

        try
        {
            careEvent.Concluir();
        }
        catch (InvalidOperationException ex)
        {
            throw new RegraDeNegocioException(ex.Message);
        }

        await _careEventRepositorio.SalvarAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var careEvent = await _careEventRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"CareEvent com Id {id} não foi encontrado.");

        await _careEventRepositorio.RemoverAsync(careEvent);
    }
}