# Estado actual del proyecto y proximos pasos (FINAZAPP / TpIDS-TD)

Documento de traspaso entre sesiones. Generado el 2026-08-26. Leer esto **antes** de tocar
codigo, diagramas o documentacion si se retoma el trabajo en una sesion nueva: da el contexto
completo (que es este proyecto, por que se esta haciendo lo que se esta haciendo, donde vive
cada cosa en el repo) sin tener que re-derivarlo de cero.

## 1. Que es este proyecto

`TpIDS-TD` es el repositorio del Trabajo de Campo / Trabajo de Diploma (TC/TD) de Agustin
Gutierrez en la Universidad Abierta Interamericana (UAI), Facultad de Tecnologia
Informatica. Es un proyecto articulado: arranca en la materia Metodologia de Desarrollo de
Sistemas I, sigue en Metodologia II y Trabajo de Campo I, y termina en Trabajo de Diploma. La
catedra exige entregar todo el trabajo siguiendo una **"Carpeta Modelo"** (documento oficial
de la catedra, aportado por el usuario como PDF: "Carpeta modelo TC y TD 2020") que organiza
el proyecto completo en 5 grandes grupos de items, cada uno con su propio codigo:

| Grupo | Que es | Estado en este repo |
| --- | --- | --- |
| **G0X** | Descripcion global del producto (proposito, alcance, roles, glosario) + diagramas de integracion (clases y DER de todos los modulos) | **En progreso** (ver seccion 4) |
| **N0X** | Procesos de negocio: especificacion funcional + casos de uso del dominio real del sistema | **NEG01-movimientos y NEG02-categorias implementados en codigo** (27/08/2026): Movimientos financieros y Categorias, 7 capas completas + TDD real + shell MDI. Diagramas/especificacion de CU (artefactos N01/N02 de cada proceso) todavia no generados (ver seccion 4bis) |
| **T0X** | Aspectos tecnicos transversales que provee el sistema (arquitectura, login, encriptado, perfiles, idiomas, bitacora, digitos verificadores, backup) | **Implementado** (T01-T08 en codigo) |
| **A0X** | Caracteristicas adicionales: instalador, exportacion PDF, serializacion | **No empezado** |
| **D0X** | Documentacion adicional: manual de usuario, ayuda en linea, guias de instalacion | **No empezado** |

El detalle exacto de que exige cada item de la carpeta modelo esta en el PDF que aporto el
usuario (no versionado en el repo, vive en su Desktop de Windows); el resumen operativo para
uso diario esta en `.agents/skills/tp-uml-y-casos-de-uso/references/catalogo-requisitos.md`.

### Por que existe FINAZAPP

El sistema que se viene construyendo desde hace tiempo (T01-T07) es solo el **esqueleto
tecnico transversal**: login, gestion de usuarios, permisos, bitacora, idiomas, integridad de
datos. El usuario senalo (26/08/2026) que, tal como esta hoy, la pantalla que se abre despues
del login es directamente `FormGestionUsuarios` — es decir, la app *es* el panel de
administracion. Eso no tiene sentido para un sistema real: gestion de usuarios/roles/
bitacora/idiomas/integridad **deberian ser pantallas de administrador**, no la aplicacion en
si.

La aplicacion real que un usuario final va a usar es **FINAZAPP**, una plataforma de gestion
de finanzas personales (Personal Finance Management). El usuario aporto una presentacion
(PDF "Presentacion con plantilla") y una descripcion de negocio detallada con el pitch, el
problema que resuelve (gente que no lleva registro de sus finanzas, descuentos bancarios que
nunca se aprovechan), la propuesta de valor (centralizacion de movimientos, motor de
descuentos geolocalizado, asistencia con IA) y el alcance de negocio completo (dentro/fuera de
alcance). Ese contenido es la base de G00-G05 (ver seccion 4).

**Decision de alcance del nucleo de negocio (N0X)**, acordada con el usuario y **NEG01-movimientos
y NEG02-categorias ya implementados el 27/08/2026** (ver seccion 4bis para el detalle completo):
- Alcance de codigo real: carga manual de ingresos/egresos (NEG01-movimientos, implementado),
  categorias globales+personalizadas (NEG02-categorias, implementado) y precarga opcional de
  movimientos desde una foto de ticket mediante Gemini (20/09/2026). Presupuesto simple y dashboard
  con KPIs quedan para procesos posteriores (todavia no implementados).
- Explicitamente fuera del codigo (aunque si documentado como vision de producto en G02):
  sincronizacion bancaria real, scraping de descuentos y geolocalizacion. El escaneo de tickets es
  la excepcion acordada: usa Gemini multimodal por API para devolver importe, fecha, descripcion y
  texto detectado; sigue requiriendo revision y confirmacion manual del usuario antes de persistir.
- Reestructuracion de navegacion: **implementada**. Shell principal MDI (`FormPrincipal`) donde
  todo usuario autenticado aterriza en `FormMovimientos` (si tiene el permiso, que el usuario
  basico tiene desde el alta), con menus "Sesion" (config propia + logout), "Finanzas"
  (Movimientos/Categorias) y "Administracion" (Usuarios/Permisos/Idiomas/Bitacora, visible solo
  con permiso). `FormGestionUsuarios` dejo de ser la pantalla post-login: perdio los botones que
  ahora son items de menu de `FormPrincipal` (Configuracion, Gestion de Idiomas, Gestion de
  Permisos, Bitacora, Logout) y conserva solo el CRUD de usuarios + arbol de permisos +
  Historial de usuario (que sigue siendo contextual a la fila seleccionada).

## 2. Estado actual del codigo (que ya existe, que no)

Arquitectura implementada: **7 capas** con dependencias validadas automaticamente en el build
(`architecture-dependency-guard`) y con alta de servicios centralizada en `CompositionRoot`
(`composition-root-guard`):

```
UI          -> ABSTRACCIONES, BLL, BE, SERVICIOS
BLL         -> ABSTRACCIONES, REPOSITORIO, BE, SERVICIOS
REPOSITORIO -> ABSTRACCIONES, DAL, BE, SERVICIOS
DAL         -> ABSTRACCIONES, BE, SERVICIOS
BE          -> ABSTRACCIONES
SERVICIOS   -> ABSTRACCIONES
ABSTRACCIONES -> (nada; nucleo de contratos)
```

Modulos tecnicos ya implementados (numeracion **de este repo**, ver nota de numeracion mas
abajo):

| Codigo (repo) | Modulo | Patron/tecnica esperada |
| --- | --- | --- |
| T01 | Arquitectura base, persistencia, navegacion | 7 capas, MDI (pendiente el shell real, ver seccion 1) |
| T02 | Login/Logout y usuarios | Singleton (sesion) |
| T03 | Encriptado (hash de contrasenas) | PBKDF2 |
| T04 | Perfiles/permisos de usuario | Composite + recursion (TreeView) |
| T05 | Multiples idiomas | Observer, sin resource files estaticos |
| T06a | Bitacora (auditoria) | — |
| T06b | Control de cambios (historial de usuario) | — |
| T07 | Digitos verificadores e integridad | Verificacion al arranque, antes del login |
| T08 | Backup y restauracion de base de datos | BACKUP/RESTORE DATABASE nativo de SQL Server |

**Nota de numeracion:** la carpeta modelo oficial numera T07=Backup y T08=Digitos
verificadores; este repo fusiono/renumero y usa T07 para digitos verificadores y T08 para
Backup (decidido con el usuario el 27/08/2026, ver seccion 6). No es un error, es una
decision de numeracion de este repo — no tratar de "corregir" la numeracion sin discutirlo
con el usuario primero.

Nucleo de negocio de finanzas personales: **NEG01-movimientos y NEG02-categorias implementados**
el 27/08/2026 (7 capas + TDD real + shell MDI, ver seccion 4bis). Lo que **todavia no existe**
en el codigo:
- Presupuesto simple y dashboard con KPIs (procesos de negocio posteriores a NEG01/NEG02, no
  arrancados).
- Instalador, exportacion PDF, serializacion (A0X).
- Manual de usuario, ayuda en linea, guias de instalacion (D0X).

## 3. Mapa del repositorio (donde esta cada cosa)

```
TpIDS-TD/
├── AGENTS.md                        <- punto de entrada: indice de skills y reglas
├── ESTADO-ACTUAL-Y-PROXIMOS-PASOS.md <- este documento
├── TpIDS.sln                        <- solucion completa (compilar antes de cerrar cualquier tarea de codigo)
├── script_bd.sql / setup-database.ps1 <- script e instalacion de la base SQL Server local
│
├── ABSTRACCIONES/   BE/   BLL/   DAL/   REPOSITORIO/   SERVICIOS/   UI/
│                                   <- las 7 capas de la arquitectura (una carpeta = un .csproj)
│
├── tests/
│   ├── BLL.Tests/  SERVICIOS.Tests/        <- unit tests (MSTest)
│   ├── TpIDS.IntegrationTests/             <- integracion (REPOSITORIO/DAL contra SQL Server real)
│   └── TpIDS.UiTests/                      <- FlaUI, smoke tests de UI (se corren aparte, avisando antes)
│
├── .agents/
│   ├── 01-stack-dotnet-winforms.md         <- stack y restricciones tecnicas
│   ├── 02-architecture-7-layers.md         <- matriz de dependencias entre capas
│   └── skills/
│       ├── tp-uml-y-casos-de-uso/          <- skill CENTRAL para diagramas y casos de uso
│       │   └── references/catalogo-requisitos.md  <- fuente unica: que exige cada requisito, donde va, como se nombra
│       ├── tp-delivery-standards/          <- restricciones de entrega, plan de entregas resumido
│       ├── tp-use-case-spec/               <- genera especificaciones de CU (.docx + .md) desde JSON
│       ├── architecture-dependency-guard/, composition-root-guard/  <- guards validados en build
│       └── (resto: winforms, dal-sql-access-patterns, dotnet-*, csharp-*, etc.)
│
└── diagramas/
    ├── TP-especificaciones-casos-de-uso.docx / .md   <- documento CONSOLIDADO de todos los CU (todo el repo)
    ├── TODO-actualizacion-diagramas.md                <- plan detallado del trabajo pendiente de la seccion 5
    ├── G00-descripcion-global-producto/               <- G01-G05 (fuente .md), escrito 26/08/2026
    ├── G07-clases-por-capa/                           <- un .puml de clases por cada capa (parcial, "implementado")
    ├── G08-modelo-datos-integrado/                    <- DER unico integrando todos los modulos
    └── T0X-<slug>/                                    <- una carpeta por requisito tecnico, cada una con:
                                                            <req>-clases.puml, <req>-datos.puml (DER),
                                                            <req>-secuencia.puml (general),
                                                            <req>-casos-de-uso.json (+ .docx/.md generados),
                                                            <req>-CUNN-<slug>-secuencia.puml (uno por CU)
```

**El documento formal para la catedra vive fuera del repo**, en Windows (no se versiona en
git):

```
C:\Users\agust\Desktop\Trabajo Final TD Gutierrez.docx
```

Desde WSL esa misma ruta es `/mnt/c/Users/agust/Desktop/Trabajo Final TD Gutierrez.docx` (el
mount de `C:` dentro de WSL). Es el documento que la catedra realmente va a leer: ahi se pega
el texto de G0X, las imagenes renderizadas de los `.puml`, y las especificaciones de CU. Su
caratula dice "Materia: INGENIERIA DE SOFTWARE / Alumnos: Ian Maldonado, Agustin Gutierrez"
de una etapa anterior (compartida con un companero); para esta etapa (TC/TD) el usuario
confirmo que la autoria es **solo Agustin Gutierrez** — no "corregir" la caratula vieja sin
que el usuario lo pida.

**Notas tecnicas de edicion de ese Word desde WSL** (aprendido el 26/08/2026, por si hay que
volver a tocarlo):
- No hay LibreOffice/Word en WSL; se edita con la libreria `python-docx` (instalada en un
  venv local, no en el sistema — `python3 -m venv .../venv && venv/bin/pip install
  python-docx`, porque el Python del sistema es "externally managed").
- Si el archivo esta abierto en Word en Windows, cualquier intento de guardado desde WSL tira
  `PermissionError` — hay que pedirle al usuario que lo cierre primero.
- Para reabrirlo en Windows desde WSL: `cmd.exe /c start "" "C:\ruta\al\archivo.docx"`
  (`explorer.exe` directo no siempre funciona en esta sesion).
- El documento usa convenciones propias que hay que respetar al escribir contenido nuevo:
  estilo `Heading 2` para titulos de modulo (T01, T02, ... y ahora G00), un patron de
  "etiqueta" en 12pt plano (sin negrita) para sub-secciones tipo `T0X.Y Label:`, negrita
  inline para sub-sub-etiquetas, viñetas reales reutilizando una numeracion de lista ya
  existente en el documento (`numId=6`, nivel 0, simbolo `●`), y un estilo de tabla compartido
  (`Table1`, sin sombreado, solo bordes). Pegar Markdown crudo desde el chat **rompe todo esto**
  (pierde tablas, aplana listas, deja literalmente `###`/`**` como texto) — por eso ahora se
  edita programáticamente con `python-docx`, no por copy-paste.

## 4. Que se hizo en la sesion del 26/08/2026 (y por que)

1. El usuario pidio continuar el TP hacia un sistema real (FINAZAPP), senalando el problema de
   la pantalla principal (ver seccion 1). Se leyeron los dos PDF que aporto: la presentacion
   de FINAZAPP y la Carpeta Modelo TC/TD 2020, y se reviso el catalogo de requisitos ya
   existente en el repo (que hasta ese momento solo cubria T0X y G06/G07, no G00-G05 ni N0X/
   A0X/D0X).
2. Se pregunto y acordo con el usuario: alcance de negocio MVP para cuando arranque el codigo
   de N0X, confirmacion conceptual de la reestructuracion de navegacion, y que NEG01
   (el primer proceso de negocio a modelar) arranque solo con "Gestion de movimientos
   financieros". **Pero luego el usuario re-prioritizo**:
   dijo que por ahora solo le interesan los puntos **G0X**, ni siquiera N0X/A0X/D0X — esa es
   la decision vigente (ver seccion 1).
3. Se establecio una distincion clave, ahora documentada tambien en
   `catalogo-requisitos.md`: **G01-G05 describen FINAZAPP como producto completo** (toda la
   vision de negocio, aunque no este codeada todavia), mientras que **G06/G07 deben reflejar
   solo lo que ya esta implementado en codigo** (la propia Carpeta Modelo los define como
   "parcial de todos los modulos **implementados**"). Por eso G06/G07 quedaron **sin tocar**
   esta sesion.
4. Se escribio `diagramas/G00-descripcion-global-producto/G00-descripcion-global-producto.md`
   con G01 (Proposito), G02 (Descripcion funcional y Alcance, con las gestiones de negocio y
   tecnicas, y el detalle de dentro/fuera de alcance que aporto el usuario), G03 (Definiciones/
   Acronimos/Abreviaturas), G04 (Roles) y G05 (Otros Requisitos).
5. El usuario peg  este contenido en su Word y aviso que quedo roto: Google Docs/Word no
   convirtio bien el Markdown (tablas truncadas o perdidas del todo, listas aplanadas sin
   viñeta, texto literal `###`/`####` sobreviviendo junto al estilo de titulo). Se
   diagnostico el problema recorriendo el XML del `.docx` con `python-docx`, se identifico la
   convencion de estilos que ya usaba el resto del documento (ver seccion 3), se borro el
   bloque roto completo y se reconstruyo programaticamente respetando esa convencion.
6. El usuario reviso el resultado y pidio un ajuste puntual: G04 tenia la tabla de personas
   participantes en el desarrollo, pero le faltaba la tabla de **roles del sistema**. Aporto
   la descripcion detallada de los 3 roles reales (Administrador, Operador, Usuario/usuario
   comun) con sus responsabilidades y accesos especificos. Se reconstruyo la tabla de roles de
   G04 con esos 3 roles, y se corrigio de paso un bullet de G02.A que todavia mezclaba
   "Administrador / Operador" como un unico rol (quedaba contradictorio con la tabla nueva).
7. Se aprendio en el camino la mecanica de edicion Windows/WSL del Word (ver notas tecnicas en
   la seccion 3): hay que cerrar el archivo en Word antes de que un script desde WSL pueda
   guardarlo, y se puede reabrir con `cmd.exe /c start`.

**Resultado al cierre de esta sesion:** G00-G05 completo y aprobado por el usuario en el Word.
G06/G07, N0X, A0X y D0X quedan sin tocar, a proposito.

## 4bis. Que se hizo en la sesion del 27/08/2026: NEG01-movimientos, NEG02-categorias y shell MDI

Sesion larga en la misma jornada: primero se cerro la revision de diagramas T0X/G0X (ver 5.1,
ya reescrita), y despues el usuario pidio arrancar NEG01-movimientos y NEG02-categorias mas la
reestructuracion de navegacion. Se armo un plan (`EnterPlanMode`) y el usuario lo aprobo.

**Hallazgo clave de la sesion:** a diferencia de la sesion anterior de diagramas, esta sesion
WSL **si tuvo acceso real** a las herramientas Windows via interop (`powershell.exe` desde
bash): `MSBuild.exe`, `dotnet.exe`, y **SQL Server Express corriendo localmente**
(`TpCatedraIDS` y `TpCatedraIDS_Test` accesibles por `sqlcmd`). Esto permitio hacer TDD real
(rojo observado por compilacion fallida, no asumido) y compilar/testear de verdad, algo que la
sesion de diagramas no pudo hacer. **Si se retoma trabajo de codigo en una sesion nueva, probar
primero si `powershell.exe`/`sqlcmd` responden** antes de asumir que hace falta pedirle al
usuario que corra las verificaciones del lado Windows.

Decisiones de diseno tomadas con el usuario antes de programar:
- **Categoria**: catalogo global (`UsuarioId` NULL, precargado: Alimentacion, Transporte,
  Vivienda, Salud, Entretenimiento, Sueldo, Otros Ingresos, Otros Egresos) + personalizado por
  usuario. Un usuario solo edita/elimina sus propias categorias, nunca las globales.
- **Estilo de UI**: se siguio el patron ya usado en todo el repo (`DataGridView.DataSource`
  directo + `UiDialogHelper` para validacion/dialogos), **no** el `BindingSource`+`ErrorProvider`
  que documenta `.agents/skills/winforms/SKILL.md` — ese patron no lo usa ninguna pantalla
  existente y se prefirio consistencia visual sobre la letra del skill. Vale la pena revisar
  ese skill si se agregan mas pantallas, por si conviene actualizarlo para que refleje la
  convencion real del repo.
- Orden de implementacion: primero NEG02-categorias (Categoria) y despues NEG01-movimientos
  (Movimiento), por la FK de Movimiento hacia Categoria — al reves del orden en que el usuario
  los nombro, es una dependencia tecnica, no un cambio de prioridad.

Trabajo realizado, en 3 etapas:

1. **Esquema de BD** (`script_bd.sql`): tablas `Categoria` (con indices filtrados
   `UQ_Categoria_Nombre_Global`/`_Usuario` para la unicidad de nombre por scope — requieren
   `SET QUOTED_IDENTIFIER ON` antes de crearse, si no tira el error 1934) y `Movimiento` (FK a
   Usuarios/Categoria, `CHECK` de tipo y monto positivo). Dos permisos nuevos
   (`gestionmovimientos`, `gestioncategorias`) agregados **tanto al grupo `admin` como al grupo
   `basico`** — a diferencia de los permisos tecnicos, NEG01/NEG02 son el producto en si y todo
   usuario los necesita desde el alta. Aplicado y verificado contra `TpCatedraIDS_Test` via
   `tests/tools/prepare-test-database.ps1` (ese script tiene un guard explicito que impide
   tocar la base real `TpCatedraIDS`, asi que la base de trabajo del usuario no fue tocada).
2. **NEG02-categorias (Categoria) y NEG01-movimientos (Movimiento), 7 capas completas con TDD real** (rojo por compilacion
   observado antes de cada implementacion, luego verde):
   - `BE/Categoria.cs`, `BE/Movimiento.cs`. El enum `TipoMovimiento` **vive en ABSTRACCIONES**
     (`ABSTRACCIONES/TipoMovimiento.cs`), no en BE: lo necesitan tanto `BE.Movimiento` como
     `SERVICIOS.FiltroMovimiento`, y SERVICIOS no puede depender de BE segun la matriz de
     capas — error real que se cometio y corrigio en el momento.
   - `REPOSITORIO`/`DAL` para ambas entidades, cableadas en `IUnidadDeTrabajo`/
     `UnidadDeTrabajoSqlServer`. Tests de integracion nuevos en
     `tests/TpIDS.IntegrationTests/CategoriaRepositorioIntegrationTests.cs` y
     `MovimientoRepositorioIntegrationTests.cs` (patron transaccion-por-test con
     `RollbackTransactionScope()`, que no existia como precedente en el repo — se establecio
     ahi). Requirio agregar `InternalsVisibleTo("TpIDS.IntegrationTests")` en
     `REPOSITORIO/Properties/AssemblyInfo.cs` (no existia) y sumar la referencia a `DAL` en
     `TpIDS.IntegrationTests.csproj` (tampoco existia).
   - `BLL/CategoriaBLL.cs` y `BLL/MovimientoBLL.cs`: ownership checks (un usuario solo edita/
     elimina lo suyo), `MovimientoBLL` valida que la categoria elegida sea visible para el
     usuario actual (global o propia), logging a bitacora en toda alta/edicion/baja. Registrados
     en `CompositionRoot` (`CompositionRoot.Categoria`, `CompositionRoot.Movimiento`).
   - `UI/FormGestionCategorias.cs` y `UI/FormMovimientos.cs` (+ `.Designer.cs`), estilo
     `FormGestionIdiomas`/`FormBitacora`. Se agrego el patron `FakeCategoriaRepositorio`/
     `FakeMovimientoRepositorio` en `tests/BLL.Tests/TestDoubles/Fakes.cs`.
   - Resultado de tests al cierre: **131 unit tests, 12 integration tests, todos verdes**.
3. **Reestructuracion de navegacion (shell MDI)**: `UI/FormPrincipal.cs` (+`.Designer.cs`),
   nuevo `MenuStrip` con tres menus — "Sesion" (Mi Configuracion + Cerrar sesion, siempre
   visible), "Finanzas" (Movimientos/Categorias, aterrizaje automatico en Movimientos si el
   usuario tiene permiso) y "Administracion" (Usuarios/Permisos/Idiomas/Bitacora, visible solo
   con permiso tecnico). `FormGestionUsuarios` perdio los botones Configuracion/Gestion
   Idiomas/Gestion Permisos/Bitacora/Logout (se movieron al menu de `FormPrincipal`); conserva
   el CRUD de usuarios + arbol de permisos + Historial de usuario (sigue contextual a la fila
   seleccionada). `FormLogin` ahora abre `FormPrincipal` en vez de `FormGestionUsuarios`, y de
   paso se corrigio que **no se ocultaba a si misma** al loguear (bug preexistente: quedaba
   visible detras de la siguiente pantalla) — ahora hace `Hide()` al entrar y `Show()` al
   volver despues del logout.

Verificacion final: `TpIDS.sln` completo compila limpio (guards de arquitectura y Composition
Root en verde), suite completa (`run-tests.ps1 -IncludeIntegration`) en verde.

**Actualizacion (mismo dia, mas tarde):** el usuario confirmo aplicar el esquema nuevo a la base
real `TpCatedraIDS` (via `setup-database.ps1 -Force`, verificado: 8 categorias, permisos y
usuarios seed OK), y despues pidio terminar los tests de UI (FlaUI) que este refactor de
navegacion habia dejado rotos. Se encontraron y arreglaron:

- **Causa raiz de la rotura**: el helper compartido `UiTestBase.LoginComo` esperaba
  `FormGestionUsuarios` justo despues del login (ahora es `FormPrincipal`), y varios tests
  clickeaban directamente los 5 botones removidos de `FormGestionUsuarios`. Se agregaron
  helpers de navegacion por menu (`AbrirGestionUsuarios`, `AbrirGestionPermisos`,
  `AbrirGestionIdiomas`, `AbrirBitacora`, `AbrirConfiguracion`, `CerrarSesionDesdeMenu`,
  `AbrirMovimientos`, `AbrirCategorias`) y se actualizaron `T02`/`T04`/`T05`/`T06` para usarlos.
  `T04_CU03` (control de acceso admin/basico) se rediseño: ahora verifica el estado
  enabled/disabled del menu "Administracion" en `FormPrincipal` en vez de botones individuales
  de `FormGestionUsuarios` (un usuario basico ya ni puede abrir esa pantalla, al no tener
  el item de menu habilitado).
- **Dos bugs reales de infraestructura de tests descubiertos y corregidos** (no eran solo
  "actualizar referencias"), verificados empiricamente corriendo la app de verdad:
  1. Los `ToolStripMenuItem` de un `MenuStrip` no exponen `AutomationId` via UIA (
     `PropertyNotSupportedException`): `UiFind.ClickMenu`/`MenuTopHabilitado` ahora matchean
     por texto visible (`Name`, resuelto via `TestDatabase.ObtenerTraduccion`), no por
     AutomationId, y accediendo dentro de un `Menu.Items`/`MenuItem.Items` (no
     `FindFirstDescendant`, que no ve los items de un dropdown).
  2. `UiAppSession.AgregarVentanas` (el buscador de ventanas del proceso bajo test) filtraba
     la recursion por `ControlType.Window` en cada nivel; los hijos MDI cuelgan de un
     `MdiClient` nativo que no es `ControlType.Window`, asi que la busqueda nunca llegaba a
     `FormGestionUsuarios`/etc. abiertos como hijos MDI (aunque la app los abria bien: se
     confirmo con una captura de pantalla del fallo). Se cambio a recorrer todos los hijos sin
     filtrar por tipo, agregando solo los que si son `Window`.
- **Resultado final: 131 unit tests + 40 UI tests (FlaUI), todos verdes**, corridos de verdad
  contra la app compilada (no solo compilacion). Integration tests (12) no se re-corrieron en
  esta ronda porque no se toco REPOSITORIO/DAL; ya habian quedado verdes antes.

**Pendiente, no se hizo esta sesion:**
- Diagramas UML y especificacion de CU (artefactos N01/N02 de cada proceso: NEG01-movimientos,
  NEG02-categorias; skill `tp-uml-y-casos-de-uso`): no se
  generaron a proposito — esa skill se ejecuta a demanda, recien cuando el usuario lo confirme
  explicitamente.
- Presupuesto simple y dashboard con KPIs (procesos de negocio posteriores, no arrancados).

## 5. Que falta y como continuar

### 5.1. Diagramas UML desactualizados (T0X, G06, G07) — revisado y corregido el 27/08/2026

Se detecto (26/08/2026) que algunos `.puml` bajo `diagramas/` — y las imagenes ya pegadas en el
Word — habian quedado con **arquitectura vieja**: se hicieron antes o durante la migracion a las
7 capas (la capa REPOSITORIO se agrego recien en el commit `23db1f8`, del 2026-06-23) y no
reflejaban refactors de codigo posteriores (alta de `CompositionRoot`, guards de arquitectura, el
renombre `BootstrapAplicacionBLL` -> `InicializadorAplicacionBLL`, cambios en
`PermisoBLL`/`UsuarioBLL`/`IntegridadBLL`).

**Se reviso carpeta por carpeta el 27/08/2026** (T01 a T07, G06, G07) contra el codigo real. Se
corrigieron T02, T03 (le faltaba la secuencia general, se creo), T05, T06b (tenia un CU04
duplicado con arquitectura vieja, se elimino el duplicado) y T07 (la referencia a
`BootstrapAplicacionBLL` detectada el 26/08 vivia en su secuencia general, se reescribio
completa). T01, T04, T06a, G06 y G07 ya estaban al dia, sin cambios. El detalle carpeta por
carpeta de que se cambio y por que esta en **`diagramas/TODO-actualizacion-diagramas.md`**.

Pendiente que quedo abierto (detalle completo en ese mismo archivo):
1. Reemplazar las imagenes viejas por las nuevas en el Word de catedra (fuera del repo, en
   Windows) — no se toco porque no se confirmo que el archivo estuviera cerrado.
2. Los DER se verificaron cruzando contra `script_bd.sql` (sin acceso a `sqlcmd`/SQL Server real
   en esta sesion tampoco); falta la verificacion contra la base en vivo.
3. T02, T04, T05 y T06a no tienen `-secuencia.puml` general (solo secuencias por CU, que cubren
   el 100% del comportamiento) — el catalogo lo pide igual como generales de todo modulo con CU.
   Se dejo sin crear a proposito, pendiente de confirmar con el usuario si vale la pena el
   artefacto extra.
4. No se pudo compilar `TpIDS.sln` (sin `dotnet`/`msbuild` en esta sesion WSL); no se toco ningun
   `.cs`/`.csproj`, asi que el riesgo es minimo, pero falta correr la verificacion de build.

### 5.2. Nucleo de negocio (N0X) y resto (A0X, D0X)

Deferidos a pedido explicito del usuario (ver seccion 1). Cuando el usuario pida retomarlos,
ya hay decisiones tomadas que no hace falta volver a preguntar: alcance MVP (movimientos
manuales y escaneo opcional con Gemini, categorias, presupuesto, dashboard — sin sincronizacion
bancaria/scraping/geolocalizacion), primer proceso de negocio a modelar ("Gestion de movimientos
financieros"), y la reestructuracion de navegacion (shell MDI + menu Administracion por permiso)
para construir junto con las primeras pantallas de negocio.

### 5.3. Como arrancar una sesion nueva

1. Leer este documento completo.
2. Si se va a tocar codigo o diagramas: leer tambien `AGENTS.md` (indice de skills) y cargar
   la skill que corresponda segun la tabla de auto-invocacion ahi listada.
3. Preguntar al usuario cual de los pendientes de la seccion 5 quiere retomar — no asumir que
   sigue el orden en el que estan listados aca.

## 6. Que se hizo en la sesion del 27/08/2026 (mas tarde): T08 Backup y Restauracion

El usuario pidio implementar Backup y Restore de la BD, con formulario dentro de Admin, y
pregunto cual era el T07/T08 vigente (ver nota de numeracion en la seccion 2: la carpeta
modelo oficial dice T07=Backup, este repo ya tenia T07=Digitos verificadores). Se acordo
usar **T08** para Backup, con `BACKUP DATABASE`/`RESTORE DATABASE` nativo de SQL Server y
seleccion de archivo a mano (sin listado de backups).

**Codigo implementado (7 capas)**: `DAL/BackupDAL.cs` (primer DAL del proyecto que NO usa
`AccesoDatosSqlServer` — SQL Server prohibe `BACKUP`/`RESTORE DATABASE` dentro de una
transaccion explicita, que es como opera el resto del sistema via `IUnidadDeTrabajo`;
`RestaurarBackup` conecta a `master`, hace `SINGLE_USER`/`RESTORE ... WITH REPLACE`/
`MULTI_USER` + `ClearPool`), `REPOSITORIO/IBackupRepositorio.cs`+`BackupRepositorio.cs`,
`BLL/BackupBLL.cs` (primera BLL sin `IUnidadDeTrabajoFactory`), `UI/FormBackup.cs`+
`.Designer.cs` (confirmacion obligatoria antes de restaurar, cierre de sesion forzado
despues), permiso nuevo `gestionbackup` (solo grupo admin) e item de menu en Administracion.
Tests: 10 unit tests de `BackupBLL`, un test de integracion de `CrearBackup`, smoke tests de
UI (FlaUI).

**Hallazgo real durante la verificacion** (no es un bug de codigo, es una restriccion de
SQL Server que hay que tener presente si se usa la pantalla): `BACKUP`/`RESTORE DATABASE`
corren del lado del **servidor** — la carpeta destino tiene que ser escribible por la
**cuenta de servicio de SQL Server** (`NT Service\MSSQL$...`), no por el usuario que corre
la app. En esta maquina de desarrollo, ni la carpeta de usuario (`%TEMP%`) es escribible por
el servicio, ni la carpeta de backup por defecto del servicio (`...\MSSQL\Backup`) es
siquiera *listable* por el usuario interactivo — son cuentas separadas con ACLs cruzadas. El
test de integracion se corrigio para preguntarle a la propia instancia su
`InstanceDefaultBackupPath` y verificar con `xp_fileexist` (via SQL) en vez de `File.Exists`
del lado cliente. **Implicancia para el uso real de `FormBackup`**: si el administrador elige
por `SaveFileDialog` una carpeta bajo su propio perfil de usuario (Escritorio, Documentos),
lo mas probable es que el backup falle con "Acceso denegado" salvo que esa carpeta tenga
permisos de escritura explicitos para la cuenta de servicio de SQL Server (o se use
directamente la carpeta de backup por defecto de la instancia). No se resolvio con codigo
porque hacerlo bien (ajustar ACLs, o correr el motor con otra cuenta) es una decision de
infraestructura fuera del alcance de este TP — queda documentado aca para no sorprender en
la demo.

**Verificado en esta sesion** (WSL con interop a `dotnet.exe`/`sqlcmd.exe` de Windows,
disponibles esta vez): `BLL`/`DAL`/`REPOSITORIO`/`BE`/`SERVICIOS`/`ABSTRACCIONES` compilan
limpio; 85/85 unit tests de `BLL.Tests` en verde; 14/14 integration tests en verde (incluido
el de Backup, contra `TpCatedraIDS_Test` real); `script_bd.sql` actualizado aplicado contra
`TpCatedraIDS_Test` via `prepare-test-database.ps1`, permiso `gestionbackup` confirmado en
grupo admin. **No se pudo compilar `UI.csproj`** con `dotnet build` en esta sesion: falla con
`MSB3822`/`MSB3823` (recursos no-string del `.resx`, `System.Resources.Extensions`
faltante) — es un gap preexistente del proyecto con la tooling de `dotnet build` sobre un
`.resx` con iconos en un proyecto old-style, **no relacionado con este cambio** (se probo con
`msbuild.exe` de VS Build Tools, que no esta instalado en esta maquina). `FormBackup.cs`/
`.Designer.cs` y los cambios en `FormPrincipal` se revisaron a mano (controles declarados y
cableados correctamente) pero no se compilaron de verdad — pendiente confirmar en una maquina
con Visual Studio completo o resolviendo el gap de `System.Resources.Extensions`.

Diagramas y documentacion generados: `diagramas/T08-backup-restore/` completo (clases, DER
sin tabla propia — solo `Bitacora` —, secuencia general + por CU, JSON/`.docx`/`.md`, estado
`Implementado`), `catalogo-requisitos.md` con la seccion T08, `G00-descripcion-global-producto.md`
actualizado, y el documento consolidado `diagramas/TP-especificaciones-casos-de-uso.md`/`.docx`
regenerado (7 requisitos, 20 CU en total).

**Pendiente, no se hizo esta sesion:**
- Compilar y correr `UI.csproj`/FlaUI de verdad (bloqueado por el gap de `System.Resources.Extensions`
  con `dotnet build` — ver arriba).
- Reemplazar las imagenes viejas por las nuevas en el Word de catedra fuera del repo (mismo
  pendiente que la seccion 5.1, ahora tambien con las imagenes nuevas de T08).
- `base.in.json` (ingles) no tiene el grupo `FormMainShell.*` (gap preexistente, no de esta
  sesion): `FormMainShell.Menu.Backup` solo quedo cargado en espanol.

## 7. Sesion del 22/09/2026: Reglas de Categorizacion y Comercio opcional en Movimientos

Se implemento de punta a punta la funcionalidad de Reglas de Categorizacion configurables y Comercio opcional en Movimientos:

- **Base de Datos y Dominio (BE / DAL / REPOSITORIO):**
  - `BE/Movimiento.cs`: se agrego la propiedad `Comercio` opcional.
  - `script_bd.sql`: columna `ComercioId INT NULL` en `dbo.Movimiento`, con FK e indices. Alta del permiso `gestionreglascategorizacion` asignado a los perfiles `admin` y `basico`.
  - `DAL/MovimientoDAL.cs` y `REPOSITORIO/MovimientoRepositorio.cs`: adaptados para persistir e hidratar `Comercio` (o `DBNull.Value` si no se especifica).
  - `REPOSITORIO/UnidadDeTrabajoSqlServer.cs`: expone `ReglasCategorizacion` y `ComercioReglaCategorizacion`.
- **Logica de Negocio (BLL) & CompositionRoot:**
  - `BLL/MovimientoBLL.cs`: metodos `Crear` y `Actualizar` reciben `int? comercioId = null` y validan su existencia si se especifica.
  - `BLL/ReglaCategorizacionBLL.cs`: CRUD completo de reglas de categorizacion y algoritmo deterministico de sugerencia `SugerirCategoria(descripcion, comercioId)`:
    - Normalizacion diacritica y casing.
    - Matching con limites de palabra (`\b<palabra>\b`) para evitar coincidencias parciales accidentales.
    - Desempate por especificidad en 3 niveles (cantidad de palabras coincidentes > cobertura total de texto > regla con coincidencia de comercio especifico). Fallback a `null` ante empate residual o sin coincidencias, obligando a la seleccion manual.
  - `BLL/CompositionRoot.cs`: registro e inyeccion limpia sin constructores por defecto ni service locator.
- **Capa UI (WinForms):**
  - `UI/FormReglasCategorizacion.cs` y `.Designer.cs`: CRUD de reglas de categorizacion, asignacion opcional de comercios via `lstbComerciosRegla` (CheckedListBox), grilla con proyeccion de categorias y comercios asociados.
  - `UI/FormPrincipal.cs` y `.Designer.cs`: agregado de `mnuReglasCategorizacion` bajo el menu `Finanzas`, gobernado por el permiso `gestionreglascategorizacion`.
  - `UI/FormMovimientos.cs` y `.Designer.cs`: agregado del selector `lblComercioMovimiento` y `cmbComercioMovimiento` (con opcion inicial `<Ninguno>`), columna `Comercio` en `dgvMovimientos`, boton `btnSugerirCategoriaMovimiento` y evento `txtbDescripcionMovimiento_Leave` con sugerencia automatica y mensaje de fallback ante falta de coincidencia o empate.
- **Internacionalizacion (i18n):**
  - Sincronizacion de `translation-seeds/base.es.json` y `base.in.json` mediante `sync-i18n-base-seed.ps1`.
- **Verificacion:**
  - `TpIDS.sln` compila limpiamente con MSBuild (0 errores, 0 advertencias), validando las guardas de arquitectura y CompositionRoot.
  - Bateria automatizada ejecutada con `run-tests.ps1 -IncludeIntegration`: 282/282 tests unitarios en verde, 102/102 tests de integracion en verde contra `TpCatedraIDS_Test`. Suite FlaUI pendiente de confirmacion explicita del usuario.


## 8. Sesion del 24-25/09/2026: Entidades Financieras, Medios de Pago y extension de Promocion

> Nota: las sesiones intermedias (Analiticas: Dashboard + Balance General, OCR de tickets,
> Asistente Personal, geolocalizacion de Comercio) no estan volcadas en este documento; su
> detalle vive en la memoria del proyecto y en el codigo.

Se completo la configuracion de entidades financieras y medios de pago del usuario, y se
conecto con las Promociones existentes. Caso de negocio que motiva el modelo: *"Pagando con
Galicia Credito Eminent tenes 10% de reintegro en combustibles lunes y miercoles, del dia A al
B, con tope de $5000"*.

- **Modelo de dominio:**
  - `MedioDePago` (aplanado) se partio en `MedioPago` (catalogo global: `Tipo` + `Marca`) y
    `MedioPagoEntidad` (producto real de una `EntidadFinanciera`: `MedioPago` + `Segmento`).
    El enum `TipoMedioDePago` se renombro a `TipoMedioPago`.
  - `EntidadFinanciera.MediosPagoEntidad` = catalogo que ofrece la entidad;
    `CuentaFinanciera.MediosPagoEntidad` = subconjunto contratado por el usuario (M:N,
    `CuentaFinancieraMedioPagoEntidad`).
  - `CuentaFinanciera.Tipo` pasa a enum `TipoCuentaFinanciera` (`BilleteraDigital`,
    `CuentaCorriente`, `CajaAhorro`) con `CHECK` en SQL.
  - `Promocion` suma `TopeDescuento` (nullable) y `DiasSemana` (tabla `PromocionDiaSemana`;
    0 filas = todos los dias). `PromocionMedioDePago` pasa a `PromocionMedioPagoEntidad`.
    `Promocion` sigue siendo de solo lectura.
- **Decisiones de alcance (usuario):**
  - Una `CuentaFinanciera` **no se edita**: solo alta y baja. La entidad es fija; el alta crea
    la cuenta y sus medios contratados en una sola transaccion.
  - `CredencialesCifradas` queda **fuera de alcance**: la columna existe pero no se expone en
    UI ni en la BLL (siempre NULL).
- **BLL + CompositionRoot:** `MedioPagoBLL`, `EntidadFinancieraBLL` (incluye alta/baja de los
  `MedioPagoEntidad` de la entidad) y `CuentaFinancieraBLL` (scoped a la sesion). Wireados en
  `IUnidadDeTrabajo` (`MediosPago`, `MediosPagoEntidad`, `EntidadesFinancieras`,
  `CuentasFinancieras`).
- **UI:** `FormGestionMediosPago` y `FormGestionEntidadesFinancieras` +
  `FormDetalleEntidadFinanciera` (menu Operador), `FormGestionCuentasFinancieras` (menu
  Finanzas). `FormDetalleComercio` muestra tope y dias de las promociones.
- **Permisos:** `gestionmediospago` y `gestionentidadesfinancieras` -> operador + admin;
  `gestioncuentasfinancieras` -> basico + admin. Aplicados tambien en `TpCatedraIDS`.
- **Traducciones:** 142 keys nuevas en `base.es.json`/`base.in.json`, insertadas tambien en
  `Palabra`/`Traduccion` de `TpCatedraIDS`.
- **Diagrama:** `G00-modelo-dominio.puml`/`.svg` actualizado (`EntidadFinanciera`,
  `CuentaFinanciera` y `Promocion` pasan a `<<Implementado>>`; `MedioPago` y
  `MedioPagoEntidad` reemplazan a `MedioDePago`).
- **Verificacion:** `TpIDS.sln` compila (0 errores, guardas OK).
  `run-tests.ps1 -IncludeIntegration -IncludeUi`: 357/357 unit, 121/121 integracion,
  43/43 FlaUI.
- **Tests UI:** `tests/TpIDS.UiTests/FIN_EntidadesMediosPagoTests.cs` (12 tests FlaUI, 12/12 en
  verde con `-UiFilter FIN`). La infraestructura suma login como `operador` y
  `UiFind.SeleccionarItemComboConTeclado` (necesario en combos que cargan datos dependientes:
  `SeleccionarItemCombo` no dispara `SelectedIndexChanged`).
- **Pendiente:** las promociones siguen sin pantalla de carga (las generara un proceso
  automatico a futuro).
