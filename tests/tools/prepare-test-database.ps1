[CmdletBinding()]
param(
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$DatabaseName = "TpCatedraIDS_Test",
    [string]$RepoRoot
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
}
else {
    $RepoRoot = (Resolve-Path $RepoRoot).Path
}

if ([string]::Equals($DatabaseName, "TpCatedraIDS", [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "La base productiva/de desarrollo TpCatedraIDS no puede usarse para tests."
}

if (-not $DatabaseName.StartsWith("TpCatedraIDS_Test", [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "La base de tests debe empezar con TpCatedraIDS_Test."
}

$scriptPath = Join-Path $RepoRoot "script_bd.sql"
if (-not (Test-Path -LiteralPath $scriptPath)) {
    throw "No se encontro script_bd.sql en $RepoRoot."
}

$tempScript = Join-Path ([System.IO.Path]::GetTempPath()) ("tpids-test-db-{0}.sql" -f ([System.Guid]::NewGuid().ToString("N")))

try {
    $content = Get-Content -LiteralPath $scriptPath -Raw
    $content = $content.Replace("TpCatedraIDS", $DatabaseName)
    $escapedRepoRoot = $RepoRoot.Replace('"', '""')
    $content = $content.Replace(':setvar RepoRoot "."', (':setvar RepoRoot "{0}"' -f $escapedRepoRoot))

    # El sqlcmd ODBC 15.x deja de procesar el resto del archivo (con exit code 0!)
    # al encontrar una linea "!!". El helper que esa linea invoca se ejecuta aca
    # directamente y la linea se quita del script temporal.
    $seedHelper = Join-Path $RepoRoot ".agents\skills\translation-base-seed-sync\scripts\emit-seed-json-sqlcmd-path.ps1"
    if (-not (Test-Path -LiteralPath $seedHelper)) {
        throw "No se encontro el helper de seed i18n: $seedHelper"
    }

    & powershell -NoProfile -ExecutionPolicy Bypass -File $seedHelper -RepoRoot $RepoRoot -OutputFile "NUL"
    if ($LASTEXITCODE -ne 0) {
        throw "emit-seed-json-sqlcmd-path.ps1 finalizo con codigo $LASTEXITCODE."
    }

    $content = [regex]::Replace(
        $content,
        '(?m)^\s*!!.*$',
        '-- (linea !! removida: prepare-test-database.ps1 ejecuta el helper de seed directamente)')

    Set-Content -LiteralPath $tempScript -Value $content -Encoding UTF8

    & sqlcmd -S $SqlServer -E -b -i $tempScript
    if ($LASTEXITCODE -ne 0) {
        throw "sqlcmd finalizo con codigo $LASTEXITCODE."
    }
}
finally {
    if (Test-Path -LiteralPath $tempScript) {
        Remove-Item -LiteralPath $tempScript -Force
    }
}
