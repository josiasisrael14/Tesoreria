using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Transacciones.Comandos.ReversarTransaccion;

/// <summary>
/// Corrige un error sin borrar ni editar la transacción original: crea la
/// contrapartida que anula su efecto, para que el libro quede auditable.
/// </summary>
public record ReversarTransaccionCommand(int TransaccionId, int UsuarioRegistroId, string? Motivo)
    : IRequest<TransaccionDto>;
