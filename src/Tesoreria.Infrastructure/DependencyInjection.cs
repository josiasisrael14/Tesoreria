using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tesoreria.Application.Abstracciones.Email;
using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Infrastructure.Email;
using Tesoreria.Infrastructure.Identidad;
using Tesoreria.Infrastructure.Persistencia;
using Tesoreria.Infrastructure.Repositorios;

namespace Tesoreria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TesoreriaDb")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión 'TesoreriaDb' en appsettings.json (ConnectionStrings:TesoreriaDb).");

        services.AddDbContext<TesoreriaDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(TesoreriaDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFondoRepository, FondoRepository>();
        services.AddScoped<IMiembroRepository, MiembroRepository>();
        services.AddScoped<ICultoRepository, CultoRepository>();
        services.AddScoped<ITransaccionRepository, TransaccionRepository>();
        services.AddScoped<IOfrendaEspecialRepository, OfrendaEspecialRepository>();
        services.AddScoped<ICompromisoOfrendaRepository, CompromisoOfrendaRepository>();

        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();

        AgregarIdentidad(services, configuration);

        return services;
    }

    /// <summary>
    /// Login y usuarios con ASP.NET Core Identity + JWT. Separado en su propio método
    /// porque es un bloque de configuración grande y distinto en espíritu al resto
    /// (repositorios propios) de este archivo.
    /// </summary>
    private static void AgregarIdentidad(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<AdminInicialSettings>(configuration.GetSection("AdminInicial"));

        services.AddIdentityCore<ApplicationUser>(opciones =>
            {
                // Políticas de contraseña pensadas para un sistema chico de una sola
                // iglesia, no para una app masiva: seguras pero sin exigir de más.
                opciones.Password.RequiredLength = 6;
                opciones.Password.RequireDigit = true;
                opciones.Password.RequireLowercase = true;
                opciones.Password.RequireUppercase = false;
                opciones.Password.RequireNonAlphanumeric = false;
                opciones.User.RequireUniqueEmail = true;
            })
            .AddErrorDescriber<IdentityErrorDescriberEnEspanol>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TesoreriaDbContext>()
            // AddIdentityCore (a diferencia de AddIdentity) NO registra esto solo — y sin esto,
            // GeneratePasswordResetTokenAsync (usado tanto por "Restablecer contraseña" del admin
            // como por el enlace de "olvidé mi contraseña") revienta con una excepción no manejada
            // apenas se lo llama, porque no encuentra ningún generador de tokens registrado.
            .AddDefaultTokenProviders();

        // El enlace de "olvidé mi contraseña" usa este mismo token (vía GeneratePasswordResetTokenAsync);
        // 2 horas es suficiente para revisar el correo sin dejar el enlace activo demasiado tiempo.
        services.Configure<DataProtectionTokenProviderOptions>(opciones =>
            opciones.TokenLifespan = TimeSpan.FromHours(2));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IGeneradorTokenJwt, GeneradorTokenJwt>();
    }
}
