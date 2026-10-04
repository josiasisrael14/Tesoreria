using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.DesactivarUsuario;

public class DesactivarUsuarioCommandHandler : IRequestHandler<DesactivarUsuarioCommand, bool>
{
    private readonly IIdentityService _identityService;

    public DesactivarUsuarioCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(DesactivarUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (request.UsuarioId == request.UsuarioActualId)
            throw new DomainException("No puedes desactivar tu propio usuario.");

        var resultado = await _identityService.DesactivarUsuarioAsync(request.UsuarioId, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return true;
    }
}
