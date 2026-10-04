namespace Tesoreria.Application.Abstracciones.Mediator;

/// <summary>
/// Envuelve la ejecución de un Handler para agregar comportamiento transversal
/// (validación, logging, una transacción de base de datos) sin repetirlo en cada Handler.
/// Se registran varios y se encadenan uno dentro de otro, en el orden en que se registran.
/// </summary>
public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> siguiente, CancellationToken cancellationToken);
}
