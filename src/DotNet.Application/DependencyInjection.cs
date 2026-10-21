using DotNet.Application.Servicos;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IResponsavelServico, ResponsavelServico>();
        services.AddScoped<IAnimalServico, AnimalServico>();
        services.AddScoped<ICareEventServico, CareEventServico>();

        return services;
    }
}
