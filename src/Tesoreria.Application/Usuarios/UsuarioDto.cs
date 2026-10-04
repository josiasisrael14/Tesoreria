namespace Tesoreria.Application.Usuarios;

public record UsuarioDto(string Id, string Email, string NombreCompleto, string Rol, bool Activo, string? FotoUrl);
