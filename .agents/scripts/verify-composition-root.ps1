[CmdletBinding()]
param(
    [string]$RepoRoot
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\\..")).Path
}

# Unico archivo autorizado a construir BLLs con 'new'.
$composerRelPath = "BLL\CompositionRoot.cs"

# Unicos archivos de BLL autorizados a nombrar CompositionRoot:
#  - el propio root,
#  - el arranque funcional (consume instancias ya compuestas),
#  - el helper estatico de excepciones (Service Locator consciente y aceptado).
$serviceLocatorAllowList = @(
    "BLL\CompositionRoot.cs",
    "BLL\InicializadorAplicacionBLL.cs",
    "BLL\ManejadorExcepcionesBLL.cs"
)

function Get-RelativePath {
    param([string]$FullPath)
    return $FullPath.Substring($RepoRoot.Length).TrimStart('\')
}

$sourceFiles = Get-ChildItem -Path $RepoRoot -Recurse -Filter *.cs -File |
    Where-Object {
        $_.FullName -notmatch '\\(bin|obj)\\' -and
        (Get-RelativePath -FullPath $_.FullName) -notlike 'tests\*'
    }

$violations = @()

# Regla 1: ningun 'new <X>BLL(' fuera de CompositionRoot.cs.
# La unica via de construccion de un BLL es el Composition Root.
$newBllMatches = $sourceFiles | Select-String -Pattern 'new\s+\w+BLL\s*\('
foreach ($match in $newBllMatches) {
    $rel = Get-RelativePath -FullPath $match.Path
    if ($rel -ne $composerRelPath) {
        $violations += [pscustomobject]@{
            Rule = "new-fuera-del-root"
            File = $rel
            Line = $match.LineNumber
            Detail = $match.Line.Trim()
        }
    }
}

# Regla 2: ningun BLL de caso de uso nombra CompositionRoot (fin del Service Locator escondido).
$bllFiles = $sourceFiles | Where-Object { (Get-RelativePath -FullPath $_.FullName) -like 'BLL\*' }
foreach ($file in $bllFiles) {
    $rel = Get-RelativePath -FullPath $file.FullName
    if ($serviceLocatorAllowList -contains $rel) {
        continue
    }
    $locatorMatches = Select-String -Path $file.FullName -Pattern 'CompositionRoot'
    foreach ($match in $locatorMatches) {
        $violations += [pscustomobject]@{
            Rule = "service-locator-en-bll"
            File = $rel
            Line = $match.LineNumber
            Detail = $match.Line.Trim()
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "Violaciones del Composition Root detectadas:" -ForegroundColor Red
    foreach ($violation in $violations) {
        if ($violation.Rule -eq "new-fuera-del-root") {
            Write-Host ("- [{0}] {1}:{2} -> construir un BLL con 'new' solo se permite en {3}. Usar CompositionRoot.<Getter>. Linea: {4}" -f `
                $violation.Rule, $violation.File, $violation.Line, $composerRelPath, $violation.Detail) -ForegroundColor Red
        }
        else {
            Write-Host ("- [{0}] {1}:{2} -> un BLL de caso de uso no debe nombrar CompositionRoot; recibir la dependencia por el ctor internal. Linea: {3}" -f `
                $violation.Rule, $violation.File, $violation.Line, $violation.Detail) -ForegroundColor Red
        }
    }

    exit 1
}

Write-Host "Composition root guard OK: los BLL se construyen solo desde CompositionRoot y ninguno actua como Service Locator."
