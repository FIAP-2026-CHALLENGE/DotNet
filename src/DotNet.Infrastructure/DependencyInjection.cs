using DotNet.Domain.Repositorios;
using DotNet.Infrastructure.Data;
using DotNet.Infrastructure.Health;
using DotNet.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
                services.AddDbContext<AppDbContext>(options =>
        {
            options.UseOracle(configuration.GetConnectionString("OracleConnection"));
        });

        services.AddScoped<IResponsavelRepositorio, ResponsavelRepositorio>();
        services.AddScoped<IAnimalRepositorio, AnimalRepositorio>();
        services.AddScoped<ICareEventRepositorio, CareEventRepositorio>();

        services
            .AddHealthChecks()
            .AddCheck<OracleHealthCheck>("oracle_database");

        return services;
    }
}