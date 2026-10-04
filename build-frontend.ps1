<#
.SYNOPSIS
    Compila el Angular real y lo copia a wwwroot/ del backend, para que
    "dotnet run" en Tesoreria.Api levante todo el sistema en un solo proceso.

.EXAMPLE
    .\build-frontend.ps1
#>

$ErrorActionPreference = "Stop"

$raiz = $PSScriptRoot
$clienteDir = Join-Path $raiz "client\tesoreria-app"
$wwwrootDir = Join-Path $raiz "src\Tesoreria.Api\wwwroot"
$distDir = Join-Path $clienteDir "dist\tesoreria-app\browser"

Write-Host "== 1/3: Instalando dependencias de Angular ==" -ForegroundColor Cyan
Push-Location $clienteDir
try {
    # Siempre se corre (no solo si falta node_modules): si package.json cambió
    # y agregó una dependencia nueva, "npm install" es la única forma de que
    # se detecte. Cuando ya está todo instalado, npm no hace nada y es rápido.
    npm install
    if ($LASTEXITCODE -ne 0) { throw "npm install falló." }

    Write-Host "== 2/3: Compilando Angular (produccion) ==" -ForegroundColor Cyan
    npm run build -- --configuration production
    if ($LASTEXITCODE -ne 0) { throw "ng build falló." }
}
finally {
    Pop-Location
}

if (-not (Test-Path $distDir)) {
    throw "No se encontró la carpeta de salida esperada: $distDir. Revisa el 'outputPath' en angular.json si lo cambiaste."
}

Write-Host "== 3/3: Copiando el build a wwwroot/ del backend ==" -ForegroundColor Cyan
if (Test-Path $wwwrootDir) {
    Remove-Item -Path (Join-Path $wwwrootDir "*") -Recurse -Force
}
else {
    New-Item -ItemType Directory -Path $wwwrootDir | Out-Null
}

Copy-Item -Path (Join-Path $distDir "*") -Destination $wwwrootDir -Recurse -Force

Write-Host ""
Write-Host "Listo. Ahora puedes correr:  dotnet run --project src\Tesoreria.Api" -ForegroundColor Green
