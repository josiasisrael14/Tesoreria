using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Transacciones.Comandos.EditarTransaccion;

public class EditarTransaccionValidador : IValidador<EditarTransaccionCommand>
{
    public ResultadoValidacion Validar(EditarTransaccionCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.Monto <= 0)
            resultado.Agregar(nameof(request.Monto), "El monto debe ser mayor a cero.");

        if (request.FondoId <= 0)
            resultado.Agregar(nameof(request.FondoId), "Debe indicar a qué fondo pertenece la transacción.");

        if (request.Fecha == default)
            resultado.Agregar(nameof(request.Fecha), "La fecha es obligatoria.");

        if (request.Fecha.Date > DateTime.UtcNow.Date.AddDays(1))
            resultado.Agregar(nameof(request.Fecha), "La fecha no puede ser en el futuro.");

        if (request.OfrendaEspecialId is <= 0)
            resultado.Agregar(nameof(request.OfrendaEspecialId), "La campaña de ofrenda especial indicada no es válida.");

        return resultado;
    }
}
