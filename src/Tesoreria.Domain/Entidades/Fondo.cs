using Tesoreria.Domain.Enums;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

/// <summary>
/// Un fondo agrupa transacciones del mismo tipo: Diezmo, Ofrenda General,
/// Ofrenda Especial, Misiones, Construcción, etc.
/// </summary>
public class Fondo : EntidadBase
{
    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public TipoFondo Tipo { get; private set; }
    public bool Activo { get; private set; } = true;

    /// <summary>
    /// True para EL fondo que agrupa los aportes de todas las campañas de Ofrenda
    /// Especial. Antes esto se adivinaba buscando la palabra "especial" en el
    /// nombre del fondo — frágil, se rompía si alguien lo renombraba. Solo un
    /// fondo puede tener esto en true a la vez (lo garantiza el Handler, no la
    /// entidad, porque necesita consultar los demás fondos).
    /// </summary>
    public bool EsFondoDeOfrendasEspeciales { get; private set; }

    private readonly List<Transaccion> _transacciones = new();
    public IReadOnlyCollection<Transaccion> Transacciones => _transacciones.AsReadOnly();

    protected Fondo() { } // EF Core

    public Fondo(string nombre, TipoFondo tipo, string? descripcion = null, bool esFondoDeOfrendasEspeciales = false)
    {
        SetNombre(nombre);
        Tipo = tipo;
        Descripcion = descripcion;
        EsFondoDeOfrendasEspeciales = esFondoDeOfrendasEspeciales;
    }

    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del fondo no puede estar vacío.");

        Nombre = nombre.Trim();
    }

    public void ActualizarDescripcion(string? descripcion) => Descripcion = descripcion;

    public void MarcarComoFondoDeOfrendasEspeciales() => EsFondoDeOfrendasEspeciales = true;
    public void DesmarcarComoFondoDeOfrendasEspeciales() => EsFondoDeOfrendasEspeciales = false;

    /// <summary>Corrige en el sitio los datos del fondo (por ejemplo, un nombre mal escrito al crearlo).</summary>
    public void Editar(string nombre, TipoFondo tipo, string? descripcion, bool esFondoDeOfrendasEspeciales)
    {
        SetNombre(nombre);
        Tipo = tipo;
        Descripcion = descripcion;
        EsFondoDeOfrendasEspeciales = esFondoDeOfrendasEspeciales;
    }

    public void Desactivar()
    {
        if (!Activo) return;
        Activo = false;
    }

    public void Activar() => Activo = true;
}
