# Bibliotecas externas incluidas en el repositorio

La solucion productiva `TpIDS.sln` se compila en el laboratorio sin NuGet (ver AGENTS.md). Estas DLL se obtuvieron de los paquetes NuGet indicados, se copian tal cual (sin modificar) y se referencian con `HintPath` relativo desde los `.props` de cada proyecto. Cada carpeta conserva el `.nuspec` y la licencia del paquete original.

## UI (`UI/Ui.references.props`)

| Carpeta (`lib/<paquete>/<version>`) | Ensamblado | Licencia |
|---|---|---|
| `gmap.net.core/2.1.7` | GMap.NET.Core | GMap.NET (wiki License) |
| `gmap.net.winforms/2.1.7` | GMap.NET.WindowsForms | GMap.NET (wiki License) |
| `krypton.toolkit/105.26.7.201` | Krypton.Toolkit | BSD-3-Clause |
| `livechartscore/2.0.5` | LiveChartsCore | MIT |
| `livechartscore.skiasharpview/2.0.5` | LiveChartsCore.SkiaSharpView | MIT |
| `livechartscore.skiasharpview.winforms/2.0.5` | LiveChartsCore.SkiaSharpView.WinForms | MIT |
| `skiasharp/3.119.0` | SkiaSharp | MIT |
| `skiasharp.harfbuzz/3.119.0` | SkiaSharp.HarfBuzz | MIT |
| `skiasharp.views.desktop.common/3.119.0` | SkiaSharp.Views.Desktop.Common | MIT |
| `skiasharp.views.windowsforms/3.119.0` | SkiaSharp.Views.WindowsForms | MIT |
| `harfbuzzsharp/8.3.1.1` | HarfBuzzSharp | MIT |
| `opentk/3.1.0` | OpenTK | OpenTK (MIT/X11) |
| `opentk.glcontrol/3.1.0` | OpenTK.GLControl | OpenTK (MIT/X11) |
| `newtonsoft.json/13.0.1` | Newtonsoft.Json | MIT |
| `system.resources.extensions/9.0.10` | System.Resources.Extensions | MIT |
| `system.formats.nrbf/9.0.10` | System.Formats.Nrbf | MIT |
| `system.reflection.metadata/9.0.10` | System.Reflection.Metadata | MIT |
| `system.collections.immutable/9.0.10` | System.Collections.Immutable | MIT |
| `microsoft.bcl.hashcode/1.1.1` | Microsoft.Bcl.HashCode | MIT |
| `system.data.sqlite.core/1.0.115.5` | System.Data.SQLite (net46) | Dominio publico (SQLite) |

### Nativas (win-x64)

| Carpeta | Archivo | Licencia |
|---|---|---|
| `skiasharp.nativeassets.win32/3.119.0` | libSkiaSharp.dll | MIT |
| `harfbuzzsharp.nativeassets.win32/8.3.1.1` | libHarfBuzzSharp.dll | MIT |

Se copian planas a la raiz del output (ahi las busca SkiaSharp). Solo x64: la aplicacion no se compila ni se ejecuta como x86.

`system.data.sqlite.core/1.0.115.5` trae ademas `x86/SQLite.Interop.dll` y `x64/SQLite.Interop.dll`, que se copian a subcarpetas `x86` y `x64` del output junto a `System.Data.SQLite.dll`. GMap.NET.Core los necesita en ejecucion (cache de mapas); sin ellos el dialogo de ubicacion de comercio falla al abrirse. Los binarios salen del paquete `Stub.System.Data.SQLite.Core.NetFramework` 1.0.115.5 (variante `net46`).

## SERVICIOS

- `pdfsharp-migradoc/6.2.4` (`SERVICIOS/PdfSharp.references.props`).
- System.Text.Json y sus dependencias no estan aca: salen del directorio de MSBuild (`SERVICIOS/SystemTextJson.references.props`).

## Actualizar una biblioteca

Copiar la nueva version a `lib/<paquete>/<version>/`, actualizar el `.props` y esta tabla, y verificar en un clon limpio sin cache NuGet. Quitar la carpeta vieja.
