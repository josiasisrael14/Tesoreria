using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Miembros.Comandos.CrearMiembro;

public class CrearMiembroValidador : IValidador<CrearMiembroCommand>
{
    public ResultadoValidacion Validar(CrearMiembroCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Nombres))
            resultado.Agregar(nameof(request.Nombres), "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Apellidos))
            resultado.Agregar(nameof(request.Apellidos), "El apellido es obligatorio.");

        if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains('@'))
            resultado.Agregar(nameof(request.Email), "El correo no tiene un formato válido.");

        return resultado;
    }
}
