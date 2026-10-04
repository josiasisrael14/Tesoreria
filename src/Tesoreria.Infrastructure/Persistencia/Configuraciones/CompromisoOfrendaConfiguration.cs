using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class CompromisoOfrendaConfiguration : IEntityTypeConfiguration<CompromisoOfrenda>
{
    public void Configure(EntityTypeBuilder<CompromisoOfrenda> builder)
    {
        builder.ToTable("CompromisosOfrenda");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.MontoComprometido).HasColumnType("decimal(18,2)");

        builder.HasOne(c => c.OfrendaEspecial)
            .WithMany()
            .HasForeignKey(c => c.OfrendaEspecialId)
            .OnDelete(DeleteBehavior.Restrict); // igual que en Transaccion: nunca se borra en cascada

        builder.HasOne(c => c.Miembro)
            .WithMany()
            .HasForeignKey(c => c.MiembroId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un miembro tiene un solo compromiso por campaña (se edita, no se duplica).
        builder.HasIndex(c => new { c.OfrendaEspecialId, c.MiembroId }).IsUnique();
    }
}
