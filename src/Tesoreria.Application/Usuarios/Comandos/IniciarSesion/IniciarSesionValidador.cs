using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.IniciarSesion;

public class IniciarSesionValidador : IValidador<IniciarSesionCommand>
{
    public ResultadoValidacion Validar(IniciarSesionCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Email))
            resultado.Agregar(nameof(request.Email), "El correo es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Contrasena))
            resultado.Agregar(nameof(request.Contrasena), "La contraseña es obligatoria.");

        return resultado;
    }
}
