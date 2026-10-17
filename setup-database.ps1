[CmdletBinding()]
param(
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$DatabaseName = "TpCatedraIDS",
    [switch]$Force
)

# Crea la base del sistema ejecutando script_bd.sql SIN necesitar SQL Server
# Management Studio. Existe porque el sqlcmd ODBC 15.x (el que instala SQL Server
# 2019/Express) deja de procesar el script en silencio, con exit code 0, al
# encontrar la linea "!!" del seed de traducciones: la BD queda creada pero sin
# idioma ni traducciones y la app muere en el login con Error.Language.DefaultNotFound.
# Este wrapper ejecuta ese helper desde PowerShell, quita la linea "!!" de una
# copia temporal del script y verifica al final que las traducciones quedaron cargadas.
# El flujo por SSMS en SQLCMD Mode sigue funcionando igual que siempre.

$ErrorActionPreference = "Stop"
$RepoRoot = $PSScriptRoot

$scriptPath = Join-Path $RepoRoot "script_bd.sql"
if (-not (Test-Path -LiteralPath $scriptPath)) {
    throw "No se encontro script_bd.sql en $RepoRoot."
}

$seedHelper = Join-Path $RepoRoot ".agents\skills\translation-base-seed-sync\scripts\emit-seed-json-sqlcmd-path.ps1"
if (-not (Test-Path -LiteralPath $seedHelper)) {
    throw "No se encontro el helper de seed i18n: $seedHelper"
}

# script_bd.sql BORRA y recrea la base: si ya existe, exigir confirmacion explicita.
$existe = (& sqlcmd -S $SqlServer -E -h -1 -W -Q "SET NOCOUNT ON; SELECT CASE WHEN DB_ID(N'$DatabaseName') IS NULL THEN 0 ELSE 1 END") | Select-Object -First 1
if ($LASTEXITCODE -ne 0) {
    throw "No se pudo consultar el servidor $SqlServer (sqlcmd exit $LASTEXITCODE)."
}

if ("$existe".Trim() -eq "1" -and -not $Force) {
    throw "La base '$DatabaseName' ya existe en $SqlServer y este script la BORRA y recrea desde cero. Ejecute con -Force para confirmar."
}

# El helper copia translation-seeds\base.es.json y base.in.json a una carpeta publica legible por
# la cuenta de servicio de SQL Server (lo que haria la linea "!!" del script).
& powershell -NoProfile -ExecutionPolicy Bypass -File $seedHelper -RepoRoot $RepoRoot -OutputFile "NUL"
if ($LASTEXITCODE -ne 0) {
    throw "emit-seed-json-sqlcmd-path.ps1 finalizo con codigo $LASTEXITCODE."
}

$tempScript = Join-Path ([System.IO.Path]::GetTempPath()) ("tpids-setup-db-{0}.sql" -f ([System.Guid]::NewGuid().ToString("N")))

try {
    $content = Get-Content -LiteralPath $scriptPath -Raw

    if ($DatabaseName -ne "TpCatedraIDS") {
        $content = $content.Replace("TpCatedraIDS", $DatabaseName)
    }

    $escapedRepoRoot = $RepoRoot.Replace('"', '""')
    $content = $content.Replace(':setvar RepoRoot "."', (':setvar RepoRoot "{0}"' -f $escapedRepoRoot))
    $content = [regex]::Replace(
        $content,
        '(?m)^\s*!!.*$',
        '-- (linea !! removida: setup-database.ps1 ejecuta el helper de seed directamente)')

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

# Verificacion post-instalacion: el modo de falla historico era una BD "creada bien"
# pero sin idioma default ni traducciones.
$chequeo = (& sqlcmd -S $SqlServer -E -d $DatabaseName -h -1 -W -Q "SET NOCOUNT ON; SELECT CONVERT(varchar(10), (SELECT COUNT(*) FROM dbo.Idioma WHERE [Default] = 1)) + '|' + CONVERT(varchar(10), (SELECT COUNT(*) FROM dbo.Traduccion))") | Select-Object -First 1
$partes = "$chequeo".Trim().Split('|')
if ($partes.Length -ne 2 -or [int]$partes[0] -lt 1 -or [int]$partes[1] -lt 1) {
    throw "La base '$DatabaseName' quedo sin idioma default o sin traducciones (chequeo: '$chequeo'). Revise el seed i18n."
}

Write-Output ("Base '{0}' creada en {1}: idioma default OK, {2} traducciones cargadas." -f $DatabaseName, $SqlServer, $partes[1])
