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

if (-not [string]::IsNullOrWhiteSpace($outputDirectory) -and -not (Test-Path $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

$escapedPath = $resolvedJsonPath.Replace('"', '""')
Set-Content -Path $OutputFile -Value (':setvar SeedJsonPath "' + $escapedPath + '"') -Encoding ASCII
