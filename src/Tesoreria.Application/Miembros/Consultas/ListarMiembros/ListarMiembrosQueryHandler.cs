using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Paginacion;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Miembros.Consultas.ListarMiembros;

public class ListarMiembrosQueryHandler : IRequestHandler<ListarMiembrosQuery, ResultadoPaginado<MiembroDto>>
{
    private readonly IMiembroRepository _miembroRepository;

    public ListarMiembrosQueryHandler(IMiembroRepository miembroRepository)
    {
        _miembroRepository = miembroRepository;
    }

    public async Task<ResultadoPaginado<MiembroDto>> Handle(ListarMiembrosQuery request, CancellationToken cancellationToken)
    {
        var pagina = request.Pagina < 1 ? 1 : request.Pagina;
        var tamanoPagina = request.TamanoPagina is < 1 or > 100 ? 10 : request.TamanoPagina;

        var (items, total) = await _miembroRepository.ListarPaginadoAsync(
            request.SoloActivos, request.Busqueda, pagina, tamanoPagina, cancellationToken);

        var dtos = items
            .Select(m => new MiembroDto(m.Id, m.Nombres, m.Apellidos, m.DocumentoIdentidad, m.Telefono, m.Email, m.Activo))
            .ToList();

        return new ResultadoPaginado<MiembroDto>(dtos, total, pagina, tamanoPagina);
    }
}
