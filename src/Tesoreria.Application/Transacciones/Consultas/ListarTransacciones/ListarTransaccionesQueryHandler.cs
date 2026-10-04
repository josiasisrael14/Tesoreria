using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Transacciones.Consultas.ListarTransacciones;

public class ListarTransaccionesQueryHandler : IRequestHandler<ListarTransaccionesQuery, List<TransaccionDto>>
{
    private readonly ITransaccionRepository _transaccionRepository;

    public ListarTransaccionesQueryHandler(ITransaccionRepository transaccionRepository)
    {
        _transaccionRepository = transaccionRepository;
    }

    public async Task<List<TransaccionDto>> Handle(ListarTransaccionesQuery request, CancellationToken cancellationToken)
    {
        var filtro = new FiltroTransacciones
        {
            FondoId = request.FondoId,
            CultoId = request.CultoId,
            MiembroId = request.MiembroId,
            OfrendaEspecialId = request.OfrendaEspecialId,
            Desde = request.Desde,
            Hasta = request.Hasta,
            MedioPago = request.MedioPago
        };

        var transacciones = await _transaccionRepository.ListarAsync(filtro, cancellationToken);

        return transacciones.Select(TransaccionDto.DesdeEntidad).ToList();
    }
}
