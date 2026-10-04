using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Comandos.ActivarMiembro;

/// <summary>Reactiva a un miembro que había sido desactivado ("eliminado" lógicamente).</summary>
public record ActivarMiembroCommand(int MiembroId) : IRequest<bool>;
