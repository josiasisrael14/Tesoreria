using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Consultas.ListarUsuarios;

public class ListarUsuariosQueryHandler : IRequestHandler<ListarUsuariosQuery, List<UsuarioDto>>
{
    private readonly IIdentityService _identityService;

    public ListarUsuariosQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<List<UsuarioDto>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
        => _identityService.ListarUsuariosAsync(cancellationToken);
}
