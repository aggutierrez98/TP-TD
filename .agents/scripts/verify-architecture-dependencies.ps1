[CmdletBinding()]
param(
    [string]$RepoRoot
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\\..")).Path
}

$allowedDependencies = [ordered]@{
    "ABSTRACCIONES" = @()
    "BE" = @("ABSTRACCIONES")
    "SERVICIOS" = @("ABSTRACCIONES")
    "DAL" = @("BE", "SERVICIOS")
    "REPOSITORIO" = @("BE", "DAL", "SERVICIOS")
    "BLL" = @("BE", "REPOSITORIO", "SERVICIOS")
    "UI" = @("BE", "BLL", "SERVICIOS")
}

function Get-ProjectFilePath {
    param(
        [string]$ProjectName
    )

    $projectFileName = "$ProjectName.csproj"
    return Join-Path $RepoRoot (Join-Path $ProjectName $projectFileName)
}

function Get-ProjectDependencies {
    param(
        [string]$ProjectName
    )

    $projectPath = Get-ProjectFilePath -ProjectName $ProjectName
    if (-not (Test-Path -LiteralPath $projectPath)) {
        throw "No se encontro el proyecto esperado: $projectPath"
    }

    [xml]$projectXml = Get-Content -LiteralPath $projectPath
    $namespaceManager = New-Object System.Xml.XmlNamespaceManager($projectXml.NameTable)
    $namespaceManager.AddNamespace("msb", $projectXml.Project.NamespaceURI)

    $references = foreach ($referenceNode in $projectXml.SelectNodes("//msb:ProjectReference", $namespaceManager)) {
        $include = $referenceNode.GetAttribute("Include")
        if ([string]::IsNullOrWhiteSpace($include)) {
            continue
        }

        $referenceDirectory = Split-Path -Path $include -Parent
        Split-Path -Path $referenceDirectory -Leaf
    }

    return @($references | Sort-Object -Unique)
}

$violations = @()

foreach ($projectName in $allowedDependencies.Keys) {
    $expectedDependencies = @($allowedDependencies[$projectName] | Sort-Object -Unique)
    $actualDependencies = Get-ProjectDependencies -ProjectName $projectName

    $missingDependencies = @($expectedDependencies | Where-Object { $_ -notin $actualDependencies })
    $unexpectedDependencies = @($actualDependencies | Where-Object { $_ -notin $expectedDependencies })

    if ($missingDependencies.Count -gt 0 -or $unexpectedDependencies.Count -gt 0) {
        $violations += [pscustomobject]@{
            Project = $projectName
            Expected = if ($expectedDependencies.Count -gt 0) { $expectedDependencies -join ", " } else { "(sin dependencias)" }
            Actual = if ($actualDependencies.Count -gt 0) { $actualDependencies -join ", " } else { "(sin dependencias)" }
            Missing = if ($missingDependencies.Count -gt 0) { $missingDependencies -join ", " } else { "-" }
            Unexpected = if ($unexpectedDependencies.Count -gt 0) { $unexpectedDependencies -join ", " } else { "-" }
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "Violaciones de arquitectura detectadas:" -ForegroundColor Red
    foreach ($violation in $violations) {
        Write-Host ("- {0}: esperado [{1}], actual [{2}], faltantes [{3}], prohibidas [{4}]" -f `
            $violation.Project, `
            $violation.Expected, `
            $violation.Actual, `
            $violation.Missing, `
            $violation.Unexpected) -ForegroundColor Red
    }

    exit 1
}

Write-Host "Architecture dependency guard OK: la matriz de referencias entre capas coincide con la arquitectura definida."
