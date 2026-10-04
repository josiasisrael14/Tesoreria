using Tesoreria.Application.Abstracciones.Validacion;

namespace Tesoreria.Application.Usuarios.Comandos.SolicitarRecuperacionContrasena;

public class SolicitarRecuperacionContrasenaValidador : IValidador<SolicitarRecuperacionContrasenaCommand>
{
    public ResultadoValidacion Validar(SolicitarRecuperacionContrasenaCommand request)
    {
        var resultado = ResultadoValidacion.Ok();

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            resultado.Agregar(nameof(request.Email), "El correo no es válido.");

        return resultado;
    }
}
