using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasenaConToken;

public class RestablecerContrasenaConTokenValidador : IValidador<RestablecerContrasenaConTokenCommand>
{
    public ResultadoValidacion Validar(RestablecerContrasenaConTokenCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            resultado.Agregar(nameof(request.Email), "El correo no es válido.");

        if (string.IsNullOrWhiteSpace(request.Token))
            resultado.Agregar(nameof(request.Token), "El enlace no es válido.");

        if (string.IsNullOrWhiteSpace(request.NuevaContrasena) || request.NuevaContrasena.Length < 6)
            resultado.Agregar(nameof(request.NuevaContrasena), "La contraseña nueva debe tener al menos 6 caracteres.");

        return resultado;
    }
}
