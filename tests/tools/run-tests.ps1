[CmdletBinding()]
param(
    [string]$Configuration = "Debug",
    [switch]$IncludeUi,
    # Prefijos de casos a correr en la suite UI (ej: T04 o T04,T05 o N01).
    # Filtra por nombre de test, asi que sirve para cualquier convencion
    # futura mientras las clases/metodos arranquen con ese prefijo.
    # Implica -IncludeUi. Los unit tests siempre corren completos (tardan ~1s).
    [string[]]$UiFilter,
    [string]$SqlServer = ".\SQLEXPRESS",
    [string]$DatabaseName = "TpCatedraIDS_Test"
)

$ErrorActionPreference = "Stop"
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$ResultsRoot = Join-Path $RepoRoot "TestResults"

if ($UiFilter) {
    $IncludeUi = $true
}

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

    # TpIDS.sln queda solo con proyectos productivos (sin NuGet) para que compile
    # offline en otras maquinas; los tests viven en tests\TpIDS.Tests.sln.
    & $msbuild "tests\TpIDS.Tests.sln" /restore /t:Build /p:Configuration=$Configuration /m
    if ($LASTEXITCODE -ne 0) {
        throw "MSBuild finalizo con codigo $LASTEXITCODE."
    }

    $unitTrx = Join-Path $ResultsRoot "unit-tests.trx"
    if (Test-Path -LiteralPath $unitTrx) {
        Remove-Item -LiteralPath $unitTrx -Force
    }

    & $vstest `
        "tests\SERVICIOS.Tests\bin\$Configuration\net472\SERVICIOS.Tests.dll" `
        /ResultsDirectory:$ResultsRoot `
        /Logger:"trx;LogFileName=unit-tests.trx"
    $unitExitCode = $LASTEXITCODE
    Assert-TestRunSucceeded -TrxPath $unitTrx -ExitCode $unitExitCode -Name "Unit tests"

    if ($IncludeUi) {
        & (Join-Path $PSScriptRoot "prepare-test-database.ps1") -SqlServer $SqlServer -DatabaseName $DatabaseName -RepoRoot $RepoRoot

        # Los tests UI lanzan TpIDS.exe con FlaUI: requieren sesion de escritorio
        # interactiva desbloqueada y SQL Server local con la BD de test preparada.
        $uiTrx = Join-Path $ResultsRoot "ui-tests.trx"
        if (Test-Path -LiteralPath $uiTrx) {
            Remove-Item -LiteralPath $uiTrx -Force
        }

        $env:TPIDS_UITEST_SQLSERVER = $SqlServer
        $env:TPIDS_UITEST_DATABASE = $DatabaseName
        $env:TPIDS_UITEST_CONFIGURATION = $Configuration

        $uiTestCaseFilter = "TestCategory=UI"
        if ($UiFilter) {
            # Cada valor matchea contra el FullyQualifiedName; el "." previo
            # ancla al inicio de la clase (TpIDS.UiTests.T04_...) para no
            # enganchar coincidencias en el medio de un nombre.
            $clauses = ($UiFilter |
                ForEach-Object { $_.Split(',') } |
                ForEach-Object { $_.Trim() } |
                Where-Object { $_ } |
                ForEach-Object { "FullyQualifiedName~.$_" }) -join "|"
            $uiTestCaseFilter = "TestCategory=UI&($clauses)"
            Write-Host "Filtro UI activo: $uiTestCaseFilter"
        }

        & $vstest `
            "tests\TpIDS.UiTests\bin\$Configuration\net472\TpIDS.UiTests.dll" `
            /TestCaseFilter:$uiTestCaseFilter `
            /InIsolation `
            /ResultsDirectory:$ResultsRoot `
            /Logger:"trx;LogFileName=ui-tests.trx"
        $uiExitCode = $LASTEXITCODE
        Assert-TestRunSucceeded -TrxPath $uiTrx -ExitCode $uiExitCode -Name "UI tests"
    }
}
finally {
    Pop-Location
}
