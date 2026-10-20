# Tasks

El progreso de este change vive solo en este archivo. Los cambios de comportamiento en BLL, REPOSITORIO y DAL siguen la skill `strict-tdd`: unitarios en `BLL.Tests`, integracion en `TpIDS.IntegrationTests`. Los tests UI FlaUI van despues del desarrollo.

## 1. Esquema y persistencia

- [x] 1.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline y anotar exit code y contadores del TRX. Baseline: exit 0, unit 814/814, integracion 192/192.
- [x] 1.2 Agregar a `script_bd.sql` la tabla `ZonaUsuario` y el permiso `gestionzonasusuario` asignado a `basico` y `admin`; aplicar el DDL y el sembrado en `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar por consulta tabla, restricciones y asignaciones en ambas.
- [x] 1.3 Escribir tests de integracion de `ZonaUsuarioDAL`/`ZonaUsuarioRepositorio` (insertar, listar por usuario, eliminar, `UNIQUE` por usuario) y verlos fallar; implementar BE, DAL (skill `dal-sql-access-patterns`) y REPOSITORIO y verlos en verde.

## 2. Reglas y distancia

- [x] 2.1 Tests de `DistanciaGeografica` con distancias conocidas (mismo punto, Obelisco a Plaza de Mayo, antipodas) en rojo; implementar y verlos en verde.
- [x] 2.2 Tests de `ZonaUsuarioBLL` (alta valida, nombre vacio, repetido sin distinguir mayusculas, radio fuera de rango, coordenadas invalidas, tope de 10, baja propia, baja de zona ajena rechazada, listado solo del usuario de la sesion, zona mas cercana y pertenencia al radio) en rojo; implementar y verlos en verde.
- [x] 2.3 Dar de alta `ZonaUsuarioBLL` en `CompositionRoot` segun `composition-root-guard` y verificar la guardia compilando.

## 3. Menu y pantalla

- [x] 3.1 Renombrar `mnuSesion` a `mnuMiUsuario` con la key `FormMainShell.Menu.MyUser`, agregar `mnuMisZonas` habilitado por `gestionzonasusuario` y ajustar los tests UI que usan el menu viejo; verificar compilando y abriendo la app con un usuario basico.
- [x] 3.2 Crear `FormMisZonas` (Krypton, MDI unico, grilla, panel de alta, busqueda, radio, mapa con circulos, marcadores y etiquetas) segun `winforms-ui-control-naming`, registrarlo en `UI.csproj`; verificar alta por busqueda, alta por clic y baja con una prueba manual y captura.
- [x] 3.3 Agregar las keys nuevas a `base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync`, insertarlas en `Palabra`/`Traduccion` de ambas bases y retirar `FormMainShell.Menu.Session` si queda sin uso; verificar por consulta en ambos idiomas.
- [x] 3.4 Agregar `tests/TpIDS.UiTests/FIN_MisZonasTests.cs` (alta, nombre repetido, baja, menu deshabilitado sin permiso, textos traducidos); avisar al usuario que toma el escritorio, correr con `-UiFilter` tras su confirmacion y verificar en el TRX.

## 4. Cierre

- [x] 4.1 Compilar `TpIDS.sln` con MSBuild (skills `winforms-build-verification` y `architecture-dependency-guard`); verificar exit code 0.
- [x] 4.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`); verificar exit code 0 y reportar contadores reales. Resultado (con `-IncludeUi`): exit 0, unit 849/849, integracion 198/198, UI 134 pasados + 1 omitido de 135.
- [x] 4.3 Registrar en `openspec/changes/actualizar-diagramas-tp/tasks.md` el pendiente de G07 (`ZonaUsuario`, DAL, repositorio, BLL, `DistanciaGeografica`, `FormMisZonas`) y G08 (`ZonaUsuario` con FK a `Usuarios`), y el CU de Mis zonas a acordar con `tp-uml-y-casos-de-uso`; verificar que quedo registrado.
