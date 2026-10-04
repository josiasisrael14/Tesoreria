using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Identidad;

namespace Tesoreria.Infrastructure.Persistencia;

public class TesoreriaDbContext : IdentityDbContext<ApplicationUser>
{
    public TesoreriaDbContext(DbContextOptions<TesoreriaDbContext> options) : base(options) { }

    public DbSet<Fondo> Fondos => Set<Fondo>();
    public DbSet<Miembro> Miembros => Set<Miembro>();
    public DbSet<Culto> Cultos => Set<Culto>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<OfrendaEspecial> OfrendasEspeciales => Set<OfrendaEspecial>();
    public DbSet<CompromisoOfrenda> CompromisosOfrenda => Set<CompromisoOfrenda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Con IdentityDbContext hay que llamar primero a base.OnModelCreating (configura
        // las tablas de Identity: AspNetUsers, AspNetRoles, etc.) y recién después aplicar
        // nuestras propias configuraciones — al revés de como estaba antes de agregar login.
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TesoreriaDbContext).Assembly);
    }
}
