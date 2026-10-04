using Tesoreria.Domain.Enums;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

/// <summary>
/// La tabla de hechos del libro de tesorería: cada ingreso o egreso.
/// Regla de negocio central: una vez guardada, el Monto nunca se edita ni se borra.
/// Un error se corrige creando una transacción de reversa que referencia a esta.
/// </summary>
public class Transaccion : EntidadBase
{
    public DateTime Fecha { get; private set; }
    public TipoMovimiento TipoMovimiento { get; private set; }
    public decimal Monto { get; private set; }
    public MedioPago MedioPago { get; private set; }
    public string? Concepto { get; private set; }
    public string? NumeroComprobante { get; private set; }

    public int FondoId { get; private set; }
    public Fondo Fondo { get; private set; } = default!;

    public int? MiembroId { get; private set; }
    public Miembro? Miembro { get; private set; }

    public int? CultoId { get; private set; }
    public Culto? Culto { get; private set; }

    /// <summary>
    /// Campaña de ofrenda especial a la que pertenece este ingreso (misionera, pro
    /// templo, aniversario, etc.). Solo aplica a ingresos del fondo de Ofrendas
    /// Especiales; en cualquier otro caso queda null.
    /// </summary>
    public int? OfrendaEspecialId { get; private set; }
    public OfrendaEspecial? OfrendaEspecial { get; private set; }

    public int UsuarioRegistroId { get; private set; }

    /// <summary>True si esta transacción es en sí misma la reversa de otra.</summary>
    public bool EsReversa { get; private set; }

    /// <summary>Id de la transacción original que esta reversa contrarresta (si EsReversa = true).</summary>
    public int? TransaccionOrigenId { get; private set; }

    /// <summary>True si esta transacción ya fue reversada por otra (no se puede reversar dos veces).</summary>
    public bool Reversada { get; private set; }

    protected Transaccion() { } // EF Core

    private Transaccion(
        DateTime fecha,
        TipoMovimiento tipoMovimiento,
        decimal monto,
        int fondoId,
        MedioPago medioPago,
        int usuarioRegistroId,
        int? miembroId,
        int? cultoId,
        int? ofrendaEspecialId,
        string? concepto,
        string? numeroComprobante,
        bool esReversa,
        int? transaccionOrigenId)
    {
        if (monto <= 0)
            throw new DomainException("El monto de una transacción debe ser mayor a cero.");
        if (fecha == default)
            throw new DomainException("La fecha de la transacción es obligatoria.");
        if (fondoId <= 0)
            throw new DomainException("La transacción debe pertenecer a un fondo.");

        Fecha = fecha;
        TipoMovimiento = tipoMovimiento;
        Monto = monto;
        FondoId = fondoId;
        MedioPago = medioPago;
        UsuarioRegistroId = usuarioRegistroId;
        MiembroId = miembroId;
        CultoId = cultoId;
        OfrendaEspecialId = ofrendaEspecialId;
        Concepto = concepto;
        NumeroComprobante = numeroComprobante;
        EsReversa = esReversa;
        TransaccionOrigenId = transaccionOrigenId;
    }

    public static Transaccion CrearIngreso(
        DateTime fecha, int fondoId, decimal monto, MedioPago medioPago, int usuarioRegistroId,
        int? miembroId = null, int? cultoId = null, int? ofrendaEspecialId = null,
        string? concepto = null, string? numeroComprobante = null)
        => new(fecha, TipoMovimiento.Ingreso, monto, fondoId, medioPago, usuarioRegistroId,
               miembroId, cultoId, ofrendaEspecialId, concepto, numeroComprobante,
               esReversa: false, transaccionOrigenId: null);

    public static Transaccion CrearEgreso(
        DateTime fecha, int fondoId, decimal monto, MedioPago medioPago, int usuarioRegistroId,
        string? concepto = null, string? numeroComprobante = null)
        => new(fecha, TipoMovimiento.Egreso, monto, fondoId, medioPago, usuarioRegistroId,
               miembroId: null, cultoId: null, ofrendaEspecialId: null, concepto, numeroComprobante,
               esReversa: false, transaccionOrigenId: null);

    /// <summary>
    /// Crea la transacción contraria que anula el efecto de esta (el tipo de movimiento
    /// se invierte). La transacción original queda marcada como Reversada. Se conservan
    /// el fondo, el miembro y la campaña de ofrenda especial originales, para que el
    /// reporte por campaña y el historial del miembro sigan cuadrando en neto.
    /// Ya no se usa desde la UI (se reemplazó por Editar, más simple para el caso
    /// común de un error de tipeo), pero se deja disponible por si hiciera falta
    /// anular un movimiento sin editar el original.
    /// </summary>
    public Transaccion Reversar(int usuarioRegistroId, string? motivo)
    {
        if (EsReversa)
            throw new DomainException("Una transacción de reversa no se puede volver a reversar.");
        if (Reversada)
            throw new DomainException("Esta transacción ya fue reversada anteriormente.");

        var tipoContrario = TipoMovimiento == TipoMovimiento.Ingreso
            ? TipoMovimiento.Egreso
            : TipoMovimiento.Ingreso;

        Reversada = true;

        return new Transaccion(
            fecha: DateTime.UtcNow.Date,
            tipoMovimiento: tipoContrario,
            monto: Monto,
            fondoId: FondoId,
            medioPago: MedioPago,
            usuarioRegistroId: usuarioRegistroId,
            miembroId: MiembroId,
            cultoId: CultoId,
            ofrendaEspecialId: OfrendaEspecialId,
            concepto: string.IsNullOrWhiteSpace(motivo)
                ? $"Reversa de transacción #{Id}"
                : $"Reversa de transacción #{Id}: {motivo}",
            numeroComprobante: null,
            esReversa: true,
            transaccionOrigenId: Id);
    }

    /// <summary>
    /// Corrige los datos de una transacción ya guardada en el sitio (se escribió
    /// mal el monto, el fondo, el miembro, etc.). A diferencia de Reversar, esto
    /// SÍ cambia el registro original — pensado para el caso más común: un error
    /// de tipeo al registrar un diezmo u otro ingreso/egreso.
    /// No se puede editar una reversa (rompería el vínculo con la transacción que
    /// corrige); tampoco se puede cambiar el tipo de movimiento (Ingreso/Egreso).
    /// </summary>
    public void Editar(
        DateTime fecha,
        int fondoId,
        decimal monto,
        MedioPago medioPago,
        int? miembroId,
        int? cultoId,
        int? ofrendaEspecialId,
        string? concepto,
        string? numeroComprobante)
    {
        if (EsReversa)
            throw new DomainException("Una transacción de reversa no se puede editar directamente.");
        if (monto <= 0)
            throw new DomainException("El monto de una transacción debe ser mayor a cero.");
        if (fecha == default)
            throw new DomainException("La fecha de la transacción es obligatoria.");
        if (fondoId <= 0)
            throw new DomainException("La transacción debe pertenecer a un fondo.");

        Fecha = fecha;
        FondoId = fondoId;
        Monto = monto;
        MedioPago = medioPago;
        Concepto = concepto;
        NumeroComprobante = numeroComprobante;

        if (TipoMovimiento == TipoMovimiento.Ingreso)
        {
            MiembroId = miembroId;
            CultoId = cultoId;
            OfrendaEspecialId = ofrendaEspecialId;
        }
    }
}
