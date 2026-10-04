namespace Tesoreria.Application.Abstracciones.Validacion;

public record ErrorValidacion(string Campo, string Mensaje);

public class ResultadoValidacion
{
    public List<ErrorValidacion> Errores { get; } = new();
    public bool EsValido => Errores.Count == 0;

    public void Agregar(string campo, string mensaje) => Errores.Add(new ErrorValidacion(campo, mensaje));

    public static ResultadoValidacion Ok() => new();
}

/// <summary>
/// Valida un Command/Query antes de que llegue a su Handler. Versión propia,
/// liviana, en vez de FluentValidation, para no sumar una dependencia externa
/// más a la que atarse — el mismo objetivo (nada inválido llega a tocar la
/// base de datos) con una interfaz de una sola línea.
/// </summary>
public interface IValidador<in T>
{
    ResultadoValidacion Validar(T request);
}
