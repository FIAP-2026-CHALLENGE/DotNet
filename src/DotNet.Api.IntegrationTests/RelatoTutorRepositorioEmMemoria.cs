using System.Collections.Concurrent;
using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;

namespace DotNet.Api.IntegrationTests;

/// <summary>
/// Repositório falso, em memória, que substitui o MongoDB nos testes de integração.
/// Cumpre o mesmo contrato (IRelatoTutorRepositorio), então a API não percebe a troca
/// e os testes rodam sem nenhum banco externo (inclusive no pipeline de CI).
/// </summary>
public class RelatoTutorRepositorioEmMemoria : IRelatoTutorRepositorio
{
    private readonly ConcurrentDictionary<string, RelatoTutor> _relatos = new();

    public Task<RelatoTutor?> ObterPorIdAsync(string id)
    {
        return Task.FromResult(_relatos.GetValueOrDefault(id));
    }

    public Task<ResultadoPaginado<RelatoTutor>> ListarPaginadoAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao)
    {
        var consulta = _relatos.Values.AsEnumerable();

        if (filtro.AnimalId is int animalId)
        {
            consulta = consulta.Where(r => r.AnimalId == animalId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Sintoma))
        {
            var sintoma = filtro.Sintoma.Trim().ToLowerInvariant();
            consulta = consulta.Where(r => r.Sintomas.Contains(sintoma));
        }

        var filtrados = consulta.OrderBy(r => r.Id, StringComparer.Ordinal).ToList();

        var itens = filtrados
            .Skip(paginacao.Pular())
            .Take(paginacao.TamanhoSeguro())
            .ToList();

        return Task.FromResult(new ResultadoPaginado<RelatoTutor>(
            itens, paginacao.PaginaSegura(), paginacao.TamanhoSeguro(), filtrados.Count));
    }

    public Task AdicionarAsync(RelatoTutor relato)
    {
        relato.Id = Guid.NewGuid().ToString("N")[..24];
        _relatos[relato.Id] = relato;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(RelatoTutor relato)
    {
        _relatos[relato.Id] = relato;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(RelatoTutor relato)
    {
        _relatos.TryRemove(relato.Id, out _);
        return Task.CompletedTask;
    }
}
