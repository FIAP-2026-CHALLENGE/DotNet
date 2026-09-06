using DotNet.Api.Data;
using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Repositorios;

public class ResponsavelRepositorio : IResponsavelRepositorio
{
    private readonly AppDbContext _context;

    public ResponsavelRepositorio(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Responsavel>> ObterTodosAsync()
    {
        return await _context.Responsaveis.AsNoTracking().ToListAsync();
    }

    public async Task<Responsavel?> ObterPorIdAsync(int id)
    {
        return await _context.Responsaveis.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Responsavel?> ObterPorCpfAsync(string cpf)
    {
        return await _context.Responsaveis.AsNoTracking().FirstOrDefaultAsync(r => r.Cpf == cpf);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Responsaveis.CountAsync(r => r.Id == id) > 0;
    }

    public async Task<bool> CpfExisteAsync(string cpf, int? idParaIgnorar = null)
    {
        return await _context.Responsaveis
            .CountAsync(r => r.Cpf == cpf && (idParaIgnorar == null || r.Id != idParaIgnorar)) > 0;
    }

    public async Task AdicionarAsync(Responsavel responsavel)
    {
        await _context.Responsaveis.AddAsync(responsavel);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Responsavel responsavel)
    {
        _context.Responsaveis.Remove(responsavel);
        await _context.SaveChangesAsync();
    }

    public async Task SalvarAsync()
    {
        await _context.SaveChangesAsync();
    }
}