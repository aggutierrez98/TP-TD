[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("tasas", "notificaciones")]
    [string]$Tarea,
    [string]$NombreTarea
)

# Elimina la tarea registrada por registrar-tarea.ps1 (tambien las registradas con los scripts de
# los proyectos anteriores, que usaban los mismos nombres).
# No falla si la tarea no existe.
#
# Uso:
#   .\eliminar-tarea.ps1 -Tarea tasas
#   .\eliminar-tarea.ps1 -Tarea notificaciones

$ErrorActionPreference = "Stop"

if (-not $NombreTarea) {
    $NombreTarea = if ($Tarea -eq "tasas") { "FINAZAPP-ActualizadorTasas" } else { "FINAZAPP-GeneradorNotificaciones" }
}

# Con "Stop", el stderr de schtasks cuando la tarea no existe se volveria una excepcion.
$ErrorActionPreference = "Continue"
& schtasks.exe /Query /TN $NombreTarea *> $null
$existe = ($LASTEXITCODE -eq 0)
$ErrorActionPreference = "Stop"

if (-not $existe) {
    Write-Host "La tarea '$NombreTarea' no existe. No hay nada que eliminar."
    exit 0
}

& schtasks.exe /Delete /TN $NombreTarea /F
if ($LASTEXITCODE -ne 0) {
    throw "schtasks /Delete fallo con codigo $LASTEXITCODE."
}

Write-Host "Tarea '$NombreTarea' eliminada."
