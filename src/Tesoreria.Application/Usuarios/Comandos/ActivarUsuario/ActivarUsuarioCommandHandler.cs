using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.ActivarUsuario;

public class ActivarUsuarioCommandHandler : IRequestHandler<ActivarUsuarioCommand, bool>
{
    private readonly IIdentityService _identityService;

    public ActivarUsuarioCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(ActivarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.ActivarUsuarioAsync(request.UsuarioId, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return true;
    }
}
