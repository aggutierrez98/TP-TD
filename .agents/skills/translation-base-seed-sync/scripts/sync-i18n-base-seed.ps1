param()

$ErrorActionPreference = "Stop"

# GetFullPath y no (Resolve-Path ...).Path: Resolve-Path no normaliza los
# saltos ".." y, sobre rutas UNC (repo abierto desde \\wsl.localhost\... o un
# share de red), devuelve la forma calificada por proveedor
# ("Microsoft.PowerShell.Core\FileSystem::\\host\..."), que herramientas como
# vstest.console rechazan.
$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\..\..\..")).TrimEnd('\')
$uiPath = @(
    (Join-Path $repoRoot "UI"),
    (Join-Path $repoRoot "TpIDS")
) | Where-Object { Test-Path $_ } | Select-Object -First 1
$seedJsonPath = Join-Path $repoRoot "translation-seeds\base.es.json"
$sqlPath = Join-Path $repoRoot "script_bd.sql"
$markerStart = "-- BEGIN AUTO-GENERATED I18N BASE SEED"
$markerEnd = "-- END AUTO-GENERATED I18N BASE SEED"
$idiomaBaseNombre = "Espa$([char]0x00F1)ol"
$seedPathResolverScript = ".agents\skills\translation-base-seed-sync\scripts\emit-seed-json-sqlcmd-path.ps1"
$publicSeedJsonPath = "C:\Users\Public\Documents\TpIDS\SqlSeed\base.es.json"
$idiomaInglesNombre = "English"
$publicSeedInglesJsonPath = "C:\Users\Public\Documents\TpIDS\SqlSeed\base.in.json"

function Find-ControlText {
    param(
        [string]$ControlName,
        [string[]]$Contents
    )

    if ([string]::IsNullOrWhiteSpace($ControlName)) {
        foreach ($content in $Contents) {
            $match = [regex]::Match($content, '(?m)^\s*(?:this\.)?Text\s*=\s*"(?<text>[^"]*)";')
            if ($match.Success) {
                return $match.Groups["text"].Value
            }
        }

        return $null
    }

    foreach ($content in $Contents) {
        $pattern = '(?m)(?:this\.)?' + [regex]::Escape($ControlName) + '\.Text\s*=\s*"(?<text>[^"]*)";'
        $match = [regex]::Match($content, $pattern)
        if ($match.Success) {
            return $match.Groups["text"].Value
        }
    }

    return $null
}

function Escape-SqlLiteral {
    param([string]$Value)

    if ($null -eq $Value) {
        return ""
    }

    return $Value.Replace("'", "''")
}

function Get-ExistingItemsFromJsonFile {
    param([string]$Path)

    $items = @{}
    if (-not (Test-Path $Path)) {
        return $items
    }

    $raw = Get-Content -Raw -Path $Path
    if ([string]::IsNullOrWhiteSpace($raw)) {
        return $items
    }

    $data = ConvertFrom-Json -InputObject $raw
    foreach ($property in $data.PSObject.Properties) {
        $items[$property.Name] = [string]$property.Value
    }

    return $items
}

$resolvedTexts = @{}
$allKeys = New-Object 'System.Collections.Generic.HashSet[string]'
$jsonItems = Get-ExistingItemsFromJsonFile -Path $seedJsonPath
$codeSearchPaths = @(
    $uiPath,
    (Join-Path $repoRoot "BLL"),
    (Join-Path $repoRoot "SERVICIOS")
) | Where-Object { Test-Path $_ }

if ([string]::IsNullOrWhiteSpace($uiPath)) {
    throw "No se encontro la carpeta del proyecto UI para sincronizar las traducciones base."
}

$formFiles = Get-ChildItem -Path $uiPath -Filter "Form*.cs" | Where-Object { $_.Name -notlike "*.Designer.cs" }
foreach ($formFile in $formFiles) {
    $relatedFiles = @($formFile.FullName)
    $designerFile = Join-Path $formFile.DirectoryName ($formFile.BaseName + ".Designer.cs")
    if (Test-Path $designerFile) {
        $relatedFiles += $designerFile
    }

    $contents = $relatedFiles | ForEach-Object { Get-Content -Raw -Path $_ }

    foreach ($content in $contents) {
        foreach ($match in [regex]::Matches($content, '(?m)^\s*(?:(?:this\.)?(?<control>\w+)\.)?Tag\s*=\s*"(?<key>Form[^"]+)";')) {
            $key = $match.Groups["key"].Value
            $control = if ($match.Groups["control"].Success) { $match.Groups["control"].Value } else { $null }
            $null = $allKeys.Add($key)

            $text = Find-ControlText -ControlName $control -Contents $contents
            if (-not [string]::IsNullOrWhiteSpace($text)) {
                $resolvedTexts[$key] = $text
            }
        }

        foreach ($match in [regex]::Matches($content, '(?:ObtenerTexto(?:Actual)?|ObtenerTraduccion)\("(?<key>Form[^"]+)"\)')) {
            $null = $allKeys.Add($match.Groups["key"].Value)
        }
    }
}

$codeFiles = foreach ($path in $codeSearchPaths) {
    Get-ChildItem -Path $path -Filter "*.cs" -Recurse
}

foreach ($codeFile in $codeFiles) {
    $content = Get-Content -Raw -Path $codeFile.FullName

        # (?:Form|Dialog|Error)[A-Za-z0-9_]*\. y no (?:Form|Dialog|Error)\.: las keys
        # reales son del tipo "FormUserHistory.Value...", con el nombre del form
        # pegado al prefijo. Exigir el punto inmediato dejaba fuera del scan a
        # toda key emitida como literal suelto desde BLL/SERVICIOS.
        foreach ($match in [regex]::Matches($content, '"(?<key>(?:Form|Dialog|Error)[A-Za-z0-9_]*(?:\.[A-Za-z0-9_]+)+)"')) {
            $null = $allKeys.Add($match.Groups["key"].Value)
        }

    foreach ($match in [regex]::Matches($content, '\{\s*"(?<key>(?:Form|Dialog|Error)\.[^"]+)"\s*,\s*"(?<text>[^"]*)"\s*\}')) {
        $key = $match.Groups["key"].Value
        $text = $match.Groups["text"].Value
        $null = $allKeys.Add($key)

        if (-not [string]::IsNullOrWhiteSpace($text)) {
            $resolvedTexts[$key] = $text
        }
    }
}

$seedDir = Split-Path -Parent $seedJsonPath
if (-not (Test-Path $seedDir)) {
    New-Item -ItemType Directory -Path $seedDir | Out-Null
}

$keysToPersist = New-Object 'System.Collections.Generic.HashSet[string]'
foreach ($key in $jsonItems.Keys) {
    $null = $keysToPersist.Add($key)
}

foreach ($key in $allKeys) {
    $null = $keysToPersist.Add($key)
}

$items = @()
foreach ($key in (@($keysToPersist) | Sort-Object)) {
    $value = $null
    if ($jsonItems.ContainsKey($key) -and -not [string]::IsNullOrWhiteSpace($jsonItems[$key]) -and $jsonItems[$key] -ne $key) {
        $value = $jsonItems[$key]
    }
    elseif ($resolvedTexts.ContainsKey($key)) {
        $value = $resolvedTexts[$key]
    }
    else {
        $value = $key
    }

    $items += [pscustomobject]@{
        Key = $key
        Valor = $value
    }
}

$jsonOrdered = [ordered]@{}
foreach ($item in $items) {
    $jsonOrdered[$item.Key] = $item.Valor
}

$jsonOutput = $jsonOrdered | ConvertTo-Json -Depth 5
Set-Content -Path $seedJsonPath -Value $jsonOutput -Encoding UTF8

$sqlLines = New-Object System.Collections.Generic.List[string]
$sqlLines.Add($markerStart)
$sqlLines.Add("-- AUTO-GENERATED BY .agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1")
$sqlLines.Add("-- SOURCE OF TRUTH: translation-seeds/base.es.json")
$sqlLines.Add("-- Si ejecuta desde SSMS en SQLCMD Mode, cambie RepoRoot por la ruta absoluta del repo en su maquina.")
$sqlLines.Add("-- Con setup-database.ps1 deje RepoRoot = . : el wrapper lo reemplaza solo.")
$sqlLines.Add(':setvar RepoRoot "."')
$sqlLines.Add("-- Formas soportadas de ejecutar este script: SSMS 19/22 en SQLCMD Mode, o .\setup-database.ps1 desde la raiz del repo.")
$sqlLines.Add("-- OJO: NO ejecutarlo directo con sqlcmd ODBC 15.x: al llegar a la linea !! corta en silencio (exit 0) y la BD queda sin traducciones.")
$sqlLines.Add("-- El helper prepara una copia publica UTF-16LE del JSON para que OPENROWSET preserve Unicode y no dependa de permisos sobre el perfil del usuario.")
$sqlLines.Add('!! powershell -NoProfile -ExecutionPolicy Bypass -File "$(RepoRoot)\' + $seedPathResolverScript + '" -RepoRoot "$(RepoRoot)" -OutputFile "NUL"')
$sqlLines.Add("GO")
$sqlLines.Add("DECLARE @IdiomaBaseId INT;")
$idiomaBaseNombreSql = Escape-SqlLiteral $idiomaBaseNombre
$sqlLines.Add("INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'$idiomaBaseNombreSql', 1);")
$sqlLines.Add("SET @IdiomaBaseId = SCOPE_IDENTITY();")
$sqlLines.Add("")
$publicSeedJsonPathSql = Escape-SqlLiteral $publicSeedJsonPath
$sqlLines.Add("DECLARE @RutaTraduccionesBase NVARCHAR(4000) = N'$publicSeedJsonPathSql';")
$sqlLines.Add("DECLARE @TraduccionesBaseJson NVARCHAR(MAX);")
$sqlLines.Add("DECLARE @SqlCargaJson NVARCHAR(MAX);")
$sqlLines.Add("")
$sqlLines.Add("SET @SqlCargaJson = N'SELECT @JsonOut = BulkColumn")
$sqlLines.Add("FROM OPENROWSET(")
$sqlLines.Add("    BULK ''' + REPLACE(@RutaTraduccionesBase, '''', '''''') + ''',")
$sqlLines.Add("    SINGLE_NCLOB")
$sqlLines.Add(") AS JsonFile;';")
$sqlLines.Add("")
$sqlLines.Add("BEGIN TRY")
$sqlLines.Add("    EXEC sys.sp_executesql")
$sqlLines.Add("        @SqlCargaJson,")
$sqlLines.Add("        N'@JsonOut NVARCHAR(MAX) OUTPUT',")
$sqlLines.Add("        @JsonOut = @TraduccionesBaseJson OUTPUT;")
$sqlLines.Add("END TRY")
$sqlLines.Add("BEGIN CATCH")
$sqlLines.Add("    THROW 50000, 'No se pudo leer translation-seeds/base.es.json desde SQL Server. Verifique la ruta, los permisos del servicio y el acceso del servidor al archivo.', 1;")
$sqlLines.Add("END CATCH")
$sqlLines.Add("")
$sqlLines.Add("IF NULLIF(LTRIM(RTRIM(@TraduccionesBaseJson)), N'') IS NULL")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    THROW 50001, 'El archivo translation-seeds/base.es.json esta vacio o no pudo leerse correctamente.', 1;")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlLines.Add("DECLARE @Seed TABLE")
$sqlLines.Add("(")
$sqlLines.Add("    [Key] VARCHAR(150) NOT NULL PRIMARY KEY,")
$sqlLines.Add("    Valor NVARCHAR(300) NOT NULL")
$sqlLines.Add(");")
$sqlLines.Add("")
$sqlLines.Add("INSERT INTO @Seed ([Key], Valor)")
$sqlLines.Add("SELECT [key], TRY_CAST([value] AS NVARCHAR(300))")
$sqlLines.Add("FROM OPENJSON(@TraduccionesBaseJson)")
$sqlLines.Add("WHERE [type] IN (1, 2);")
$sqlLines.Add("")
$sqlLines.Add("IF NOT EXISTS (SELECT 1 FROM @Seed)")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    THROW 50002, 'El archivo translation-seeds/base.es.json no contiene traducciones validas.', 1;")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlLines.Add("IF EXISTS (SELECT 1 FROM @Seed WHERE [Key] IS NULL OR Valor IS NULL)")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    THROW 50003, 'El archivo translation-seeds/base.es.json contiene keys o valores incompatibles con el esquema esperado.', 1;")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlLines.Add("INSERT INTO dbo.Palabra ([Key])")
$sqlLines.Add("SELECT [Key]")
$sqlLines.Add("FROM @Seed")
$sqlLines.Add("ORDER BY [Key];")
$sqlLines.Add("")
$sqlLines.Add("INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor)")
$sqlLines.Add("SELECT p.Id, @IdiomaBaseId, s.Valor")
$sqlLines.Add("FROM @Seed s")
$sqlLines.Add("INNER JOIN dbo.Palabra p ON p.[Key] = s.[Key];")
$sqlLines.Add("")
$sqlLines.Add("-- Idioma ingles (no default) desde translation-seeds/base.in.json. Solo se cargan las keys que existen")
$sqlLines.Add("-- en el idioma base: base.es.json sigue siendo la fuente de verdad de las Palabras.")
$sqlLines.Add("DECLARE @IdiomaInglesId INT;")
$idiomaInglesNombreSql = Escape-SqlLiteral $idiomaInglesNombre
$sqlLines.Add("INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'$idiomaInglesNombreSql', 0);")
$sqlLines.Add("SET @IdiomaInglesId = SCOPE_IDENTITY();")
$sqlLines.Add("")
$publicSeedInglesJsonPathSql = Escape-SqlLiteral $publicSeedInglesJsonPath
$sqlLines.Add("DECLARE @RutaTraduccionesIngles NVARCHAR(4000) = N'$publicSeedInglesJsonPathSql';")
$sqlLines.Add("DECLARE @TraduccionesInglesJson NVARCHAR(MAX);")
$sqlLines.Add("")
$sqlLines.Add("SET @SqlCargaJson = N'SELECT @JsonOut = BulkColumn")
$sqlLines.Add("FROM OPENROWSET(")
$sqlLines.Add("    BULK ''' + REPLACE(@RutaTraduccionesIngles, '''', '''''') + ''',")
$sqlLines.Add("    SINGLE_NCLOB")
$sqlLines.Add(") AS JsonFile;';")
$sqlLines.Add("")
$sqlLines.Add("BEGIN TRY")
$sqlLines.Add("    EXEC sys.sp_executesql")
$sqlLines.Add("        @SqlCargaJson,")
$sqlLines.Add("        N'@JsonOut NVARCHAR(MAX) OUTPUT',")
$sqlLines.Add("        @JsonOut = @TraduccionesInglesJson OUTPUT;")
$sqlLines.Add("END TRY")
$sqlLines.Add("BEGIN CATCH")
$sqlLines.Add("    THROW 50004, 'No se pudo leer translation-seeds/base.in.json desde SQL Server. Verifique la ruta, los permisos del servicio y el acceso del servidor al archivo.', 1;")
$sqlLines.Add("END CATCH")
$sqlLines.Add("")
$sqlLines.Add("IF NULLIF(LTRIM(RTRIM(@TraduccionesInglesJson)), N'') IS NULL")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    THROW 50005, 'El archivo translation-seeds/base.in.json esta vacio o no pudo leerse correctamente.', 1;")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlLines.Add("INSERT INTO dbo.Traduccion (PalabraId, IdiomaId, Valor)")
$sqlLines.Add("SELECT p.Id, @IdiomaInglesId, TRY_CAST(j.[value] AS NVARCHAR(300))")
$sqlLines.Add("FROM OPENJSON(@TraduccionesInglesJson) j")
$sqlLines.Add("INNER JOIN dbo.Palabra p ON p.[Key] = j.[key] COLLATE DATABASE_DEFAULT")
$sqlLines.Add("WHERE j.[type] IN (1, 2) AND TRY_CAST(j.[value] AS NVARCHAR(300)) IS NOT NULL;")
$sqlLines.Add("")

$sqlLines.Add($markerEnd)
$sqlBlock = ($sqlLines -join [Environment]::NewLine)

$scriptLines = Get-Content -Path $sqlPath
$startIndex = [Array]::IndexOf($scriptLines, $markerStart)
$endIndex = [Array]::IndexOf($scriptLines, $markerEnd)

if ($startIndex -lt 0 -or $endIndex -lt 0 -or $endIndex -lt $startIndex) {
    throw "No se encontraron los marcadores de seed automatico en script_bd.sql."
}

$prefix = @()
if ($startIndex -gt 0) {
    $prefix = $scriptLines[0..($startIndex - 1)]
}

$suffix = @()
if ($endIndex -lt ($scriptLines.Count - 1)) {
    $suffix = $scriptLines[($endIndex + 1)..($scriptLines.Count - 1)]
}

Set-Content -Path $sqlPath -Value ($prefix + ($sqlBlock -split "`r?`n") + $suffix) -Encoding UTF8
Write-Host "Seed base sincronizado."
