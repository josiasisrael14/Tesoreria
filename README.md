# Tesorería — Sistema para una iglesia local

.NET 10 + Angular en un solo proyecto (Entity Framework Core, sin Dapper), patrón Mediator
hecho a mano, SQL Server. Cubre la Fase 1 del roadmap: Fondos, Cultos, Transacciones
(registrar ingreso/egreso, listar, reversar) y el reporte de balance por fondo.

## Antes de arrancar — un aviso honesto

Este proyecto se armó en un entorno sin acceso a NuGet (el registro de paquetes de .NET),
así que **el Domain y el Application se compilaron y verificaron de verdad** (`dotnet build`,
0 errores) porque no dependen de ningún paquete externo. El **Angular también se compiló
de verdad** (`ng build`, 0 errores). Pero **Infrastructure y Api no se pudieron compilar
en ese entorno** porque dependen de los paquetes de Entity Framework Core, que solo se
pueden descargar con internet normal — el primer `dotnet restore` que corras en tu laptop
los va a descargar y ahí vas a ver si compilan. El código está escrito con cuidado contra
la API estable de EF Core 10, pero avísame si `dotnet build` te tira algún error ahí —
lo corregimos en el momento.

## Estructura

```
Tesoreria.slnx
src/
  Tesoreria.Domain/           Entidades: Transaccion, Fondo, Miembro, Culto
  Tesoreria.Application/      Commands/Queries (Mediator hecho a mano) — sin paquetes externos
  Tesoreria.Infrastructure/   EF Core: DbContext, configuraciones, repositorios
  Tesoreria.Api/               Controllers, Program.cs, wwwroot/ (build de Angular)
client/
  tesoreria-app/                Angular (standalone components + signals)
build-frontend.ps1             Compila Angular y lo copia a wwwroot/
```

## 1. Requisitos

- .NET 10 SDK (ya lo tienes: `dotnet --version` → 10.0.302)
- Node.js + npm (ya lo tienes)
- Angular CLI (`npm install -g @angular/cli`, ya lo tienes)
- SQL Server. La cadena de conexión en `src/Tesoreria.Api/appsettings.json` apunta a
  **LocalDB** por defecto (`(localdb)\MSSQLLocalDB`), que viene con Visual Studio. Si no
  tienes LocalDB instalado, las opciones son:
  - Instalar **SQL Server Express** (gratis, https://www.microsoft.com/sql-server/sql-server-downloads)
    y cambiar la cadena de conexión a `Server=localhost\SQLEXPRESS;...`
  - O instalar solo **LocalDB** (parte del "SQL Server Express LocalDB" installer, más liviano)
  - Si ya tienes un SQL Server corriendo en otro lado, solo cambia la cadena de conexión.

## 2. Restaurar paquetes

```
dotnet restore
```

Esto descarga Entity Framework Core y todo lo demás. Si te da un error de paquete no
encontrado, dime el mensaje exacto y lo resolvemos (puede ser una versión que ya no exista
para cuando corras esto — le puse `10.0.0` a los paquetes de EF Core, ajustable).

## 3. Crear la base de datos (migraciones)

Primera vez, instala la herramienta de EF Core:

```
dotnet tool install --global dotnet-ef
```

Luego, desde la raíz del proyecto:

```
dotnet ef migrations add InicialCreate --project src\Tesoreria.Infrastructure --startup-project src\Tesoreria.Api
dotnet ef database update --project src\Tesoreria.Infrastructure --startup-project src\Tesoreria.Api
```

Esto crea las tablas (Fondos, Miembros, Cultos, Transacciones) en la base de datos
`TesoreriaIglesia`.

## 4. Correr en modo desarrollo (dos procesos, con recarga en caliente)

Terminal 1 — backend:
```
dotnet run --project src\Tesoreria.Api
```

Terminal 2 — frontend (con recarga automática al guardar):
```
cd client\tesoreria-app
npm start
```

Abre `http://localhost:4200` — el Angular en modo desarrollo, con las llamadas a `/api`
redirigidas automáticamente al backend (`proxy.conf.json`).

## 5. Correr como un solo proceso (lo que pediste: un solo `dotnet run`)

```
.\build-frontend.ps1
dotnet run --project src\Tesoreria.Api
```

El script compila Angular para producción y copia el resultado a `wwwroot/`. Abre la URL
que te muestre la consola (por defecto `http://localhost:5255`) — ahí está todo el sistema,
backend y frontend, en un solo proceso.

Cada vez que cambies algo en el Angular, vuelve a correr `build-frontend.ps1` antes de
`dotnet run` para que el cambio se refleje.

## Qué incluye esta primera entrega (Fase 1 del roadmap)

- **Fondos**: crear, listar, desactivar.
- **Cultos**: crear, listar.
- **Miembros**: crear, listar (para asociar donantes a una ofrenda).
- **Transacciones**: registrar ingreso (pantalla de "registro rápido"), registrar egreso,
  listar con filtro por fondo, **reversar** (nunca se edita ni se borra un monto ya
  guardado — se crea la transacción contraria, como quedamos en la guía de arquitectura).
- **Reporte**: balance por fondo en un rango de fechas (con EF Core, `GroupBy` de LINQ —
  ya no usa Dapper).

## Qué falta (Fases 2 a 4, para cuando quieras seguir)

- Recibos/comprobantes imprimibles por donante.
- Dashboard con comparativos mes a mes.
- Login con roles (Tesorero / Pastor / Auditor) — por ahora `usuarioRegistroId` está
  fijo en `1` en el frontend (marcado con `TODO` en el código) porque no hay
  autenticación todavía.
- Exportar reportes a Excel/PDF.

## Sobre MediatR / Mediator

Se implementó el patrón Mediator **a mano**, sin el paquete MediatR (que desde 2025
requiere licencia comercial) ni el paquete gratuito "Mediator" — para no depender de
ningún paquete de terceros en la capa de Application. Está en
`src/Tesoreria.Application/Abstracciones/Mediator/`. Si en algún momento prefieres usar
una librería en su lugar, el cambio es acotado a esa carpeta.
