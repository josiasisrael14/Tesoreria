using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.ActualizarCompromiso;

public class ActualizarCompromisoValidador : IValidador<ActualizarCompromisoCommand>
{
    public ResultadoValidacion Validar(ActualizarCompromisoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.MontoComprometido <= 0)
            resultado.Agregar(nameof(request.MontoComprometido), "El monto comprometido debe ser mayor a cero.");

        return resultado;
    }
}
