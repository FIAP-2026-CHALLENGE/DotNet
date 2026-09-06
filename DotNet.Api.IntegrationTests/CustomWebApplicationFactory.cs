using DotNet.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Api.IntegrationTests;

/// <summary>
/// Fábrica de WebApplicationFactory que substitui completamente o AppDbContext configurado
/// para Oracle por um provider EF Core InMemory isolado por execução. Remove TODAS as
/// entradas de configuração relacionadas ao AppDbContext (o EF Core registra mais de uma
/// internamente), garantindo que nenhuma configuração do Oracle "vaze" e se combine com a
/// configuração InMemory.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descritoresParaRemover = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("AppDbContext")))
                .ToList();

            foreach (var descritor in descritoresParaRemover)
            {
                services.Remove(descritor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}