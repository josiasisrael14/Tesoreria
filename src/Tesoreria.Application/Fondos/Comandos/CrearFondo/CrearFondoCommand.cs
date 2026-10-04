using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Fondos.Comandos.CrearFondo;

public record CrearFondoCommand(
    string Nombre,
    TipoFondo Tipo,
    string? Descripcion,
    bool EsFondoDeOfrendasEspeciales = false) : IRequest<FondoDto>;
