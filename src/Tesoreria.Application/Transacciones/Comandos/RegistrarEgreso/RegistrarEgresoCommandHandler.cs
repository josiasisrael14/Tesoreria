using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Enums;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Transacciones.Comandos.RegistrarEgreso;

public class RegistrarEgresoCommandHandler : IRequestHandler<RegistrarEgresoCommand, TransaccionDto>
{
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IFondoRepository _fondoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarEgresoCommandHandler(
        ITransaccionRepository transaccionRepository, IFondoRepository fondoRepository, IUnitOfWork unitOfWork)
    {
        _transaccionRepository = transaccionRepository;
        _fondoRepository = fondoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransaccionDto> Handle(RegistrarEgresoCommand request, CancellationToken cancellationToken)
    {
        var fondo = await _fondoRepository.ObtenerPorIdAsync(request.FondoId, cancellationToken)
            ?? throw new DomainException($"No existe el fondo #{request.FondoId}.");

        if (!fondo.Activo)
            throw new DomainException($"El fondo '{fondo.Nombre}' está desactivado, no se pueden registrar nuevos egresos.");

        // Misma protección contra doble envío que en RegistrarIngreso: si ya existe
        // un egreso prácticamente idéntico guardado hace pocos segundos, devolvemos
        // ese en vez de crear uno nuevo.
        var posibleDuplicado = await _transaccionRepository.BuscarPosibleDuplicadoAsync(
            TipoMovimiento.Egreso, request.FondoId, request.Monto, request.MedioPago,
            request.UsuarioRegistroId, null, DateTime.UtcNow.AddSeconds(-10), cancellationToken);

        if (posibleDuplicado is not null)
            return TransaccionDto.DesdeEntidad(posibleDuplicado) with { EsDuplicadoDetectado = true };

        var transaccion = Transaccion.CrearEgreso(
            request.Fecha, request.FondoId, request.Monto, request.MedioPago, request.UsuarioRegistroId,
            request.Concepto, request.NumeroComprobante);

        await _transaccionRepository.AgregarAsync(transaccion, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new TransaccionDto(
            transaccion.Id, transaccion.Fecha, transaccion.TipoMovimiento, transaccion.Monto,
            transaccion.MedioPago, transaccion.Concepto, transaccion.NumeroComprobante,
            transaccion.FondoId, fondo.Nombre, null, null,
            transaccion.CultoId, null, null, transaccion.EsReversa, transaccion.Reversada);
    }
}
