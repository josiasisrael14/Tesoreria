using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasena;

public class RestablecerContrasenaValidador : IValidador<RestablecerContrasenaCommand>
{
    public ResultadoValidacion Validar(RestablecerContrasenaCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.NuevaContrasena) || request.NuevaContrasena.Length < 6)
            resultado.Agregar(nameof(request.NuevaContrasena), "La contraseña debe tener al menos 6 caracteres.");

        return resultado;
    }
}
