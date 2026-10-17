[CmdletBinding()]
param(
    [string]$NombreTarea = "FINAZAPP-ActualizadorTasas"
)

# Elimina la tarea horaria registrada por registrar-tarea.ps1.
# No falla si la tarea no existe.
#
# Uso:
#   .\eliminar-tarea.ps1

$ErrorActionPreference = "Stop"

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
