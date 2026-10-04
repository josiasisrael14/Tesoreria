using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Tesoreria.Application;
using Tesoreria.Application.Abstracciones.Validacion;
using Tesoreria.Domain.Excepciones;
using Tesoreria.Infrastructure;
using Tesoreria.Infrastructure.Identidad;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(opciones =>
    // Por defecto todo endpoint requiere estar autenticado; los que no (login) se
    // marcan explícitamente con [AllowAnonymous] — más seguro que acordarse de poner
    // [Authorize] en cada controlador nuevo.
    opciones.Filters.Add(new AuthorizeFilter()))
    .AddJsonOptions(opciones =>
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// Las dos capas que armamos: casos de uso (Mediator, Commands/Queries) e
// infraestructura (EF Core + SQL Server, repositorios, Identity).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la sección 'Jwt' en appsettings.json.");

builder.Services.AddAuthentication(opciones =>
    {
        opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Emisor,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audiencia,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.ClaveSecreta)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Traduce las excepciones de negocio a códigos HTTP entendibles para el frontend,
// en vez de que cada controlador repita el mismo try/catch.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var excepcion = feature?.Error;

        context.Response.ContentType = "application/json";

        switch (excepcion)
        {
            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    mensaje = "Uno o más valores no son válidos.",
                    errores = validationException.Errores
                });
                break;

            case DomainException domainException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { mensaje = domainException.Message });
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var esDesarrollo = app.Environment.IsDevelopment();
                await context.Response.WriteAsJsonAsync(new
                {
                    mensaje = "Ocurrió un error inesperado.",
                    detalle = esDesarrollo ? excepcion?.ToString() : null
                });
                break;
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

// Sirve el build de Angular (dist/ copiado a wwwroot/) y las rutas de la API.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Cualquier ruta que no sea /api/* y no matchee un archivo estático se resuelve
// con index.html, para que el router de Angular funcione al refrescar el navegador
// en una ruta como /transacciones o /reportes.
app.MapFallbackToFile("index.html");

// Crea los roles y, si todavía no hay ningún usuario, el administrador inicial
// (los datos salen de appsettings.json → "AdminInicial").
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.EjecutarAsync(scope.ServiceProvider);
}

app.Run();
