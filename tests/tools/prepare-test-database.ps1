[CmdletBinding()]
param(
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$DatabaseName = "TpCatedraIDS_Test",
    [string]$RepoRoot
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    # GetFullPath y no (Resolve-Path ...).Path: Resolve-Path no normaliza los
    # saltos ".." y, sobre rutas UNC (repo abierto desde \\wsl.localhost\... o un
    # share de red), devuelve la forma calificada por proveedor
    # ("Microsoft.PowerShell.Core\FileSystem::\\host\..."), que herramientas como
    # vstest.console rechazan.
    $RepoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\.."))
}
else {
    $RepoRoot = [System.IO.Path]::GetFullPath((Resolve-Path -LiteralPath $RepoRoot).ProviderPath)
}

$RepoRoot = $RepoRoot.TrimEnd('\')

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
