using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Repositorios;

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