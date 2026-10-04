namespace Tesoreria.Infrastructure.Identidad;

/// <summary>Mapea la sección "AdminInicial" de appsettings.json: los datos del primer
/// administrador, creado automáticamente por IdentitySeeder si todavía no hay ningún usuario.</summary>
public class AdminInicialSettings
{
    public string Email { get; set; } = default!;
    public string Contrasena { get; set; } = default!;
    public string NombreCompleto { get; set; } = "Administrador";
}
