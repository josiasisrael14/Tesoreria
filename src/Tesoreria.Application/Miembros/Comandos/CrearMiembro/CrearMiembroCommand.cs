using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Comandos.CrearMiembro;

public record CrearMiembroCommand(
    string Nombres,
    string Apellidos,
    string? DocumentoIdentidad,
    string? Telefono,
    string? Email) : IRequest<MiembroDto>;
