using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Enums;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Transacciones.Comandos.RegistrarIngreso;

public class RegistrarIngresoCommandHandler : IRequestHandler<RegistrarIngresoCommand, TransaccionDto>
{
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IFondoRepository _fondoRepository;
    private readonly IMiembroRepository _miembroRepository;
    private readonly ICultoRepository _cultoRepository;
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarIngresoCommandHandler(
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

    public async Task<TransaccionDto> Handle(RegistrarIngresoCommand request, CancellationToken cancellationToken)
    {
        var fondo = await _fondoRepository.ObtenerPorIdAsync(request.FondoId, cancellationToken)
            ?? throw new DomainException($"No existe el fondo #{request.FondoId}.");

        if (!fondo.Activo)
            throw new DomainException($"El fondo '{fondo.Nombre}' está desactivado, no se pueden registrar nuevos ingresos.");

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

            if (!ofrendaEspecial.Activo)
                throw new DomainException($"La campaña '{ofrendaEspecial.Nombre}' ya está cerrada, no se le pueden registrar nuevos aportes.");
        }

        // Protección contra doble envío: si ya existe un ingreso prácticamente
        // idéntico guardado hace pocos segundos, es casi seguro que es un reintento
        // (se perdió la respuesta anterior por un corte de internet, el navegador
        // reenvió la petición, etc.) y no un segundo ingreso real. Devolvemos el que
        // ya existe en vez de crear uno nuevo.
        var posibleDuplicado = await _transaccionRepository.BuscarPosibleDuplicadoAsync(
            TipoMovimiento.Ingreso, request.FondoId, request.Monto, request.MedioPago,
            request.UsuarioRegistroId, request.MiembroId, DateTime.UtcNow.AddSeconds(-10), cancellationToken);

        if (posibleDuplicado is not null)
            return TransaccionDto.DesdeEntidad(posibleDuplicado) with { EsDuplicadoDetectado = true };

        var transaccion = Transaccion.CrearIngreso(
            request.Fecha, request.FondoId, request.Monto, request.MedioPago, request.UsuarioRegistroId,
            request.MiembroId, request.CultoId, request.OfrendaEspecialId, request.Concepto, request.NumeroComprobante);

        await _transaccionRepository.AgregarAsync(transaccion, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new TransaccionDto(
            transaccion.Id, transaccion.Fecha, transaccion.TipoMovimiento, transaccion.Monto,
            transaccion.MedioPago, transaccion.Concepto, transaccion.NumeroComprobante,
            transaccion.FondoId, fondo.Nombre, transaccion.MiembroId, miembro?.NombreCompleto,
            transaccion.CultoId, transaccion.OfrendaEspecialId, ofrendaEspecial?.Nombre,
            transaccion.EsReversa, transaccion.Reversada);
    }
}
