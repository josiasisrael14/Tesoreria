using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.RegistrarCompromiso;

public class RegistrarCompromisoValidador : IValidador<RegistrarCompromisoCommand>
{
    public ResultadoValidacion Validar(RegistrarCompromisoCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (request.OfrendaEspecialId <= 0)
            resultado.Agregar(nameof(request.OfrendaEspecialId), "Debe indicar a qué campaña pertenece el compromiso.");

        if (request.MiembroId <= 0)
            resultado.Agregar(nameof(request.MiembroId), "Debe indicar qué miembro se compromete.");

        if (request.MontoComprometido <= 0)
            resultado.Agregar(nameof(request.MontoComprometido), "El monto comprometido debe ser mayor a cero.");

        return resultado;
    }
}
