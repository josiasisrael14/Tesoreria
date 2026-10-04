using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.CrearUsuario;

/// <summary>
/// Solo valida forma. Las reglas de contraseña "de verdad" (mayúsculas, dígitos, etc.)
/// las aplica Identity en el Handler, porque están configuradas en un solo lugar
/// (AddIdentityCore, en Tesoreria.Infrastructure) y no queremos que se desincronicen.
/// </summary>
public class CrearUsuarioValidador : IValidador<CrearUsuarioCommand>
{
    public ResultadoValidacion Validar(CrearUsuarioCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            resultado.Agregar(nameof(request.Email), "El correo no es válido.");

        if (string.IsNullOrWhiteSpace(request.NombreCompleto))
            resultado.Agregar(nameof(request.NombreCompleto), "El nombre completo es obligatorio.");
        else if (request.NombreCompleto.Length > 150)
            resultado.Agregar(nameof(request.NombreCompleto), "El nombre completo no puede superar los 150 caracteres.");

        if (string.IsNullOrWhiteSpace(request.Contrasena) || request.Contrasena.Length < 6)
            resultado.Agregar(nameof(request.Contrasena), "La contraseña debe tener al menos 6 caracteres.");

        if (!Roles.Todos.Contains(request.Rol))
            resultado.Agregar(nameof(request.Rol), $"El rol debe ser uno de: {string.Join(", ", Roles.Todos)}.");

        return resultado;
    }
}
