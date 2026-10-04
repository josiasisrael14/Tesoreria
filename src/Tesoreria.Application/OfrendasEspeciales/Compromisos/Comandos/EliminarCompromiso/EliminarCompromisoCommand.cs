using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.EliminarCompromiso;

/// <summary>
/// Elimina un compromiso (no es dinero movido, así que a diferencia de una
/// Transaccion sí se borra de verdad, no queda como registro histórico).
/// </summary>
public record EliminarCompromisoCommand(int Id) : IRequest<bool>;
