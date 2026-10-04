using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Cultos.Consultas.ListarCultos;

public class ListarCultosQueryHandler : IRequestHandler<ListarCultosQuery, List<CultoDto>>
{
    private readonly ICultoRepository _cultoRepository;

    public ListarCultosQueryHandler(ICultoRepository cultoRepository)
    {
        _cultoRepository = cultoRepository;
    }

    public async Task<List<CultoDto>> Handle(ListarCultosQuery request, CancellationToken cancellationToken)
    {
        var cultos = await _cultoRepository.ListarAsync(request.Desde, request.Hasta, cancellationToken);

        return cultos
            .Select(c => new CultoDto(c.Id, c.Fecha, c.Tipo, c.Descripcion))
            .ToList();
    }
}
