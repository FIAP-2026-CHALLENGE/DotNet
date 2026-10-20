using DotNet.Infrastructure.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNet.Api.Infraestrutura.Health;

/// <summary>
/// Health check customizado que valida a conectividade real com o banco Oracle da FIAP,
/// usado pelo endpoint nativo /health.
/// </summary>
public class OracleHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public OracleHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var podeConectar = await _context.Database.CanConnectAsync(cancellationToken);

            return podeConectar
                ? HealthCheckResult.Healthy("Conexão com o Oracle Database (FIAP) estabelecida com sucesso.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao Oracle Database.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar a conexão com o Oracle Database.", ex);
        }
    }
}