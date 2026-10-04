namespace Tesoreria.Application.Abstracciones.Validacion;

/// <summary>
/// Lanzada por el ValidationBehavior cuando un Command/Query no pasa su validación.
/// El controlador la traduce a un 400 Bad Request con el detalle de los errores.
/// </summary>
public class ValidationException : Exception
{
    public IReadOnlyCollection<ErrorValidacion> Errores { get; }

    public ValidationException(IEnumerable<ErrorValidacion> errores)
        : base("Uno o más valores no son válidos.")
    {
        Errores = errores.ToList();
    }
}
