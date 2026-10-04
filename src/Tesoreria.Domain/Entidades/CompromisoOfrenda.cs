using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

/// <summary>
/// Lo que un miembro se comprometió a dar para una campaña de ofrenda especial
/// (ej. "Pamela se compromete a dar S/500 para el Aniversario"), para poder
/// comparar contra lo que realmente dio y saber si está al día, pendiente o en
/// mora. Un miembro tiene un solo compromiso por campaña — si cambia de monto,
/// se edita este mismo registro (ver SetMonto), no se crean varios.
///
/// La fecha límite para dar sin caer en mora es la FechaFin de la propia
/// campaña (OfrendaEspecial.FechaFin) — no se guarda una fecha aparte acá. Si
/// el evento es el 30 y el plazo para dar es el 29, la campaña se crea con
/// FechaFin = 29.
/// </summary>
public class CompromisoOfrenda : EntidadBase
{
    public int OfrendaEspecialId { get; private set; }
    public OfrendaEspecial OfrendaEspecial { get; private set; } = default!;

    public int MiembroId { get; private set; }
    public Miembro Miembro { get; private set; } = default!;

    public decimal MontoComprometido { get; private set; }

    protected CompromisoOfrenda() { } // EF Core

    public CompromisoOfrenda(int ofrendaEspecialId, int miembroId, decimal montoComprometido)
    {
        if (ofrendaEspecialId <= 0)
            throw new DomainException("El compromiso debe pertenecer a una campaña de ofrenda especial.");
        if (miembroId <= 0)
            throw new DomainException("El compromiso debe pertenecer a un miembro.");

        OfrendaEspecialId = ofrendaEspecialId;
        MiembroId = miembroId;
        SetMonto(montoComprometido);
    }

    public void SetMonto(decimal montoComprometido)
    {
        if (montoComprometido <= 0)
            throw new DomainException("El monto comprometido debe ser mayor a cero.");

        MontoComprometido = montoComprometido;
    }
}
