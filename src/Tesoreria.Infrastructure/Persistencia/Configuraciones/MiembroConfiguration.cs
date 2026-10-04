using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class MiembroConfiguration : IEntityTypeConfiguration<Miembro>
{
    public void Configure(EntityTypeBuilder<Miembro> builder)
    {
        builder.ToTable("Miembros");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombres).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Apellidos).IsRequired().HasMaxLength(100);
        builder.Property(m => m.DocumentoIdentidad).HasMaxLength(30);
        builder.Property(m => m.Telefono).HasMaxLength(30);
        builder.Property(m => m.Email).HasMaxLength(150);

        builder.Ignore(m => m.NombreCompleto);
    }
}
