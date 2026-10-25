using DotNet.Application.Servicos;
using DotNet.Domain.Repositorios;
using DotNet.Infrastructure.Autenticacao;
using DotNet.Infrastructure.Data;
using DotNet.Infrastructure.Health;
using DotNet.Infrastructure.Mongo;
using DotNet.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

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

        RelatoTutorMapeamento.Registrar();

        services.AddSingleton<IMongoClient>(_ =>
        {
            var configuracoes = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoConnection"));
            configuracoes.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            return new MongoClient(configuracoes);
        });

        services.AddSingleton(provedor => provedor.GetRequiredService<IMongoClient>()
            .GetDatabase(configuration["Mongo:Database"] ?? "clyvo_vet"));

        services.AddScoped<IRelatoTutorRepositorio, RelatoTutorRepositorio>();

        services
            .AddHealthChecks()
            .AddCheck<OracleHealthCheck>("oracle_database");

        services.Configure<JwtOpcoes>(configuration.GetSection(JwtOpcoes.Secao));
        services.Configure<CredenciaisAdminOpcoes>(configuration.GetSection(CredenciaisAdminOpcoes.Secao));
        services.AddScoped<IAutenticacaoServico, AutenticacaoServico>();

        services.AddAutenticacaoJwt(configuration);

        return services;
    }
}