# Design

## Context

Los proyectos son formato clasico (`ToolsVersion="15.0"`, no SDK-style): con `Reference` + `HintPath` compilan sin restore. Precedentes en el repo: `SERVICIOS/PdfSharp.references.props` (DLL en `lib/pdfsharp-migradoc/6.2.4`) y `SERVICIOS/SystemTextJson.references.props`. El grafo transitivo real sale de `UI/obj/project.assets.json`. Las 4 `PackageReference` directas arrastran 36 paquetes; no todos hacen falta en runtime.

Inventario necesario (version del grafo actual):

| Origen | Administradas | Nativas |
|---|---|---|
| GMap.NET 2.1.7 | GMap.NET.Core, GMap.NET.WindowsForms, Newtonsoft.Json 13.0.1 | - |
| Krypton.Toolkit 105.26.7.201 | Krypton.Toolkit, System.Resources.Extensions 9.0.10, System.Formats.Nrbf, System.Reflection.Metadata, System.Collections.Immutable, Microsoft.Bcl.HashCode | - |
| LiveCharts 2.0.5 | LiveChartsCore, .SkiaSharpView, .SkiaSharpView.WinForms | - |
| SkiaSharp 3.119.0 | SkiaSharp, SkiaSharp.HarfBuzz, SkiaSharp.Views.Desktop.Common, SkiaSharp.Views.WindowsForms, HarfBuzzSharp 8.3.1.1, OpenTK 3.1.0, OpenTK.GLControl | libSkiaSharp.dll, libHarfBuzzSharp.dll (win-x64) |

`System.Memory`, `System.Buffers`, `System.Runtime.CompilerServices.Unsafe`, `System.Numerics.Vectors`, `System.Threading.Tasks.Extensions`, `Microsoft.Bcl.AsyncInterfaces`, `System.Text.Json`, `System.Text.Encodings.Web` y `System.ValueTuple` ya los provee `SystemTextJson.references.props`; no se duplican en `lib/`.

## Goals / Non-Goals

**Goals:**
- `TpIDS.sln` compila y arranca desde un clon limpio, sin restore.
- Un unico lugar (`.props`) que enumera las referencias de UI.

**Non-Goals:**
- Reemplazar o actualizar GMap, Krypton o LiveCharts.
- Portar los proyectos de tests fuera de NuGet.
- Soporte x86 o ARM: se mantiene win-x64 como hoy.

## Decisions

1. **DLL en `lib/<paquete>/<version>/`** (confirmado por el usuario: el TP permite incluirlas). Alternativas descartadas: reemplazar librerias (rehace la migracion a Krypton) y exigir NuGet en el laboratorio (viola la regla).
2. **`UI/Ui.references.props`** importado desde `UI.csproj`, con `Reference`/`HintPath`/`Private=true`, siguiendo `PdfSharp.references.props`. Las DLL que ya vienen del grupo System.Text.Json no se repiten.
3. **Nativas como `None` + `CopyToOutputDirectory`** apuntando a `lib/`, planas en la raiz del output (donde SkiaSharp las encuentra, ya verificado). Se eliminan `RuntimeIdentifier(s)` y la ruta a `$(NuGetPackageRoot)`.
4. **GMap en modo sin SQLite**: hoy `GMaps.Instance.Mode = AccessMode.ServerAndCache` usa `System.Data.SQLite` (cache de tiles) y su `SQLite.Interop.dll` nativo x86/x64, ademas de `System.Data.SqlClient` y `EntityFramework`. Se pasa a `AccessMode.ServerOnly`: cuesta recargar tiles entre sesiones, pero evita 8 paquetes y un nativo con problemas de bitness. Confirmado por el usuario. Si el smoke test muestra que GMap igual los carga, se incluyen en `lib/`.
5. **`bindingRedirect` explicitos en `App.config`** para las DLL con versiones cruzadas (System.Resources.Extensions ya esta; revisar Newtonsoft, System.Collections.Immutable, System.Reflection.Metadata), porque `AutoGenerateBindingRedirects` se comporta distinto segun se compile desde VS o MSBuild.
7. **SQLite incluido en `lib/` igualmente** (autorizado explicitamente por el usuario): con `AccessMode.ServerOnly` el control `GMapControl` igual inicializa `SQLitePureImageCache.Ping()` en su constructor estatico, y sin `System.Data.SQLite.dll` el dialogo de ubicacion falla al abrirse con `FileNotFoundException` (5 tests UI de ubicacion en rojo). Se agrega `lib/system.data.sqlite.core/1.0.115.5/` con `System.Data.SQLite.dll` (net46) y `x86`/`x64` de `SQLite.Interop.dll`, tomados sin modificar de `Stub.System.Data.SQLite.Core.NetFramework` 1.0.115.5. No hacen falta `System.Data.SQLite.EF6`, `.Linq`, `System.Data.SqlClient` ni `EntityFramework`. Solo es dependencia de ejecucion: la UI no la referencia en compilacion.
6. **Verificacion en clon limpio** (`git clone`, `NuGetPackageRoot` vacio, `msbuild /t:Rebuild` sin `/restore`), nunca sobre el arbol de trabajo.

## Risks / Trade-offs

- [`System.Resources.Extensions.targets` fija `GenerateResourceUsePreserializedResources`; al quitar el paquete los `.resx` con recursos serializados pueden fallar] → verificar en el clon limpio; si hace falta, declarar la propiedad en `UI.csproj`.
- [Falta una DLL transitiva que solo se carga en un flujo concreto] → smoke test que abre cada pantalla con mapa y graficos, y `fuslogvw`/`ProcMon` ante `FileNotFoundException`.
- [Una DLL queda excluida por `.gitignore` y el laboratorio compila sin ella] → verificar con `git clone` y `git check-ignore -v lib/...`, no solo en el arbol de trabajo.
- [Peso del repo y licencias] → README con licencia por paquete (MIT / Apache-2.0 / LGPL segun corresponda; confirmar al copiar).
- [Los builds locales usan el cache y ocultan faltantes] → ninguna afirmacion de portabilidad sin la verificacion de la decision 6.

## Migration Plan

1. Copiar DLL a `lib/` y documentar; 2. agregar el `.props` sin quitar aun los `PackageReference` y compilar; 3. quitar los `PackageReference` y las propiedades de RID; 4. ajustar `App.config` y GMap; 5. verificar en clon limpio y con la suite de tests. Rollback: `git revert`; los `PackageReference` originales quedan en el historial.

## Open Questions

- Ninguna que cambie el alcance. Las licencias exactas se registran al copiar.
