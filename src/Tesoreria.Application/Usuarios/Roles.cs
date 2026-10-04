namespace Tesoreria.Application.Usuarios;

/// <summary>
/// Roles del sistema. Son constantes (no una tabla editable desde la UI) a propósito:
/// agregar un rol nuevo es una decisión de código, no algo que un usuario deba poder
/// crear desde la pantalla de Usuarios.
/// </summary>
public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Tesorero = "Tesorero";
    public const string Secretaria = "Secretaria";
    public const string Pastor = "Pastor";

    public static readonly string[] Todos = { Administrador, Tesorero, Secretaria, Pastor };
}
