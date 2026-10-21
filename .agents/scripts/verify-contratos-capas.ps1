[CmdletBinding()]
param(
    [string]$RepoRoot
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    # GetFullPath y no Resolve-Path: mismo motivo que en verify-composition-root.ps1
    # (Resolve-Path no normaliza ".." y sobre UNC devuelve la ruta calificada por proveedor).
    $RepoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\.."))
}

$RepoRoot = $RepoRoot.TrimEnd('\')

# Capas cuyas firmas no privadas no pueden recibir IDs.
$capasSinIds = @("ABSTRACCIONES", "BLL", "REPOSITORIO", "DAL", "SERVICIOS")
# Capas donde vale ObtenerPorId(int id) para rehidratar entidades.
$capasConObtenerPorId = @("REPOSITORIO", "DAL")
# Capas que pueden leer configuracion.
$capasConConfiguracion = @("SERVICIOS", "DAL")

function Get-RelativePath {
    param([string]$FullPath)
    if ($FullPath.StartsWith($RepoRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $FullPath.Substring($RepoRoot.Length).TrimStart('\')
    }

    return $FullPath
}

function Get-Capa {
    param([string]$RelativePath)
    return $RelativePath.Split('\')[0]
}

# Quita comentarios para que el codigo comentado y la documentacion XML no
# cuenten como violaciones. Los literales de texto se dejan: una "//" dentro de
# una URL solo recorta esa linea, que nunca es una declaracion.
function Remove-Comentarios {
    param([string]$Texto)
    $sinBloques = [regex]::Replace($Texto, '/\*.*?\*/', { param($m) [regex]::Replace($m.Value, '[^\n]', '') }, 'Singleline')
    return [regex]::Replace($sinBloques, '//[^\n]*', '')
}

function Get-NumeroDeLinea {
    param([string]$Texto, [int]$Indice)
    return ([regex]::Matches($Texto.Substring(0, $Indice), "`n")).Count + 1
}

# El recorrido excluye '.git' (rutas que superan MAX_PATH por UNC), bin, obj y tests.
$scanRoots = Get-ChildItem -LiteralPath $RepoRoot -Directory |
    Where-Object { $_.Name -ne '.git' }

$sourceFiles = @()
foreach ($scanRoot in $scanRoots) {
    $sourceFiles += Get-ChildItem -LiteralPath $scanRoot.FullName -Recurse -Filter *.cs -File
}

$sourceFiles = $sourceFiles |
    Where-Object {
        $_.FullName -notmatch '\\(bin|obj)\\' -and
        (Get-RelativePath -FullPath $_.FullName) -notlike 'tests\*'
    }

# Declaracion de tipo: sirve para la regla 1 y para saber si un miembro sin
# modificador de acceso pertenece a una interfaz (publico) o a una clase (privado).
$regexTipo = [regex]'(?m)^[ \t]*(?:(?:public|internal|protected|private|static|sealed|abstract|partial|readonly)\s+)*(?<tipo>class|struct|record|interface|enum)\s+(?<nombre>\w+)'
# Inicio de una declaracion de metodo o constructor hasta su "(".
$regexMiembro = [regex]'(?m)^[ \t]*(?<mods>(?:(?:public|internal|protected|private|static|async|override|virtual|abstract|sealed|new|extern|unsafe|partial)\s+)*)(?:[\w<>\[\],.?]+(?:[ \t]*<[^()\r\n]*?>)?[\w\[\]?]*[ \t]+)?(?<explicito>(?:\w+\.)+)?(?<nombre>\w+)[ \t]*(?:<[^()\r\n]*?>)?[ \t]*\('
$palabrasNoMiembro = @("if", "for", "foreach", "while", "switch", "using", "lock", "catch", "return", "throw", "new", "nameof", "typeof", "sizeof", "default", "await", "base", "this", "fixed", "checked", "unchecked", "when")
$regexParamId = [regex]'^(?:(?:ref|out|in|params|this)\s+)?(?:int|long|Guid|System\.Guid)\??\s+(?<nombre>[iI]d|\w+Id)\b'
$regexParamIds = [regex]'^(?:(?:ref|out|in|params|this)\s+)?(?:(?:List|IList|IEnumerable|ICollection|IReadOnlyList|IReadOnlyCollection|HashSet|ISet)<(?:int|long)>|(?:int|long)\[\])\s+(?<nombre>\w*Ids)\b'
$regexCascaron = [regex]'new\s+(?<tipo>\w+)\s*(?:\(\s*\))?\s*\{\s*Id\s*='
$regexFkBe = [regex]'(?m)^[ \t]*public\s+(?:int|long|Guid)\??\s+(?<nombre>\w+Id)\s*\{'

$violations = New-Object System.Collections.Generic.List[object]

function Add-Violacion {
    param([string]$Regla, [string]$Archivo, [int]$Linea, [string]$Detalle)
    $violations.Add([pscustomobject]@{
        Regla = $Regla
        Archivo = $Archivo
        Linea = $Linea
        Detalle = $Detalle
        Clave = ("{0}|{1}|{2}" -f $Regla, $Archivo, $Detalle)
    })
}

foreach ($file in $sourceFiles) {
    $rel = Get-RelativePath -FullPath $file.FullName
    $capa = Get-Capa -RelativePath $rel
    $texto = Remove-Comentarios -Texto ([System.IO.File]::ReadAllText($file.FullName))
    $tipos = $regexTipo.Matches($texto)

    # Regla 1: ABSTRACCIONES solo declara interfaces y enums.
    if ($capa -eq "ABSTRACCIONES") {
        foreach ($tipo in $tipos) {
            if (@("class", "struct", "record") -contains $tipo.Groups["tipo"].Value) {
                Add-Violacion "abstracciones-concreta" $rel (Get-NumeroDeLinea $texto $tipo.Index) $tipo.Groups["nombre"].Value
            }
        }
    }

    # Regla 2: firmas no privadas sin IDs.
    if ($capasSinIds -contains $capa) {
        foreach ($miembro in $regexMiembro.Matches($texto)) {
            $nombre = $miembro.Groups["nombre"].Value
            if ($palabrasNoMiembro -contains $nombre) {
                continue
            }

            $mods = $miembro.Groups["mods"].Value
            if ($mods -match '\bprivate\b' -and $mods -notmatch '\bprotected\b') {
                continue
            }

            # Una implementacion explicita (IContrato.Metodo) es parte del contrato publico.
            if ($mods -notmatch '\b(public|internal|protected)\b' -and -not $miembro.Groups["explicito"].Success) {
                # Sin modificador: solo cuenta si el tipo que lo contiene es una interfaz.
                $contenedor = $null
                foreach ($tipo in $tipos) {
                    if ($tipo.Index -lt $miembro.Index) { $contenedor = $tipo } else { break }
                }
                if ($null -eq $contenedor -or $contenedor.Groups["tipo"].Value -ne "interface") {
                    continue
                }
            }

            # Parametros hasta el ")" que cierra, aunque ocupen varias lineas.
            $inicio = $miembro.Index + $miembro.Length
            $profundidad = 1
            $fin = $inicio
            while ($fin -lt $texto.Length -and $profundidad -gt 0) {
                $c = $texto[$fin]
                if ($c -eq '(') { $profundidad++ } elseif ($c -eq ')') { $profundidad-- }
                $fin++
            }
            $parametros = $texto.Substring($inicio, [Math]::Max(0, $fin - $inicio - 1))

            # Separar por comas de primer nivel (los genericos tambien llevan comas).
            $partes = @()
            $actual = New-Object System.Text.StringBuilder
            $anidado = 0
            foreach ($c in $parametros.ToCharArray()) {
                if ($c -eq '<' -or $c -eq '(' -or $c -eq '[') { $anidado++ }
                elseif ($c -eq '>' -or $c -eq ')' -or $c -eq ']') { $anidado-- }
                if ($c -eq ',' -and $anidado -eq 0) {
                    $partes += $actual.ToString().Trim()
                    [void]$actual.Clear()
                }
                else {
                    [void]$actual.Append($c)
                }
            }
            $partes += $actual.ToString().Trim()

            foreach ($parte in $partes) {
                $param = [regex]::Replace($parte, '^\[[^\]]*\]\s*', '')
                $matchId = $regexParamId.Match($param)
                $matchIds = $regexParamIds.Match($param)
                if (-not $matchId.Success -and -not $matchIds.Success) {
                    continue
                }

                $nombreParam = if ($matchId.Success) { $matchId.Groups["nombre"].Value } else { $matchIds.Groups["nombre"].Value }
                if ($matchId.Success -and $nombre -eq "ObtenerPorId" -and $nombreParam -eq "id" -and ($capasConObtenerPorId -contains $capa)) {
                    continue
                }

                Add-Violacion "firma-con-id" $rel (Get-NumeroDeLinea $texto $miembro.Index) ("{0}({1})" -f $nombre, $nombreParam)
            }
        }
    }

    # Regla 3: BLL no arma entidades cascaron con solo el Id.
    if ($capa -eq "BLL") {
        foreach ($cascaron in $regexCascaron.Matches($texto)) {
            Add-Violacion "entidad-cascaron" $rel (Get-NumeroDeLinea $texto $cascaron.Index) $cascaron.Groups["tipo"].Value
        }
    }

    # Regla 4: BE se relaciona con objetos, no con FKs.
    if ($capa -eq "BE") {
        foreach ($fk in $regexFkBe.Matches($texto)) {
            Add-Violacion "fk-en-be" $rel (Get-NumeroDeLinea $texto $fk.Index) $fk.Groups["nombre"].Value
        }
    }

    # Regla 5: solo SERVICIOS y DAL leen configuracion.
    if (-not ($capasConConfiguracion -contains $capa)) {
        foreach ($config in [regex]::Matches($texto, '\bConfigurationManager\b')) {
            Add-Violacion "configuracion-fuera-de-servicios" $rel (Get-NumeroDeLinea $texto $config.Index) "ConfigurationManager"
        }
    }
}

$ayuda = @{
    "abstracciones-concreta" = "ABSTRACCIONES solo admite interfaces y enums; mover la clase a la capa que corresponda o reemplazarla por una interfaz I* con implementacion en SERVICIOS"
    "firma-con-id" = "recibir la entidad en lugar de su Id (unica excepcion: ObtenerPorId(int id) en REPOSITORIO/DAL)"
    "entidad-cascaron" = "BLL no arma entidades con solo el Id; recibir la entidad real"
    "fk-en-be" = "BE se relaciona con objetos; reemplazar la FK por una referencia a la entidad"
    "configuracion-fuera-de-servicios" = "solo SERVICIOS y DAL leen ConfigurationManager; el servicio lee su propia configuracion"
}

if ($violations.Count -gt 0) {
        Write-Host "Violaciones de los contratos entre capas (ver 'Contratos entre clases' en .agents/02-architecture-7-layers.md):" -ForegroundColor Red
        foreach ($violation in $violations) {
            Write-Host ("- [{0}] {1}:{2} -> {3}. Detalle: {4}" -f `
                $violation.Regla, $violation.Archivo, $violation.Linea, $ayuda[$violation.Regla], $violation.Detalle) -ForegroundColor Red
        }
    exit 1
}

Write-Host "Contratos entre capas OK: ABSTRACCIONES solo con interfaces y enums, firmas con entidades y configuracion solo en SERVICIOS/DAL."
