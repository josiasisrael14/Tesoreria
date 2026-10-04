namespace Tesoreria.Application.Abstracciones.Mediator;

/// <summary>
/// Marca un Command o Query que espera una respuesta de tipo TResponse.
/// Es la pieza base del patrón Mediator: separa "qué se quiere hacer" (el request)
/// de "quién lo resuelve" (el handler).
/// </summary>
public interface IRequest<TResponse> { }
