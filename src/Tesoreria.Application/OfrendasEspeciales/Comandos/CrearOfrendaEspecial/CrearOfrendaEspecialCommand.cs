using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.OfrendasEspeciales.Comandos.CrearOfrendaEspecial;

/// <summary>
/// Crea una campaña de ofrenda especial (ej. "Ofrenda Misionera", "Pro Templo",
/// "Aniversario") para un año determinado. El mismo nombre se puede volver a usar
/// en otro año — lo único que no puede repetirse es Nombre + Año juntos.
/// </summary>
public record CrearOfrendaEspecialCommand(
    string Nombre,
    int Anio,
    string? Descripcion,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal? MetaMonto) : IRequest<OfrendaEspecialDto>;
