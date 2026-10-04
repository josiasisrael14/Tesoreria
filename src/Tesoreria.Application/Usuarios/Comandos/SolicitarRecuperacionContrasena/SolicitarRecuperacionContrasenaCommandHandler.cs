using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.SolicitarRecuperacionContrasena;

public class SolicitarRecuperacionContrasenaCommandHandler
    : IRequestHandler<SolicitarRecuperacionContrasenaCommand, bool>
{
    private readonly IIdentityService _identityService;

    public SolicitarRecuperacionContrasenaCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(SolicitarRecuperacionContrasenaCommand request, CancellationToken cancellationToken)
    {
        await _identityService.SolicitarRecuperacionContrasenaAsync(request.Email, cancellationToken);

        // Siempre true: el controller responde el mismo mensaje exista o no ese correo en el
        // sistema, para no filtrarle a quien pregunta qué correos están registrados.
        return true;
    }
}
