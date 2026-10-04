using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Comandos.ActualizarMiembro;

public record ActualizarMiembroCommand(
    int Id,
    string Nombres,
    string Apellidos,
    string? DocumentoIdentidad,
    string? Telefono,
    string? Email) : IRequest<MiembroDto>;
