using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Transacciones.Comandos.RegistrarEgreso;

public class RegistrarEgresoValidador : IValidador<RegistrarEgresoCommand>
{
    public ResultadoValidacion Validar(RegistrarEgresoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.Monto <= 0)
            resultado.Agregar(nameof(request.Monto), "El monto debe ser mayor a cero.");

        if (request.FondoId <= 0)
            resultado.Agregar(nameof(request.FondoId), "Debe indicar de qué fondo sale el egreso.");

        if (string.IsNullOrWhiteSpace(request.Concepto))
            resultado.Agregar(nameof(request.Concepto), "Todo egreso debe indicar un concepto (para qué fue el gasto).");

        return resultado;
    }
}
