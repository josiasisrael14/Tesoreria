namespace Tesoreria.Application.Miembros;

public record MiembroDto(int Id, string Nombres, string Apellidos, string? DocumentoIdentidad,
    string? Telefono, string? Email, bool Activo);
