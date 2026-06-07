param()

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..\..\..")).Path
$uiPath = @(
    (Join-Path $repoRoot "UI"),
    (Join-Path $repoRoot "TpIDS")
) | Where-Object { Test-Path $_ } | Select-Object -First 1
$seedJsonPath = Join-Path $repoRoot "translation-seeds\base.es.json"
$sqlPath = Join-Path $repoRoot "script_bd.sql"
$markerStart = "-- BEGIN AUTO-GENERATED I18N BASE SEED"
$markerEnd = "-- END AUTO-GENERATED I18N BASE SEED"
$idiomaBaseNombre = "Espa$([char]0x00F1)ol"

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

function Unescape-SqlLiteral {
    param([string]$Value)

    if ($null -eq $Value) {
        return ""
    }

    return $Value.Replace("''", "'")
}

function Get-ExistingItemsFromSqlBlock {
    param(
        [string]$SqlContent,
        [string]$StartMarker,
        [string]$EndMarker
    )

    $items = @{}
    $blockPattern = [regex]::Escape($StartMarker) + '(?<block>[\s\S]*?)' + [regex]::Escape($EndMarker)
    $blockMatch = [regex]::Match($SqlContent, $blockPattern)
    if (-not $blockMatch.Success) {
        return $items
    }

    $block = $blockMatch.Groups["block"].Value
    $itemPattern = "SELECT @PalabraId = Id FROM dbo\.Palabra WHERE \[Key\] = N'(?<key>(?:''|[^'])*)';[\s\S]*?THEN N'(?<value>(?:''|[^'])*)' ELSE Valor END"
    foreach ($match in [regex]::Matches($block, $itemPattern)) {
        $items[(Unescape-SqlLiteral $match.Groups["key"].Value)] = Unescape-SqlLiteral $match.Groups["value"].Value
    }

    return $items
}

$sqlContent = Get-Content -Raw -Path $sqlPath
$existingItems = Get-ExistingItemsFromSqlBlock -SqlContent $sqlContent -StartMarker $markerStart -EndMarker $markerEnd
$committedItems = @{}

try {
    $committedSqlContent = git -C $repoRoot show HEAD:script_bd.sql 2>$null
    if ($LASTEXITCODE -eq 0) {
        $committedItems = Get-ExistingItemsFromSqlBlock -SqlContent ($committedSqlContent -join [Environment]::NewLine) -StartMarker $markerStart -EndMarker $markerEnd
    }
}
catch {
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

        foreach ($match in [regex]::Matches($content, '"(?<key>(?:Form|Dialog|Error)\.[^"]+)"')) {
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
    elseif ($existingItems.ContainsKey($key)) {
        $value = $existingItems[$key]

        if ($value -eq $key -and $committedItems.ContainsKey($key) -and $committedItems[$key] -ne $key) {
            $value = $committedItems[$key]
        }
    }
    elseif ($committedItems.ContainsKey($key)) {
        $value = $committedItems[$key]
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

function Get-SqlJsonAssignments {
    param([string]$JsonText)

    $escaped = Escape-SqlLiteral $JsonText
    $chunkSize = 3500
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("DECLARE @TraduccionesBaseJson NVARCHAR(MAX) = N'';")

    for ($index = 0; $index -lt $escaped.Length; $index += $chunkSize) {
        $chunk = $escaped.Substring($index, [Math]::Min($chunkSize, $escaped.Length - $index))
        $lines.Add("SET @TraduccionesBaseJson = @TraduccionesBaseJson + N'$chunk';")
    }

    return $lines
}

$sqlLines = New-Object System.Collections.Generic.List[string]
$sqlLines.Add($markerStart)
$sqlLines.Add("-- AUTO-GENERATED BY .agents/skills/translation-base-seed-sync/scripts/sync-i18n-base-seed.ps1")
$sqlLines.Add("-- SOURCE OF TRUTH: translation-seeds/base.es.json")
$sqlLines.Add("DECLARE @IdiomaBaseId INT;")
$idiomaBaseNombreSql = Escape-SqlLiteral $idiomaBaseNombre
$sqlLines.Add("SELECT TOP 1 @IdiomaBaseId = Id FROM dbo.Idioma WHERE Nombre = N'$idiomaBaseNombreSql';")
$sqlLines.Add("IF @IdiomaBaseId IS NULL")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    INSERT INTO dbo.Idioma (Nombre, [Default]) VALUES (N'$idiomaBaseNombreSql', 0);")
$sqlLines.Add("    SET @IdiomaBaseId = SCOPE_IDENTITY();")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlJsonLines = Get-SqlJsonAssignments -JsonText $jsonOutput
foreach ($line in $sqlJsonLines) {
    $sqlLines.Add($line)
}
$sqlLines.Add("")
$sqlLines.Add("IF EXISTS (SELECT 1 FROM sys.databases WHERE name = DB_NAME() AND compatibility_level < 130)")
$sqlLines.Add("BEGIN")
$sqlLines.Add("    ALTER DATABASE CURRENT SET COMPATIBILITY_LEVEL = 130;")
$sqlLines.Add("END")
$sqlLines.Add("")
$sqlLines.Add("DECLARE @Seed TABLE")
$sqlLines.Add("(")
$sqlLines.Add("    [Key] VARCHAR(150) NOT NULL,")
$sqlLines.Add("    Valor VARCHAR(300) NOT NULL")
$sqlLines.Add(");")
$sqlLines.Add("")
$sqlLines.Add("INSERT INTO @Seed ([Key], Valor)")
$sqlLines.Add("SELECT [key], TRY_CAST([value] AS VARCHAR(300))")
$sqlLines.Add("FROM OPENJSON(@TraduccionesBaseJson)")
$sqlLines.Add("WHERE [type] IN (1, 2);")
$sqlLines.Add("")
$sqlLines.Add("MERGE dbo.Palabra AS Target")
$sqlLines.Add("USING (SELECT [Key] FROM @Seed) AS Source")
$sqlLines.Add("    ON Target.[Key] = Source.[Key]")
$sqlLines.Add("WHEN MATCHED THEN")
$sqlLines.Add("    UPDATE SET EsBase = 1")
$sqlLines.Add("WHEN NOT MATCHED THEN")
$sqlLines.Add("    INSERT ([Key], EsBase) VALUES (Source.[Key], 1);")
$sqlLines.Add("")
$sqlLines.Add("MERGE dbo.Traduccion AS Target")
$sqlLines.Add("USING")
$sqlLines.Add("(")
$sqlLines.Add("    SELECT p.Id AS PalabraId, @IdiomaBaseId AS IdiomaId, s.Valor")
$sqlLines.Add("    FROM @Seed s")
$sqlLines.Add("    INNER JOIN dbo.Palabra p ON p.[Key] = s.[Key]")
$sqlLines.Add(") AS Source")
$sqlLines.Add("    ON Target.PalabraId = Source.PalabraId AND Target.IdiomaId = Source.IdiomaId")
$sqlLines.Add("WHEN MATCHED THEN")
$sqlLines.Add("    UPDATE SET Valor = CASE WHEN NULLIF(LTRIM(RTRIM(Target.Valor)), '') IS NULL THEN Source.Valor ELSE Target.Valor END")
$sqlLines.Add("WHEN NOT MATCHED THEN")
$sqlLines.Add("    INSERT (PalabraId, IdiomaId, Valor) VALUES (Source.PalabraId, Source.IdiomaId, Source.Valor);")
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

$suffix = $suffix | ForEach-Object {
    $_ -replace 'ORDER BY CASE WHEN Id = @IdiomaBaseId THEN 0 ELSE 1 END, Nombre, Id;', 'ORDER BY Nombre, Id;'
}

Set-Content -Path $sqlPath -Value ($prefix + ($sqlBlock -split "`r?`n") + $suffix) -Encoding UTF8
Write-Host "Seed base sincronizado."
