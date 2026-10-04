using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

/// <summary>
/// Catálogo de campañas de ofrenda especial (ofrenda misionera, pro templo,
/// aniversario, etc.). Una Transaccion de ingreso al fondo de Ofrendas Especiales
/// puede asociarse a una de estas campañas (OfrendaEspecialId) para poder reportar
/// cuánto se ha recaudado en cada una, y cuánto ha dado cada miembro por campaña.
///
/// Cada campaña pertenece a un Año: así "Aniversario" se puede volver a crear cada
/// año (Aniversario 2026, Aniversario 2027, ...) sin chocar con el nombre usado el
/// año anterior. Cada año queda como una fila separada, con su propio historial de
/// compromisos y transacciones — no hay que hacer nada más para que quede "como
/// histórico": simplemente esa fila deja de ser la que se usa este año.
/// </summary>
public class OfrendaEspecial : EntidadBase
{
    public string Nombre { get; private set; } = default!;
    public int Anio { get; private set; }
    public string? Descripcion { get; private set; }
    public DateTime? FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }

    /// <summary>Monto objetivo de la campaña (opcional, ej. para "Pro Templo").</summary>
    public decimal? MetaMonto { get; private set; }

    public bool Activo { get; private set; } = true;

    protected OfrendaEspecial() { } // EF Core

    public OfrendaEspecial(string nombre, int anio, string? descripcion = null, DateTime? fechaInicio = null,
        DateTime? fechaFin = null, decimal? metaMonto = null)
    {
        SetNombre(nombre);
        SetAnio(anio);
        SetMetaMonto(metaMonto);
        ActualizarVigencia(fechaInicio, fechaFin);
        Descripcion = descripcion;
    }

    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la ofrenda especial no puede estar vacío.");

        Nombre = nombre.Trim();
    }

    public void SetAnio(int anio)
    {
        if (anio < 2000 || anio > 2100)
            throw new DomainException("El año de la campaña no es válido.");

        Anio = anio;
    }

    public void SetMetaMonto(decimal? metaMonto)
    {
        if (metaMonto is <= 0)
            throw new DomainException("La meta de la campaña, si se define, debe ser mayor a cero.");

        MetaMonto = metaMonto;
    }

    public void ActualizarVigencia(DateTime? fechaInicio, DateTime? fechaFin)
    {
        if (fechaInicio.HasValue && fechaFin.HasValue && fechaFin.Value.Date < fechaInicio.Value.Date)
            throw new DomainException("La fecha de fin de la campaña no puede ser anterior a la fecha de inicio.");

        FechaInicio = fechaInicio?.Date;
        FechaFin = fechaFin?.Date;
    }

    public void ActualizarDescripcion(string? descripcion) => Descripcion = descripcion;

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}
