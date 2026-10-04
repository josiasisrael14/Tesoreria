using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Fondos.Consultas.ListarFondos;

public class ListarFondosQueryHandler : IRequestHandler<ListarFondosQuery, List<FondoDto>>
{
    private readonly IFondoRepository _fondoRepository;

    public ListarFondosQueryHandler(IFondoRepository fondoRepository)
    {
        _fondoRepository = fondoRepository;
    }

    public async Task<List<FondoDto>> Handle(ListarFondosQuery request, CancellationToken cancellationToken)
    {
        var fondos = await _fondoRepository.ListarAsync(request.SoloActivos, cancellationToken);

        return fondos
            .Select(f => new FondoDto(f.Id, f.Nombre, f.Descripcion, f.Tipo, f.Activo, f.EsFondoDeOfrendasEspeciales))
            .ToList();
    }
}
