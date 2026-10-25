using DotNet.Infrastructure.Data;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using Microsoft.EntityFrameworkCore;
using DotNet.Domain.Consultas;

namespace DotNet.Infrastructure.Repositorios;

public class AnimalRepositorio : IAnimalRepositorio
{
    private readonly AppDbContext _context;

    public AnimalRepositorio(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginado<Animal>> ListarPaginadoAsync(FiltroAnimal filtro, ParametrosPaginacao paginacao)
    {
        var consulta = _context.Animais.AsNoTracking().AsQueryable();

        if (filtro.ResponsavelId is int responsavelId)
        {
            consulta = consulta.Where(a => a.ResponsavelId == responsavelId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Species))
        {
            var species = filtro.Species.Trim().ToUpper();
            consulta = consulta.Where(a => a.Species == species);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Breed))
        {
            var breed = filtro.Breed.Trim().ToLower();
            consulta = consulta.Where(a => a.Breed.ToLower() == breed);
        }

        if (filtro.Ativo is bool ativo)
        {
            consulta = consulta.Where(a => a.IsActive == ativo);
        }

        var total = await consulta.CountAsync();

        var itens = await Ordenar(consulta, paginacao)
            .Skip(paginacao.Pular())
            .Take(paginacao.TamanhoSeguro())
            .ToListAsync();

        return new ResultadoPaginado<Animal>(itens, paginacao.PaginaSegura(), paginacao.TamanhoSeguro(), total);
    }

    private static IQueryable<Animal> Ordenar(IQueryable<Animal> consulta, ParametrosPaginacao paginacao)
    {
        return (paginacao.OrdenarPor?.Trim().ToLowerInvariant(), paginacao.Decrescente) switch
        {
            ("name", false) => consulta.OrderBy(a => a.Name).ThenBy(a => a.Id),
            ("name", true) => consulta.OrderByDescending(a => a.Name).ThenBy(a => a.Id),
            ("species", false) => consulta.OrderBy(a => a.Species).ThenBy(a => a.Id),
            ("species", true) => consulta.OrderByDescending(a => a.Species).ThenBy(a => a.Id),
            ("birthdate", false) => consulta.OrderBy(a => a.BirthDate).ThenBy(a => a.Id),
            ("birthdate", true) => consulta.OrderByDescending(a => a.BirthDate).ThenBy(a => a.Id),
            ("createdat", false) => consulta.OrderBy(a => a.CreatedAt).ThenBy(a => a.Id),
            ("createdat", true) => consulta.OrderByDescending(a => a.CreatedAt).ThenBy(a => a.Id),
            (_, true) => consulta.OrderByDescending(a => a.Id),
            _ => consulta.OrderBy(a => a.Id)
        };
    }

    public async Task<Animal?> ObterPorIdAsync(int id)
    {
        return await _context.Animais.FirstOrDefaultAsync(a => a.Id == id);
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