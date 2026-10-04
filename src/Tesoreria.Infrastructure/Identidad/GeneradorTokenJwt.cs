using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Usuarios;

namespace Tesoreria.Infrastructure.Identidad;

public class GeneradorTokenJwt : IGeneradorTokenJwt
{
    private readonly JwtSettings _settings;

    public GeneradorTokenJwt(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenGeneradoDto Generar(UsuarioDto usuario)
    {
        var expiracionUtc = DateTime.UtcNow.AddMinutes(_settings.MinutosExpiracion);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.NombreCompleto),
            new(ClaimTypes.Role, usuario.Rol)
        };

        var claveFirma = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.ClaveSecreta));
        var credenciales = new SigningCredentials(claveFirma, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Emisor,
            audience: _settings.Audiencia,
            claims: claims,
            expires: expiracionUtc,
            signingCredentials: credenciales);

        return new TokenGeneradoDto(new JwtSecurityTokenHandler().WriteToken(token), expiracionUtc);
    }
}
