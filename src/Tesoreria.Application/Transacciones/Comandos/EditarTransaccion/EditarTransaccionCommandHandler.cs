using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Transacciones.Comandos.EditarTransaccion;

public class EditarTransaccionCommandHandler : IRequestHandler<EditarTransaccionCommand, TransaccionDto>
{
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IFondoRepository _fondoRepository;
    private readonly IMiembroRepository _miembroRepository;
    private readonly ICultoRepository _cultoRepository;
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarTransaccionCommandHandler(
        ITransaccionRepository transaccionRepository,
        IFondoRepository fondoRepository,
        IMiembroRepository miembroRepository,
        ICultoRepository cultoRepository,
        IOfrendaEspecialRepository ofrendaEspecialRepository,
        IUnitOfWork unitOfWork)
    {
        _transaccionRepository = transaccionRepository;
        _fondoRepository = fondoRepository;
        _miembroRepository = miembroRepository;
        _cultoRepository = cultoRepository;
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransaccionDto> Handle(EditarTransaccionCommand request, CancellationToken cancellationToken)
    {
        // ObtenerPorIdAsync debe traer (Include) la navegación a Fondo, Miembro y
        // OfrendaEspecial, para armar el DTO sin otra consulta luego de editar.
        var transaccion = await _transaccionRepository.ObtenerPorIdAsync(request.TransaccionId, cancellationToken)
            ?? throw new DomainException($"No existe la transacción #{request.TransaccionId}.");

        var fondo = await _fondoRepository.ObtenerPorIdAsync(request.FondoId, cancellationToken)
            ?? throw new DomainException($"No existe el fondo #{request.FondoId}.");

        // A propósito NO se valida fondo.Activo/ofrendaEspecial.Activo aquí: desactivar
        // un fondo o cerrar una campaña bloquea registrar movimientos NUEVOS (ver
        // RegistrarIngresoCommandHandler), pero corregir un dato de algo que ya existe
        // no debería quedar bloqueado por eso.
        Miembro? miembro = null;
        if (request.MiembroId.HasValue)
        {
            miembro = await _miembroRepository.ObtenerPorIdAsync(request.MiembroId.Value, cancellationToken)
                ?? throw new DomainException($"No existe el miembro #{request.MiembroId}.");
        }

        if (request.CultoId.HasValue)
        {
            var culto = await _cultoRepository.ObtenerPorIdAsync(request.CultoId.Value, cancellationToken)
                ?? throw new DomainException($"No existe el culto #{request.CultoId}.");
        }

        OfrendaEspecial? ofrendaEspecial = null;
        if (request.OfrendaEspecialId.HasValue)
        {
            ofrendaEspecial = await _ofrendaEspecialRepository.ObtenerPorIdAsync(request.OfrendaEspecialId.Value, cancellationToken)
                ?? throw new DomainException($"No existe la ofrenda especial #{request.OfrendaEspecialId}.");
        }

        transaccion.Editar(
            request.Fecha, request.FondoId, request.Monto, request.MedioPago,
            request.MiembroId, request.CultoId, request.OfrendaEspecialId,
            request.Concepto, request.NumeroComprobante);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new TransaccionDto(
            transaccion.Id, transaccion.Fecha, transaccion.TipoMovimiento, transaccion.Monto,
            transaccion.MedioPago, transaccion.Concepto, transaccion.NumeroComprobante,
            transaccion.FondoId, fondo.Nombre, transaccion.MiembroId, miembro?.NombreCompleto,
            transaccion.CultoId, transaccion.OfrendaEspecialId, ofrendaEspecial?.Nombre,
            transaccion.EsReversa, transaccion.Reversada);
    }
}
