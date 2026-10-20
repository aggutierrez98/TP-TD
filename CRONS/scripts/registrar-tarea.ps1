[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("tasas", "notificaciones")]
    [string]$Tarea,
    [ValidateSet("Debug", "Release")]
    [string]$Configuracion = "Release",
    [string]$RutaExe,
    [string]$NombreTarea
)

# Registra en el Programador de tareas de Windows la tarea indicada, que ejecuta
# CRONS.exe <tarea> con el usuario actual y sin pedir contrasena (solo corre con la sesion
# iniciada). Es idempotente: reemplaza la tarea si ya existe.
#
# - tasas: cada hora (FINAZAPP-ActualizadorTasas). schtasks no permite "no iniciar otra
#   instancia" por linea de comandos; no hace falta: el tope de 60 minutos se revalida dentro de
#   la transaccion y una segunda corrida superpuesta no consulta la API.
# - notificaciones: todos los dias a las 07:00 y al iniciar sesion (FINAZAPP-GeneradorNotificaciones);
#   FINAZAPP puede estar cerrada.
#   - StartWhenAvailable: si el equipo estaba apagado a las 07:00, corre al estar disponible.
#   - Disparador de inicio de sesion: cubre el caso de encender el equipo despues de las 07:00.
#     El BLL solo procesa la jornada actual y no duplica avisos, asi que repetir la corrida es seguro.
#   - MultipleInstances IgnoreNew: no superpone instancias (la base sigue siendo la autoridad).
#
# Uso:
#   .\registrar-tarea.ps1 -Tarea tasas                  # bin\Release\CRONS.exe
#   .\registrar-tarea.ps1 -Tarea notificaciones -Configuracion Debug
#   .\registrar-tarea.ps1 -Tarea tasas -RutaExe C:\ruta\CRONS.exe

$ErrorActionPreference = "Stop"

if (-not $NombreTarea) {
    $NombreTarea = if ($Tarea -eq "tasas") { "FINAZAPP-ActualizadorTasas" } else { "FINAZAPP-GeneradorNotificaciones" }
}

if (-not $RutaExe) {
    $RutaExe = Join-Path $PSScriptRoot "..\bin\$Configuracion\CRONS.exe"
}

if (-not (Test-Path -LiteralPath $RutaExe)) {
    throw "No se encontro el ejecutable en '$RutaExe'. Compile TpIDS.sln o indique -RutaExe."
}

$RutaExe = (Resolve-Path -LiteralPath $RutaExe).Path

if ($Tarea -eq "tasas") {
    # El valor de /TR lleva comillas propias (la ruta puede tener espacios): Windows PowerShell 5.1
    # no las escapa al invocar un ejecutable nativo, por eso van con barra invertida.
    & schtasks.exe /Create /TN $NombreTarea /TR "\`"$RutaExe\`" tasas" /SC HOURLY /MO 1 /RL LIMITED /F
    if ($LASTEXITCODE -ne 0) {
        throw "schtasks /Create fallo con codigo $LASTEXITCODE."
    }

    Write-Host "Tarea '$NombreTarea' registrada: cada hora ejecuta $RutaExe tasas."
    return
}

$Carpeta = Split-Path -Parent $RutaExe
$Usuario = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name

$accion = New-ScheduledTaskAction -Execute $RutaExe -Argument "notificaciones" -WorkingDirectory $Carpeta
$disparadores = @(
    (New-ScheduledTaskTrigger -Daily -At "07:00"),
    (New-ScheduledTaskTrigger -AtLogOn -User $Usuario)
)
$configuracionTarea = New-ScheduledTaskSettingsSet `
    -StartWhenAvailable `
    -MultipleInstances IgnoreNew `
    -AllowStartIfOnBatteries `
    -DontStopIfGoingOnBatteries `
    -ExecutionTimeLimit (New-TimeSpan -Hours 4)
$principal = New-ScheduledTaskPrincipal -UserId $Usuario -LogonType Interactive -RunLevel Limited

Register-ScheduledTask -TaskName $NombreTarea -Action $accion -Trigger $disparadores `
    -Settings $configuracionTarea -Principal $principal -Force | Out-Null

Write-Host "Tarea '$NombreTarea' registrada: todos los dias a las 07:00 y al iniciar sesion ejecuta $RutaExe notificaciones (usuario $Usuario)."
