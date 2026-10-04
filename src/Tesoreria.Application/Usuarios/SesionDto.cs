namespace Tesoreria.Application.Usuarios;

/// <summary>Lo que recibe el frontend al iniciar sesión: los datos del usuario + el token JWT.</summary>
public record SesionDto(string Id, string Email, string NombreCompleto, string Rol, string? FotoUrl, string Token, DateTime ExpiracionUtc);
