using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.OfrendasEspeciales.Consultas.ListarOfrendasEspeciales;

public class ListarOfrendasEspecialesQueryHandler
    : IRequestHandler<ListarOfrendasEspecialesQuery, List<OfrendaEspecialDto>>
{
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;

    public ListarOfrendasEspecialesQueryHandler(IOfrendaEspecialRepository ofrendaEspecialRepository)
    {
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
    }

    public async Task<List<OfrendaEspecialDto>> Handle(ListarOfrendasEspecialesQuery request, CancellationToken cancellationToken)
    {
        var ofrendas = await _ofrendaEspecialRepository.ListarAsync(request.SoloActivas, cancellationToken);

        return ofrendas
            .Select(o => new OfrendaEspecialDto(o.Id, o.Nombre, o.Anio, o.Descripcion, o.FechaInicio, o.FechaFin, o.MetaMonto, o.Activo))
            .ToList();
    }
}
