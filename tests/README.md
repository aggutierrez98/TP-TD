# Tests de TpIDS

## Dos soluciones, a proposito

- **`TpIDS.sln` (raiz)**: solo los 6 proyectos productivos. No usa ningun paquete NuGet,
  por lo que compila y corre en cualquier maquina copiando el repo, sin internet ni
  pasos extra en Visual Studio.
- **`tests/TpIDS.Tests.sln`**: productivos + proyectos de test. Los tests usan NuGet
  (MSTest, FlaUI), asi que esta solucion se compila solo en maquinas de desarrollo
  con acceso a NuGet (o cache local). **Nunca** referenciar proyectos de test ni FlaUI
  desde los proyectos productivos.

## Proyectos

| Proyecto | Que prueba | BD |
| --- | --- | --- |
| `SERVICIOS.Tests` | Logica transversal pura (hash, DVH/DVV, permisos, idioma, sesion) | No |
| `TpIDS.UiTests` | Casos de uso reales end-to-end (FlaUI + SQL) | `TpCatedraIDS_Test` |

> En esta rama no hay `BLL.Tests` ni `TpIDS.IntegrationTests`: la arquitectura simple
> (BLL con `new` directo, sin inyeccion ni CompositionRoot) no es unit-testeable y esa
> simplicidad se preserva a proposito. La cobertura de BLL/DAL llega end-to-end via UI.

## Como correr

```powershell
# Unit tests (sin BD)
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1

# + integracion (prepara TpCatedraIDS_Test desde script_bd.sql)
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -IncludeIntegration

# + tests UI end-to-end (FlaUI)
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -IncludeUi

# Solo los tests UI de un caso (T0X, o N0X futuro); implica -IncludeUi.
# Acepta varios prefijos y tambien un metodo puntual (ej: T04_CU01).
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -UiFilter T04
powershell -NoProfile -ExecutionPolicy Bypass -File tests\tools\run-tests.ps1 -UiFilter T04,T05
```

`-UiFilter` filtra por prefijo de nombre (`FullyQualifiedName~.<valor>`), asi que
funciona para cualquier convencion futura mientras la clase o el metodo arranquen
con ese prefijo. Los unit tests siempre corren completos (tardan ~1s y actuan de
gate rapido); si el filtro no matchea ningun test UI, el runner falla en vez de
reportar verde vacio.

> Para agentes de IA: el flujo TDD y el gate de cierre estan definidos en
> `.agents/skills/strict-tdd/SKILL.md` y `.agents/skills/test-suite-verification/SKILL.md`.

## Tests UI (`TpIDS.UiTests`)

- Lanzan una **copia staged** de `TpIDS.exe` cuyo `.exe.config` apunta a
  `TpCatedraIDS_Test`; el `UI/App.config` productivo no se toca.
- Requieren: SQL Server local, sesion de escritorio interactiva **desbloqueada**
  (FlaUI mueve ventanas reales) y la BD de test preparada
  (`run-tests.ps1 -IncludeUi` la prepara solo).
- Datos seed de `script_bd.sql` que la suite asume: usuarios `admin` (grupo `admin`,
  todos los permisos) y `user` (grupo `basico`), e idioma default `Español` con las
  traducciones de `translation-seeds/base.es.json` (los asserts de mensajes comparan
  contra la tabla `Traduccion` del idioma default, no contra textos hardcodeados).
- En el primer arranque sobre BD virgen, la suite siembra passwords conocidas para
  `admin`/`user` (con el `PasswordHasher` real, antes de que se inicialicen las
  firmas DVH/DVV) y un usuario inactivo de prueba (`uitest_inactivo`).
- Las escrituras de negocio van siempre por la UI para no romper DVH/DVV; el SQL
  directo se usa solo para asserts (SELECT).
- Nomenclatura: `T0X_CUYY_Escenario`, trazable a `diagramas/T0X-*/...casos-de-uso.json`.
- Ante un fallo queda captura de pantalla en `TestResults/ui-screenshots/`.

### Cobertura por requisito tecnico

| Archivo | Requisito | CU cubiertos |
| --- | --- | --- |
| `SmokeTests.cs` | Harness + arranque | Inicializacion de firmas (T07 parcial) |
| `T02_LoginLogoutTests.cs` | T02 | CU01 Login (+4 alternativos), CU02 Registro (+duplicado, +campos vacios), CU03 Logout (+cancelacion) |
| `T04_UsuariosPermisosTests.cs` | T04 | CU01 Gestionar permisos (+grupo vacio, nombre vacio/con punto, duplicado en catalogo, permiso sin seleccion, ciclo entre grupos), CU02 Asignar perfiles (+acciones sin usuario seleccionado), CU03 Control de acceso (admin/basico), baja logica/reactivacion y borrado fisico (+cancelaciones, +alta sin password) |
| `T05_MultidiomaTests.cs` | T05 | CU01/CU02 alta+rename (+duplicado, +rename a nombre existente), CU03 eliminar (+proteccion default, +cancelacion), CU06 editar traduccion (+valor vacio), CU07 preferencia (+idioma incompleto). CU04/CU05 (import/export con dialogos nativos) quedan sin cobertura automatizada en esta rama |
| `T06_BitacoraHistorialTests.cs` | T06a/T06b | CU01 Consultar bitacora (filtros usuario/nivel vs SQL), historial de versiones y restauracion (+sin seleccion, +cancelacion) |
| `T07_IntegridadTests.cs` | T07 | Corrupcion horizontal (DVH) y vertical (DVV) -> pantalla de contingencia; restauran las firmas al terminar |

Flujos alternativos que quedan fuera de la UI a proposito: los "Xa: excepcion tecnica"
(requieren tirar la infraestructura), los "sin seleccion" de combos con DataSource (auto-
seleccionan siempre) y el re-chequeo de permiso con boton deshabilitado (rama defensiva).
En esta rama el ciclo entre grupos (T04 CU01 6a) se valida al guardar el catalogo
(ValidarSinCiclos en PermisoService), no al agregar la relacion.
