using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Fondos.Comandos.CrearFondo;

/// <summary>
/// Solo valida forma (campos obligatorios, longitudes). La regla de negocio
/// "no puede repetirse el nombre" se revisa en el Handler porque necesita
/// consultar la base de datos, y IValidador es intencionalmente síncrono.
/// </summary>
public class CrearFondoValidador : IValidador<CrearFondoCommand>
{
    public ResultadoValidacion Validar(CrearFondoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Nombre))
            resultado.Agregar(nameof(request.Nombre), "El nombre del fondo es obligatorio.");
        else if (request.Nombre.Length > 100)
            resultado.Agregar(nameof(request.Nombre), "El nombre del fondo no puede superar los 100 caracteres.");

        return resultado;
    }
}
