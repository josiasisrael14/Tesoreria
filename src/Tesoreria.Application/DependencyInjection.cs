using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el Mediator, todos los Handlers (Commands/Queries) y todos los
    /// Validadores de este ensamblado por reflexión — no hay que acordarse de
    /// agregar cada uno a mano cuando se crea un caso de uso nuevo.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<IMediator, Mediator>();

        RegistrarImplementacionesDe(services, assembly, typeof(IRequestHandler<,>));
        RegistrarImplementacionesDe(services, assembly, typeof(IValidador<>));

        // El behavior de validación corre antes que cualquier Handler.
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static void RegistrarImplementacionesDe(IServiceCollection services, Assembly assembly, Type interfazAbierta)
    {
        var tipos = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfazAbierta)
                .Select(i => new { Implementacion = t, Interfaz = i }));

        foreach (var tipo in tipos)
            services.AddScoped(tipo.Interfaz, tipo.Implementacion);
    }
}
