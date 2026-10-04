using Tesoreria.Application.Usuarios;

namespace Tesoreria.Application.Abstracciones.Identidad;

/// <summary>Genera el token JWT para un usuario ya autenticado. Separado de IIdentityService
/// porque son dos responsabilidades distintas: validar credenciales vs. firmar un token.</summary>
public interface IGeneradorTokenJwt
{
    TokenGeneradoDto Generar(UsuarioDto usuario);
}

public record TokenGeneradoDto(string Token, DateTime ExpiracionUtc);
