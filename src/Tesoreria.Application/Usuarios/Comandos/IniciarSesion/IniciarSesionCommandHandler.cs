using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.IniciarSesion;

public class IniciarSesionCommandHandler : IRequestHandler<IniciarSesionCommand, SesionDto>
{
    private readonly IIdentityService _identityService;
    private readonly IGeneradorTokenJwt _generadorToken;

    public IniciarSesionCommandHandler(IIdentityService identityService, IGeneradorTokenJwt generadorToken)
    {
        _identityService = identityService;
        _generadorToken = generadorToken;
    }

    public async Task<SesionDto> Handle(IniciarSesionCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _identityService.AutenticarAsync(request.Email, request.Contrasena, cancellationToken);

        // Mismo mensaje para "no existe", "contraseña incorrecta" y "usuario desactivado":
        // no hay que darle a quien intenta entrar ninguna pista de cuál es el caso.
        if (usuario is null)
            throw new DomainException("El correo o la contraseña no son correctos.");

        var token = _generadorToken.Generar(usuario);

        return new SesionDto(
            usuario.Id, usuario.Email, usuario.NombreCompleto, usuario.Rol, usuario.FotoUrl, token.Token, token.ExpiracionUtc);
    }
}
