using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.CambiarContrasena;

public class CambiarContrasenaValidador : IValidador<CambiarContrasenaCommand>
{
    public ResultadoValidacion Validar(CambiarContrasenaCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.ContrasenaActual))
            resultado.Agregar(nameof(request.ContrasenaActual), "Debes escribir tu contraseña actual.");

        if (string.IsNullOrWhiteSpace(request.ContrasenaNueva) || request.ContrasenaNueva.Length < 6)
            resultado.Agregar(nameof(request.ContrasenaNueva), "La contraseña nueva debe tener al menos 6 caracteres.");

        return resultado;
    }
}
