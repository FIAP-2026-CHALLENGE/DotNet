using DotNet.Api.Aplicacao.Middlewares;
using DotNet.Api.Data;
using DotNet.Api.Infraestrutura.Health;
using DotNet.Api.Infraestrutura.Observabilidade;
using DotNet.Api.Repositorios;
using DotNet.Api.Servicos;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using DotNet.Domain.Repositorios;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}"));

    
    builder.Services.AddControllers();

   
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
    });

    
    builder.Services.AddScoped<IResponsavelRepositorio, ResponsavelRepositorio>();
    builder.Services.AddScoped<IAnimalRepositorio, AnimalRepositorio>();
    builder.Services.AddScoped<ICareEventRepositorio, CareEventRepositorio>();

    builder.Services.AddScoped<IResponsavelServico, ResponsavelServico>();
    builder.Services.AddScoped<IAnimalServico, AnimalServico>();
    builder.Services.AddScoped<ICareEventServico, CareEventServico>();

    
    builder.Services
        .AddHealthChecks()
        .AddCheck<OracleHealthCheck>("oracle_database");

    
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(AplicacaoMetricas.NomeServico))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(AplicacaoMetricas.NomeServico)
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter(AplicacaoMetricas.NomeServico)
            .AddConsoleExporter());

    var app = builder.Build();

    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging();

    app.UseAuthorization();

    app.MapControllers();

    
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação DotNet.Api falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}

// Exposto como classe pública parcial para permitir o uso de WebApplicationFactory<Program>
// nos testes de integração (DotNet.Api.Tests).
public partial class Program
{
}