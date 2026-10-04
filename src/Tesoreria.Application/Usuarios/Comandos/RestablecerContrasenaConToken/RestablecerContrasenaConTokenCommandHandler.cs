using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasenaConToken;

public class RestablecerContrasenaConTokenCommandHandler
    : IRequestHandler<RestablecerContrasenaConTokenCommand, bool>
{
    private readonly IIdentityService _identityService;

    public RestablecerContrasenaConTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(RestablecerContrasenaConTokenCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.RestablecerContrasenaConTokenAsync(
            request.Email, request.Token, request.NuevaContrasena, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return true;
    }
}
