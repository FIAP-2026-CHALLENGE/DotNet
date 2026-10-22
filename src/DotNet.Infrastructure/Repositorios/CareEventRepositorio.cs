using DotNet.Infrastructure.Data;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using Microsoft.EntityFrameworkCore;
using DotNet.Domain.Consultas;


namespace DotNet.Infrastructure.Repositorios;

public class CareEventRepositorio : ICareEventRepositorio
{
    private readonly AppDbContext _context;

    public CareEventRepositorio(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CareEvent>> ObterTodosAsync()
    {
        return await _context.CareEvents.AsNoTracking().ToListAsync();
    }

    public async Task<ResultadoPaginado<CareEvent>> ListarPaginadoAsync(FiltroCareEvent filtro, ParametrosPaginacao paginacao)
    {
        var consulta = _context.CareEvents.AsNoTracking().AsQueryable();

        if (filtro.AnimalId is int animalId)
        {
            consulta = consulta.Where(e => e.PetId == animalId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Status))
        {
            var status = filtro.Status;
            consulta = consulta.Where(e => e.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Type))
        {
            var type = filtro.Type;
            consulta = consulta.Where(e => e.Type == type);
        }

        if (filtro.Atrasados == true)
        {
            var hoje = DateTime.UtcNow.Date;

            consulta = consulta.Where(e =>
                e.Status == "OVERDUE" ||
                (e.ScheduledDate.Date < hoje &&
                e.Status != "COMPLETED" &&
                e.Status != "CANCELED"));
        }

        var total = await consulta.CountAsync();

        var itens = await Ordenar(consulta, paginacao)
            .Skip(paginacao.Pular())
            .Take(paginacao.TamanhoSeguro())
            .ToListAsync();

        return new ResultadoPaginado<CareEvent>(itens, paginacao.PaginaSegura(), paginacao.TamanhoSeguro(), total);
    }

    private static IQueryable<CareEvent> Ordenar(IQueryable<CareEvent> consulta, ParametrosPaginacao paginacao)
    {
        return (paginacao.OrdenarPor?.Trim().ToLowerInvariant(), paginacao.Decrescente) switch
        {
            ("scheduleddate", false) => consulta.OrderBy(e => e.ScheduledDate).ThenBy(e => e.Id),
            ("scheduleddate", true) => consulta.OrderByDescending(e => e.ScheduledDate).ThenBy(e => e.Id),
            ("status", false) => consulta.OrderBy(e => e.Status).ThenBy(e => e.Id),
            ("status", true) => consulta.OrderByDescending(e => e.Status).ThenBy(e => e.Id),
            ("priority", false) => consulta.OrderBy(e => e.Priority).ThenBy(e => e.Id),
            ("priority", true) => consulta.OrderByDescending(e => e.Priority).ThenBy(e => e.Id),
            ("createdat", false) => consulta.OrderBy(e => e.CreatedAt).ThenBy(e => e.Id),
            ("createdat", true) => consulta.OrderByDescending(e => e.CreatedAt).ThenBy(e => e.Id),
            (_, true) => consulta.OrderByDescending(e => e.Id),
            _ => consulta.OrderBy(e => e.Id)
        };
    }

    public async Task<CareEvent?> ObterPorIdAsync(int id)
    {
        return await _context.CareEvents.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<CareEvent>> ObterPorAnimalIdAsync(int animalId)
    {
        return await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.PetId == animalId)
            .ToListAsync();
    }

    public async Task<IEnumerable<CareEvent>> ObterPorStatusAsync(string status)
    {
        return await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<CareEvent>> ObterPorTypeAsync(string type)
    {
        return await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.Type == type)
            .ToListAsync();
    }

    public async Task<IEnumerable<CareEvent>> ObterPorAnimalIdEStatusAsync(int animalId, string status)
    {
        return await _context.CareEvents
            .AsNoTracking()
            .Where(e => e.PetId == animalId && e.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<CareEvent>> ObterAtrasadosAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _context.CareEvents
            .AsNoTracking()
            .Where(e =>
                e.Status == "OVERDUE" ||
                (e.ScheduledDate.Date < today &&
                 e.Status != "COMPLETED" &&
                 e.Status != "CANCELED"))
            .ToListAsync();
    }

    public async Task AdicionarAsync(CareEvent careEvent)
    {
        await _context.CareEvents.AddAsync(careEvent);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(CareEvent careEvent)
    {
        _context.CareEvents.Remove(careEvent);
        await _context.SaveChangesAsync();
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }
}