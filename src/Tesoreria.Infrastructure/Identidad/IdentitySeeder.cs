using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tesoreria.Application.Usuarios;

namespace Tesoreria.Infrastructure.Identidad;

/// <summary>
/// Crea los roles del sistema y, si todavía no existe ningún usuario, el primer
/// administrador (con los datos de appsettings.json → "AdminInicial"). Así el
/// sistema arranca usable sin tener que insertar nada a mano en la base de datos.
/// Se corre una vez al iniciar la Api (ver Program.cs).
/// </summary>
public static class IdentitySeeder
{
    public static async Task EjecutarAsync(IServiceProvider servicios)
    {
        var userManager = servicios.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
        var adminInicial = servicios.GetRequiredService<IOptions<AdminInicialSettings>>().Value;

        foreach (var rol in Roles.Todos)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // Solo se crea el admin inicial si la base todavía no tiene ningún usuario —
        // así este código es seguro de dejar corriendo siempre al iniciar la Api,
        // no solo "la primera vez".
        if (userManager.Users.Any())
            return;

        var admin = new ApplicationUser
        {
            UserName = adminInicial.Email,
            Email = adminInicial.Email,
            NombreCompleto = adminInicial.NombreCompleto,
            Activo = true,
            EmailConfirmed = true
        };

        var resultado = await userManager.CreateAsync(admin, adminInicial.Contrasena);
        if (resultado.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.Administrador);
    }
}
