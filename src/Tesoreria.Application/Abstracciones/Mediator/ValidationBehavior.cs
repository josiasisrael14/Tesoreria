using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Abstracciones.Mediator;

/// <summary>
/// Pipeline behavior que corre ANTES que el Handler: si hay un IValidador registrado
/// para este request y falla, lanza ValidationException y el Handler ni se ejecuta
/// (o sea, un RegistrarIngresoCommand con monto negativo nunca llega a tocar la base de datos).
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidador<TRequest>> _validadores;

    public ValidationBehavior(IEnumerable<IValidador<TRequest>> validadores)
    {
        _validadores = validadores;
    }

    public async Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> siguiente, CancellationToken cancellationToken)
    {
        var errores = _validadores
            .Select(v => v.Validar(request))
            .SelectMany(r => r.Errores)
            .ToList();

        if (errores.Count > 0)
            throw new ValidationException(errores);

        return await siguiente();
    }
}
