[CmdletBinding()]
param(
    [string]$Configuration = "Debug",
    [switch]$IncludeIntegration,
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$DatabaseName = "TpCatedraIDS_Test"
)

$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$ResultsRoot = Join-Path $RepoRoot "TestResults"

function Resolve-FirstExistingPath {
    param([string[]]$Candidates)

    foreach ($candidate in $Candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    return $null
}

function Resolve-MSBuild {
    $known = Resolve-FirstExistingPath @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    )

    if ($known) {
        return $known
    }

    $found = Get-ChildItem -Path "C:\Program Files\Microsoft Visual Studio", "C:\Program Files (x86)\Microsoft Visual Studio" -Recurse -Filter MSBuild.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like "*\MSBuild\Current\Bin\MSBuild.exe" } |
        Select-Object -First 1 -ExpandProperty FullName

    if (-not $found) {
        throw "No se encontro MSBuild.exe."
    }

    return $found
}

function Resolve-VSTest {
    $known = Resolve-FirstExistingPath @(
        "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
    )

    if ($known) {
        return $known
    }

    $found = Get-ChildItem -Path "C:\Program Files\Microsoft Visual Studio", "C:\Program Files (x86)\Microsoft Visual Studio" -Recurse -Filter vstest.console.exe -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName

    if (-not $found) {
        throw "No se encontro vstest.console.exe."
    }

    return $found
}

function Assert-TestRunSucceeded {
    param(
        [string]$TrxPath,
        [int]$ExitCode,
        [string]$Name
    )

    if ($ExitCode -ne 0) {
        throw "$Name fallo con exit code $ExitCode."
    }

    if (-not (Test-Path -LiteralPath $TrxPath)) {
        throw "No se encontro el resultado TRX para $Name en $TrxPath."
    }

    [xml]$trx = Get-Content -LiteralPath $TrxPath
    $counters = $trx.TestRun.ResultSummary.Counters
    if ($null -eq $counters) {
        throw "El TRX de $Name no contiene contadores."
    }

    $total = [int]$counters.total
    $failed = [int]$counters.failed
    if ($total -le 0) {
        throw "$Name ejecuto 0 tests. Se aborta para evitar falsos positivos."
    }

    if ($failed -gt 0) {
        throw "$Name tiene $failed tests fallidos."
    }
}

Push-Location $RepoRoot
try {
    New-Item -ItemType Directory -Force -Path $ResultsRoot | Out-Null

    $msbuild = Resolve-MSBuild
    $vstest = Resolve-VSTest

    & $msbuild TpIDS.sln /restore /t:Build /p:Configuration=$Configuration /m
    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild finalizo con codigo $LASTEXITCODE."
    }

    $unitTrx = Join-Path $ResultsRoot "unit-tests.trx"
    if (Test-Path -LiteralPath $unitTrx) {
        Remove-Item -LiteralPath $unitTrx -Force
    }

    & $vstest `
        "tests\SERVICIOS.Tests\bin\$Configuration\net472\SERVICIOS.Tests.dll" `
        "tests\BLL.Tests\bin\$Configuration\net472\BLL.Tests.dll" `
        /ResultsDirectory:$ResultsRoot `
        /Logger:"trx;LogFileName=unit-tests.trx"
    $unitExitCode = $LASTEXITCODE
    Assert-TestRunSucceeded -TrxPath $unitTrx -ExitCode $unitExitCode -Name "Unit tests"

    if ($IncludeIntegration) {
        & (Join-Path $PSScriptRoot "prepare-test-database.ps1") -SqlServer $SqlServer -DatabaseName $DatabaseName -RepoRoot $RepoRoot

        $integrationTrx = Join-Path $ResultsRoot "integration-tests.trx"
        if (Test-Path -LiteralPath $integrationTrx) {
            Remove-Item -LiteralPath $integrationTrx -Force
        }

        & $vstest `
            "tests\TpIDS.IntegrationTests\bin\$Configuration\net472\TpIDS.IntegrationTests.dll" `
            /TestCaseFilter:"TestCategory=Integration" `
            /InIsolation `
            /ResultsDirectory:$ResultsRoot `
            /Logger:"trx;LogFileName=integration-tests.trx"
        $integrationExitCode = $LASTEXITCODE
        Assert-TestRunSucceeded -TrxPath $integrationTrx -ExitCode $integrationExitCode -Name "Integration tests"
    }
}
finally {
    Pop-Location
}
