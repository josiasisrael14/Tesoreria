using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Paginacion;

namespace Tesoreria.Application.Miembros.Consultas.ListarMiembros;

public record ListarMiembrosQuery(
    bool SoloActivos = true,
    string? Busqueda = null,
    int Pagina = 1,
    int TamanoPagina = 10) : IRequest<ResultadoPaginado<MiembroDto>>;
