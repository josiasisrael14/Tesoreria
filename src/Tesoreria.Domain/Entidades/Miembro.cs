using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

/// <summary>
/// Un miembro/feligrés que puede aparecer como donante en una transacción.
/// Es opcional: una ofrenda puede quedar sin miembro asociado (anónima).
/// </summary>
public class Miembro : EntidadBase
{
    public string Nombres { get; private set; } = default!;
    public string Apellidos { get; private set; } = default!;
    public string? DocumentoIdentidad { get; private set; }
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public bool Activo { get; private set; } = true;

    public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();

    protected Miembro() { } // EF Core

    public Miembro(string nombres, string apellidos, string? documentoIdentidad = null,
        string? telefono = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(nombres))
            throw new DomainException("El nombre del miembro es obligatorio.");
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new DomainException("El apellido del miembro es obligatorio.");

        Nombres = nombres.Trim();
        Apellidos = apellidos.Trim();
        DocumentoIdentidad = documentoIdentidad;
        Telefono = telefono;
        Email = email;
    }

    public void ActualizarDatosContacto(string? telefono, string? email)
    {
        Telefono = telefono;
        Email = email;
    }

    /// <summary>Actualiza todos los datos editables del miembro (usado al editar desde el listado).</summary>
    public void ActualizarDatos(string nombres, string apellidos, string? documentoIdentidad,
        string? telefono, string? email)
    {
        if (string.IsNullOrWhiteSpace(nombres))
            throw new DomainException("El nombre del miembro es obligatorio.");
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new DomainException("El apellido del miembro es obligatorio.");

        Nombres = nombres.Trim();
        Apellidos = apellidos.Trim();
        DocumentoIdentidad = documentoIdentidad;
        Telefono = telefono;
        Email = email;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
