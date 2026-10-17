param(
    [string]$RepoRoot = ".",
    # Catalogos que lee el seed de script_bd.sql: espanol (idioma base) e ingles.
    [string[]]$RelativeJsonPaths = @("translation-seeds\base.es.json", "translation-seeds\base.in.json"),
    [Parameter(Mandatory = $true)]
    [string]$OutputFile
)

$ErrorActionPreference = "Stop"

# GetFullPath y no (Resolve-Path ...).Path: Resolve-Path no normaliza los
# saltos ".." y, sobre rutas UNC (repo abierto desde \\wsl.localhost\... o un
# share de red), devuelve la forma calificada por proveedor
# ("Microsoft.PowerShell.Core\FileSystem::\\host\..."), que herramientas como
# vstest.console rechazan.
$resolvedRepoRoot = [System.IO.Path]::GetFullPath((Resolve-Path -LiteralPath $RepoRoot).ProviderPath).TrimEnd('\')
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

# SQL Server lee la copia publica con SINGLE_NCLOB. Convertir explicitamente el
# JSON fuente (UTF-8) a UTF-16LE evita que sus bytes pasen por VARCHAR/code pages
# antes de llegar a OPENJSON.
$utf8Strict = New-Object System.Text.UTF8Encoding($false, $true)
$utf16LittleEndian = New-Object System.Text.UnicodeEncoding($false, $true)
foreach ($relativeJsonPath in $RelativeJsonPaths) {
    $resolvedJsonPath = [System.IO.Path]::GetFullPath((Resolve-Path -LiteralPath (Join-Path $resolvedRepoRoot $relativeJsonPath)).ProviderPath)
    $json = [System.IO.File]::ReadAllText($resolvedJsonPath, $utf8Strict)
    [System.IO.File]::WriteAllText((Join-Path $stagingDirectory (Split-Path -Leaf $resolvedJsonPath)), $json, $utf16LittleEndian)
}

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
