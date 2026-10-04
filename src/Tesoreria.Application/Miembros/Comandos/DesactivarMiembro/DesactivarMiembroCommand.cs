using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Comandos.DesactivarMiembro;

/// <summary>
/// "Eliminar" un miembro es lógico: pasa su campo Activo de true a false, nunca
/// se borra el registro (así se conserva su historial de aportes).
/// </summary>
public record DesactivarMiembroCommand(int MiembroId) : IRequest<bool>;
