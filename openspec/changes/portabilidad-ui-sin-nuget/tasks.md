# Tasks

## 1. Inventario y libreria

- [x] 1.1 Copiar a `lib/<paquete>/<version>/` las DLL administradas de la tabla de design.md (desde `UI/obj/project.assets.json` y el cache local), con sus `.nuspec` y licencias; verificar con `fd . lib` que cada una existe
- [x] 1.2 Copiar `libSkiaSharp.dll` y `libHarfBuzzSharp.dll` (win-x64) y verificar que son de 64 bits con `file`
- [x] 1.3 Crear `lib/README.md` con paquete, version, origen y licencia por DLL, y verificar que no falta ninguna de `lib/`
- [x] 1.4 Verificar con `git check-ignore -v` que ninguna DLL de `lib/` esta ignorada; ajustar `.gitignore` si hace falta

- [x] 1.5 Incluir `System.Data.SQLite` 1.0.115.5 (`lib/system.data.sqlite.core/1.0.115.5/`, con las nativas x86/x64) y referenciarlo desde `UI/Ui.references.props`, porque `GMapControl` lo carga aun en `ServerOnly`; verificar con `fd -I sqlite UI/bin/Debug` y con los 5 tests UI de ubicacion en verde

## 2. Referencias de UI

- [x] 2.1 Crear `UI/Ui.references.props` con `Reference`/`HintPath`/`Private=true` e importarlo desde `UI.csproj`; verificar que `TpIDS.sln` compila (se hizo junto con 2.2: convivir con los `PackageReference` duplicaria referencias)
- [x] 2.2 Quitar los 4 `PackageReference`, `RuntimeIdentifier(s)` y la copia desde `$(NuGetPackageRoot)`; reemplazar por `None` + `CopyToOutputDirectory` hacia `lib/`; verificar que el output contiene `libSkiaSharp.dll` y `libHarfBuzzSharp.dll`
- [x] 2.3 Pasar GMap a `AccessMode.ServerOnly` en `FormSeleccionarUbicacionComercio.cs` y verificar que el mapa carga tiles y los tests UI del mapa siguen pasando
- [x] 2.4 Revisar los `bindingRedirect` de `App.config` contra las identidades reales del output y verificar con `monodis`/`ildasm` o con el arranque que no hay `FileLoadException`

## 3. Verificacion de portabilidad

- [x] 3.1 En un `git clone` nuevo con `NuGetPackageRoot` vacio, ejecutar `msbuild TpIDS.sln /t:Rebuild` sin restore y verificar exit code 0; registrar el comando y el resultado en design.md
- [ ] 3.2 Arrancar el `TpIDS.exe` de ese clon y abrir Dashboard, seleccion de ubicacion de comercio y una pantalla Krypton; verificar que no hay excepciones de carga
- [x] 3.3 Verificar que ningun proyecto de `TpIDS.sln` contiene `PackageReference`, `packages.config` ni `NuGetPackageRoot` con `rg`
- [ ] 3.4 Ejecutar `tests/tools/run-tests.ps1 -IncludeIntegration` y verificar que queda en verde con los contadores previos

## 4. Cierre

- [x] 4.1 Actualizar `tests/README.md` y el apartado de portabilidad de AGENTS.md y verificar que ya no declaran a UI como incumplidora
- [x] 4.2 Marcar la tarea 5.2 de `migrar-json-system-text-sin-nuget` como resuelta por este cambio, anotando el nombre, y verificar con `openspec list`
