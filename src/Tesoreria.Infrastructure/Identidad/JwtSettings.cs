namespace Tesoreria.Infrastructure.Identidad;

/// <summary>Mapea la sección "Jwt" de appsettings.json.</summary>
public class JwtSettings
{
    public string ClaveSecreta { get; set; } = default!;
    public string Emisor { get; set; } = default!;
    public string Audiencia { get; set; } = default!;
    public int MinutosExpiracion { get; set; } = 480;
}
