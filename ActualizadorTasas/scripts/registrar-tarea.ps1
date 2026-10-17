[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuracion = "Release",
    [string]$RutaExe,
    [string]$NombreTarea = "FINAZAPP-ActualizadorTasas"
)

# Registra en el Programador de tareas de Windows una tarea que ejecuta
# ActualizadorTasas.exe cada hora, con el usuario actual y sin pedir contrasena
# (solo corre con la sesion iniciada). Es idempotente: /F recrea la tarea si existe.
#
# Uso:
#   .\registrar-tarea.ps1                              # bin\Release\ActualizadorTasas.exe
#   .\registrar-tarea.ps1 -Configuracion Debug
#   .\registrar-tarea.ps1 -RutaExe C:\ruta\ActualizadorTasas.exe
#
# Limitacion: schtasks no permite "no iniciar otra instancia" al crear la tarea por linea de
# comandos. No hace falta: el tope de 60 minutos se revalida dentro de la transaccion y una
# segunda corrida superpuesta no consulta la API.

$ErrorActionPreference = "Stop"

if (-not $RutaExe) {
    $RutaExe = Join-Path $PSScriptRoot "..\bin\$Configuracion\ActualizadorTasas.exe"
}

if (-not (Test-Path -LiteralPath $RutaExe)) {
    throw "No se encontro el ejecutable en '$RutaExe'. Compile TpIDS.sln o indique -RutaExe."
}

$RutaExe = (Resolve-Path -LiteralPath $RutaExe).Path

& schtasks.exe /Create /TN $NombreTarea /TR "`"$RutaExe`"" /SC HOURLY /MO 1 /RL LIMITED /F
if ($LASTEXITCODE -ne 0) {
    throw "schtasks /Create fallo con codigo $LASTEXITCODE."
}

Write-Host "Tarea '$NombreTarea' registrada: cada hora ejecuta $RutaExe."
