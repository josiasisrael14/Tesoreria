using System.Collections;
using System.Reflection;

namespace Tesoreria.Application.Abstracciones.Mediator;

/// <summary>
/// Implementación mínima y sin dependencias externas del patrón Mediator
/// (a propósito, en vez de usar el paquete MediatR: desde 2025 requiere licencia
/// comercial para uso de empresa; esta clase da el mismo comportamiento —
/// un Send() que ubica el Handler correcto y lo envuelve en los Behaviors
/// registrados — sin depender de ningún paquete de terceros).
///
/// Solo usa System.IServiceProvider (parte del runtime base), así que este
/// proyecto no necesita ningún PackageReference para compilar.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"No se registró ningún handler para '{requestType.Name}'. " +
                $"Verifica que exista una clase que implemente IRequestHandler<{requestType.Name}, {typeof(TResponse).Name}> " +
                "y que AddApplication() la haya registrado.");

        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;

        Func<Task<TResponse>> pipeline = () =>
            (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = ResolverBehaviors(behaviorType);

        // Se envuelven en orden inverso para que el primero registrado sea el más "externo".
        // Ojo: Enumerable.Reverse() y no List<T>.Reverse() (ese último es in-place y devuelve void).
        foreach (var behavior in Enumerable.Reverse(behaviors))
        {
            var siguiente = pipeline;
            var behaviorHandleMethod = behaviorType.GetMethod(
                nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;

            pipeline = () => (Task<TResponse>)behaviorHandleMethod.Invoke(
                behavior, new object[] { request, siguiente, cancellationToken })!;
        }

        return await pipeline();
    }

    private List<object> ResolverBehaviors(Type behaviorType)
    {
        var enumerableType = typeof(IEnumerable<>).MakeGenericType(behaviorType);
        var servicio = _serviceProvider.GetService(enumerableType);

        if (servicio is IEnumerable enumerable)
        {
            var lista = new List<object>();
            foreach (var item in enumerable)
                lista.Add(item);
            return lista;
        }

        return new List<object>();
    }
}
