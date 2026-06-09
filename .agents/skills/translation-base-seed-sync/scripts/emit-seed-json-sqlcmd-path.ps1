param(
    [string]$RepoRoot = ".",
    [string]$RelativeJsonPath = "translation-seeds\base.es.json",
    [Parameter(Mandatory = $true)]
    [string]$OutputFile
)

$ErrorActionPreference = "Stop"

$resolvedRepoRoot = (Resolve-Path $RepoRoot).Path
$resolvedJsonPath = (Resolve-Path (Join-Path $resolvedRepoRoot $RelativeJsonPath)).Path
$outputDirectory = Split-Path -Parent $OutputFile
$stagingDirectory = Join-Path $env:PUBLIC "Documents\TpIDS\SqlSeed"
$stagingJsonPath = Join-Path $stagingDirectory "base.es.json"
$shouldWriteOutputFile = -not [string]::Equals($OutputFile, "NUL", [System.StringComparison]::OrdinalIgnoreCase)

if ($shouldWriteOutputFile -and -not [string]::IsNullOrWhiteSpace($outputDirectory) -and -not (Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

if (-not (Test-Path $stagingDirectory)) {
    New-Item -ItemType Directory -Path $stagingDirectory -Force | Out-Null
}

Copy-Item -LiteralPath $resolvedJsonPath -Destination $stagingJsonPath -Force

# Expone una copia temporal en una ubicacion publica para evitar bloqueos de lectura
# cuando SQL Server corre bajo una cuenta de servicio sin acceso al perfil del usuario.
$grantTargets = @(
    '*S-1-1-0',      # Everyone
    '*S-1-5-6',      # Service
    '*S-1-5-18'      # Local System
)

foreach ($target in $grantTargets) {
    & icacls $stagingDirectory /grant ($target + ':(OI)(CI)RX') /T /C | Out-Null
}

if ($shouldWriteOutputFile) {
    $escapedPath = $stagingJsonPath.Replace('"', '""')
    Set-Content -Path $OutputFile -Value (':setvar SeedJsonPath "' + $escapedPath + '"') -Encoding ASCII
}
