using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Fondos.Comandos.EditarFondo;

public record EditarFondoCommand(
    int FondoId,
    string Nombre,
    TipoFondo Tipo,
    string? Descripcion,
    bool EsFondoDeOfrendasEspeciales) : IRequest<FondoDto>;
