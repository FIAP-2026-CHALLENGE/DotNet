using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DotNet.Infrastructure.Health;

/// <summary>
/// Health check que valida a conectividade com o MongoDB, exposto em /health/mongo.
/// </summary>
public class MongoHealthCheck : IHealthCheck
{
    public const string Tag = "mongo";

    private readonly IMongoDatabase _database;

    public MongoHealthCheck(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy("Conexão com o MongoDB estabelecida com sucesso.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar a conexão com o MongoDB.", ex);
        }
    }
}