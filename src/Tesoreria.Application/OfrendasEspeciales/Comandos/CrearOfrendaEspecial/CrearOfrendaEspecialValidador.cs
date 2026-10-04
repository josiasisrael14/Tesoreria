using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.OfrendasEspeciales.Comandos.CrearOfrendaEspecial;

public class CrearOfrendaEspecialValidador : IValidador<CrearOfrendaEspecialCommand>
{
    public ResultadoValidacion Validar(CrearOfrendaEspecialCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Nombre))
            resultado.Agregar(nameof(request.Nombre), "El nombre de la campaña es obligatorio.");
        else if (request.Nombre.Length > 150)
            resultado.Agregar(nameof(request.Nombre), "El nombre de la campaña no puede superar los 150 caracteres.");

        if (request.Anio < 2000 || request.Anio > 2100)
            resultado.Agregar(nameof(request.Anio), "El año de la campaña no es válido.");

        if (request.MetaMonto is <= 0)
            resultado.Agregar(nameof(request.MetaMonto), "La meta, si se define, debe ser mayor a cero.");

        if (request.FechaInicio.HasValue && request.FechaFin.HasValue && request.FechaFin < request.FechaInicio)
            resultado.Agregar(nameof(request.FechaFin), "La fecha de fin no puede ser anterior a la fecha de inicio.");

        return resultado;
    }
}
