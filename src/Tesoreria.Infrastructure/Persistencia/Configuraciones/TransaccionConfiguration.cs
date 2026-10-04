using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class TransaccionConfiguration : IEntityTypeConfiguration<Transaccion>
{
    public void Configure(EntityTypeBuilder<Transaccion> builder)
    {
        builder.ToTable("Transacciones");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Monto).HasColumnType("decimal(18,2)");
        builder.Property(t => t.TipoMovimiento).HasConversion<string>().HasMaxLength(10);
        builder.Property(t => t.MedioPago).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Concepto).HasMaxLength(400);
        builder.Property(t => t.NumeroComprobante).HasMaxLength(50);

        builder.HasOne(t => t.Fondo)
            .WithMany(f => f.Transacciones)
            .HasForeignKey(t => t.FondoId)
            .OnDelete(DeleteBehavior.Restrict); // nunca se borra en cascada el historial de movimientos

        builder.HasOne(t => t.Miembro)
            .WithMany()
            .HasForeignKey(t => t.MiembroId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(t => t.Culto)
            .WithMany()
            .HasForeignKey(t => t.CultoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Campaña de ofrenda especial (misionera, pro templo, aniversario...) — solo
        // aplica a ingresos del fondo de Ofrendas Especiales; en cualquier otro caso es null.
        builder.HasOne(t => t.OfrendaEspecial)
            .WithMany()
            .HasForeignKey(t => t.OfrendaEspecialId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Auto-referencia: la transacción de reversa apunta a la original.
        builder.HasOne<Transaccion>()
            .WithMany()
            .HasForeignKey(t => t.TransaccionOrigenId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(t => t.Fecha);
        builder.HasIndex(t => new { t.FondoId, t.Fecha });
        builder.HasIndex(t => t.OfrendaEspecialId);
    }
}
