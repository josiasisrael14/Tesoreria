using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Transacciones.Consultas.BalancePorFondo;

public class BalancePorFondoQueryHandler : IRequestHandler<BalancePorFondoQuery, List<BalancePorFondoRegistro>>
{
    private readonly ITransaccionRepository _transaccionRepository;

    public BalancePorFondoQueryHandler(ITransaccionRepository transaccionRepository)
    {
        _transaccionRepository = transaccionRepository;
    }

    public Task<List<BalancePorFondoRegistro>> Handle(BalancePorFondoQuery request, CancellationToken cancellationToken)
        => _transaccionRepository.ObtenerBalancePorFondoAsync(request.Desde, request.Hasta, cancellationToken);
}
