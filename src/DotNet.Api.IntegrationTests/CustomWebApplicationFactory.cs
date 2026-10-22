using DotNet.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;

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


    public CustomWebApplicationFactory()
    {
    Environment.SetEnvironmentVariable("Jwt__Key", "chave-de-teste-com-mais-de-trinta-e-dois-caracteres");
    Environment.SetEnvironmentVariable("Auth__Usuario", "admin-teste");
    Environment.SetEnvironmentVariable("Auth__Senha", "senha-teste");
    }

    public HttpClient CriarClienteAutenticado()
    {
        var client = CreateClient();

        using var scope = Services.CreateScope();
        var autenticacao = scope.ServiceProvider.GetRequiredService<IAutenticacaoServico>();
        var token = autenticacao.Autenticar(new LoginRequest("admin-teste", "senha-teste"));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        return client;
    }

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