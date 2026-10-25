using DotNet.Infrastructure.Data;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using Microsoft.EntityFrameworkCore;
using DotNet.Domain.Consultas;


namespace DotNet.Infrastructure.Repositorios;

public class ResponsavelRepositorio : IResponsavelRepositorio
{
    private readonly AppDbContext _context;

    public ResponsavelRepositorio(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginado<Responsavel>> ListarPaginadoAsync(FiltroResponsavel filtro, ParametrosPaginacao paginacao)
    {
        var consulta = _context.Responsaveis.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLower();
            consulta = consulta.Where(r => r.Name.ToLower().Contains(nome));
        }

        if (filtro.Ativo is bool ativo)
        {
            consulta = consulta.Where(r => r.IsActive == ativo);
        }

        var total = await consulta.CountAsync();

        var itens = await Ordenar(consulta, paginacao)
            .Skip(paginacao.Pular())
            .Take(paginacao.TamanhoSeguro())
            .ToListAsync();

        return new ResultadoPaginado<Responsavel>(itens, paginacao.PaginaSegura(), paginacao.TamanhoSeguro(), total);
    }

    private static IQueryable<Responsavel> Ordenar(IQueryable<Responsavel> consulta, ParametrosPaginacao paginacao)
    {
        return (paginacao.OrdenarPor?.Trim().ToLowerInvariant(), paginacao.Decrescente) switch
        {
            ("name", false) => consulta.OrderBy(r => r.Name).ThenBy(r => r.Id),
            ("name", true) => consulta.OrderByDescending(r => r.Name).ThenBy(r => r.Id),
            ("email", false) => consulta.OrderBy(r => r.Email).ThenBy(r => r.Id),
            ("email", true) => consulta.OrderByDescending(r => r.Email).ThenBy(r => r.Id),
            ("createdat", false) => consulta.OrderBy(r => r.CreatedAt).ThenBy(r => r.Id),
            ("createdat", true) => consulta.OrderByDescending(r => r.CreatedAt).ThenBy(r => r.Id),
            (_, true) => consulta.OrderByDescending(r => r.Id),
            _ => consulta.OrderBy(r => r.Id)
        };
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