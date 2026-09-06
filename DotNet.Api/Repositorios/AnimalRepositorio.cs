using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Repositorios;

public class AnimalRepositorio : IAnimalRepositorio
{
    private readonly AppDbContext _context;

    public AnimalRepositorio(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Animal>> ObterTodosAsync()
    {
        return await _context.Animais.AsNoTracking().ToListAsync();
    }

    public async Task<Animal?> ObterPorIdAsync(int id)
    {
        return await _context.Animais.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Animal>> ObterPorResponsavelIdAsync(int responsavelId)
    {
        return await _context.Animais
            .AsNoTracking()
            .Where(a => a.ResponsavelId == responsavelId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Animal>> ObterPorSpeciesAsync(string species)
    {
        return await _context.Animais
            .AsNoTracking()
            .Where(a => a.Species == species)
            .ToListAsync();
    }

    public async Task<IEnumerable<Animal>> ObterPorBreedAsync(string breed)
    {
        var normalizedBreed = breed.ToLower();

        return await _context.Animais
            .AsNoTracking()
            .Where(a => a.Breed.ToLower() == normalizedBreed)
            .ToListAsync();
    }

    public async Task<Animal?> ObterPorRgaAsync(string rga)
    {
        return await _context.Animais.AsNoTracking().FirstOrDefaultAsync(a => a.Rga == rga);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Animais.CountAsync(a => a.Id == id) > 0;
    }

    public async Task<bool> RgaExisteAsync(string rga, int? idParaIgnorar = null)
    {
        if (string.IsNullOrWhiteSpace(rga))
        {
            return false;
        }

        return await _context.Animais
            .CountAsync(a => a.Rga == rga && (idParaIgnorar == null || a.Id != idParaIgnorar)) > 0;
    }

    public async Task AdicionarAsync(Animal animal)
    {
        await _context.Animais.AddAsync(animal);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Animal animal)
    {
        _context.Animais.Remove(animal);
        await _context.SaveChangesAsync();
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }
}