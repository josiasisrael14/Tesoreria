using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Transacciones.Comandos.ReversarTransaccion;

public class ReversarTransaccionCommandHandler : IRequestHandler<ReversarTransaccionCommand, TransaccionDto>
{
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReversarTransaccionCommandHandler(ITransaccionRepository transaccionRepository, IUnitOfWork unitOfWork)
    {
        _transaccionRepository = transaccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransaccionDto> Handle(ReversarTransaccionCommand request, CancellationToken cancellationToken)
    {
        // ObtenerPorIdAsync debe traer (Include) la navegación a Fondo, Miembro y
        // OfrendaEspecial, para poder armar el DTO de la reversa sin otra consulta.
        var original = await _transaccionRepository.ObtenerPorIdAsync(request.TransaccionId, cancellationToken)
            ?? throw new DomainException($"No existe la transacción #{request.TransaccionId}.");

        var reversa = original.Reversar(request.UsuarioRegistroId, request.Motivo);

        await _transaccionRepository.AgregarAsync(reversa, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new TransaccionDto(
            reversa.Id, reversa.Fecha, reversa.TipoMovimiento, reversa.Monto, reversa.MedioPago,
            reversa.Concepto, reversa.NumeroComprobante, reversa.FondoId, original.Fondo.Nombre,
            reversa.MiembroId, original.Miembro?.NombreCompleto, reversa.CultoId,
            reversa.OfrendaEspecialId, original.OfrendaEspecial?.Nombre,
            reversa.EsReversa, reversa.Reversada);
    }
}
