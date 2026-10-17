# Proposal

## Why

`UI.csproj` declara cuatro `PackageReference` (GMap.NET.Core, GMap.NET.WinForms, Krypton.Toolkit, LiveChartsCore.SkiaSharpView.WinForms) y copia `libSkiaSharp.dll` desde el cache NuGet. Eso viola la restriccion inviolable de AGENTS.md: el laboratorio recibe solo el codigo fuente, compila `TpIDS.sln` alli y no puede restaurar paquetes. Hoy la solucion productiva compila unicamente en la maquina del desarrollador. Este cambio cierra la tarea 5.2 de `migrar-json-system-text-sin-nuget`.

## What Changes

- Incluir en `lib/` las DLL administradas y nativas que UI necesita, con version, origen y licencia documentados (convencion ya usada en `lib/pdfsharp-migradoc/6.2.4`).
- Reemplazar los `PackageReference` de `UI.csproj` por `Reference` con `HintPath` relativo al repositorio, centralizadas en un `.props` como `PdfSharp.references.props`.
- Copiar las nativas (`libSkiaSharp.dll`, `libHarfBuzzSharp.dll`) al output sin depender de `$(NuGetPackageRoot)`.
- Quitar de `UI.csproj` lo que solo existia por NuGet (`RuntimeIdentifier(s)`, copia desde el cache) y ajustar los `bindingRedirect` de `App.config`.
- Dejar el cache de tiles de GMap en modo que no requiera SQLite (ver design.md, decision 4).
- Verificar la compilacion y el arranque desde un clon limpio, sin restore ni cache NuGet.
- Actualizar `tests/README.md` y `lib/README.md`.

## Capabilities

### New Capabilities
- `compilacion-portable`: la solucion productiva `TpIDS.sln` se compila y ejecuta a partir del codigo fuente, sin restaurar paquetes NuGet ni depender de un cache previo.

### Modified Capabilities

## Impact

- `UI/UI.csproj`, `UI/App.config`, nuevo `UI/Ui.references.props` (o equivalente), `lib/`, `.gitignore` (que no excluya `lib/**`), `tests/README.md`.
- `UI/FormSeleccionarUbicacionComercio.cs` (modo de acceso de GMap).
- Los proyectos de tests pueden seguir usando NuGet; no se tocan.
- Peso del repositorio: se suman las DLL (unos pocos MB, nativas incluidas).
