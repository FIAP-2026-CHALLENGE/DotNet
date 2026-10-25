using System.Linq.Expressions;
using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DotNet.Infrastructure.Repositorios;

public class RelatoTutorRepositorio : IRelatoTutorRepositorio
{
    private readonly IMongoCollection<RelatoTutor> _colecao;

    public RelatoTutorRepositorio(IMongoDatabase database)
    {
        _colecao = database.GetCollection<RelatoTutor>("relatos_tutor");
    }

    public async Task<RelatoTutor?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _colecao.Find(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task<ResultadoPaginado<RelatoTutor>> ListarPaginadoAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao)
    {
        var construtor = Builders<RelatoTutor>.Filter;
        var filtroMongo = construtor.Empty;

        if (filtro.AnimalId is int animalId)
        {
            filtroMongo &= construtor.Eq(r => r.AnimalId, animalId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Sintoma))
        {
            filtroMongo &= construtor.AnyEq(r => r.Sintomas, filtro.Sintoma.Trim().ToLowerInvariant());
        }

        var total = await _colecao.CountDocumentsAsync(filtroMongo);

        var itens = await _colecao.Find(filtroMongo)
            .Sort(Ordenar(paginacao))
            .Skip(paginacao.Pular())
            .Limit(paginacao.TamanhoSeguro())
            .ToListAsync();

        return new ResultadoPaginado<RelatoTutor>(itens, paginacao.PaginaSegura(), paginacao.TamanhoSeguro(), (int)total);
    }

    private static SortDefinition<RelatoTutor> Ordenar(ParametrosPaginacao paginacao)
    {
        var ordem = Builders<RelatoTutor>.Sort;

        Expression<Func<RelatoTutor, object>>? campo = paginacao.OrdenarPor?.Trim().ToLowerInvariant() switch
        {
            "dataocorrencia" => r => r.DataOcorrencia,
            "criadoem" => r => r.CriadoEm,
            _ => null
        };

        if (campo is null)
        {
            return paginacao.Decrescente ? ordem.Descending(r => r.Id) : ordem.Ascending(r => r.Id);
        }

        var principal = paginacao.Decrescente ? ordem.Descending(campo) : ordem.Ascending(campo);

        return ordem.Combine(principal, ordem.Ascending(r => r.Id));
    }

    public async Task AdicionarAsync(RelatoTutor relato)
    {
        await _colecao.InsertOneAsync(relato);
    }

    public async Task AtualizarAsync(RelatoTutor relato)
    {
        await _colecao.ReplaceOneAsync(r => r.Id == relato.Id, relato);
    }

    public async Task RemoverAsync(RelatoTutor relato)
    {
        await _colecao.DeleteOneAsync(r => r.Id == relato.Id);
    }
}