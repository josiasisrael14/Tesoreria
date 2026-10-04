using Microsoft.AspNetCore.Identity;

namespace Tesoreria.Infrastructure.Identidad;

/// <summary>
/// Extiende el usuario estándar de Identity con los dos campos que necesitamos:
/// el nombre para mostrar en la UI, y un soft-delete (Activo) consistente con el
/// resto del sistema (Miembro, Fondo, OfrendaEspecial usan el mismo patrón).
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = default!;
    public bool Activo { get; set; } = true;

    /// <summary>Ruta pública (p. ej. "/uploads/usuarios/{id}.jpg") de la foto de perfil, o null si no tiene.</summary>
    public string? FotoUrl { get; set; }
}
