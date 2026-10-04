using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Cultos.Comandos.CrearCulto;

public class CrearCultoValidador : IValidador<CrearCultoCommand>
{
    public ResultadoValidacion Validar(CrearCultoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.Fecha == default)
            resultado.Agregar(nameof(request.Fecha), "La fecha del culto es obligatoria.");

        if (request.Fecha.Date > DateTime.UtcNow.Date.AddDays(1))
            resultado.Agregar(nameof(request.Fecha), "La fecha del culto no puede ser en el futuro.");

        return resultado;
    }
}
