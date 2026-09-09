# TODO: actualizar diagramas UML desactualizados (arquitectura vieja)

Nota de traspaso para la proxima sesion. Generada el 2026-08-26, actualizada el 2026-08-27
tras completar la revision carpeta por carpeta.

## Contexto

Los `.puml` bajo `diagramas/` (y las imagenes ya pegadas en
`/mnt/c/Users/agust/Desktop/Trabajo Final TD Gutierrez.docx`) se hicieron antes o durante la
migracion a la arquitectura de 7 capas (capa `REPOSITORIO` introducida en el commit `23db1f8`,
2026-06-23) y algunos quedaron desactualizados frente a refactors posteriores del codigo: alta de
`CompositionRoot`, guards de arquitectura, renombres (`BootstrapAplicacionBLL` ->
`InicializadorAplicacionBLL`) y cambios en `PermisoBLL`/`UsuarioBLL`/`IntegridadBLL`.

## Trabajo realizado (26-27/08/2026): revision carpeta por carpeta

Se recorrio, segun el plan original, cada carpeta bajo `diagramas/` en el orden T01, T02, T03,
T04, T05, T06a, T06b, T07, G06, G07: se leyo el codigo real de las clases involucradas y se
comparo contra cada `.puml`. Resultado por carpeta:

- **T01-arquitectura-base**: sin cambios. Componentes y navegacion ya reflejaban `CompositionRoot`,
  `InicializadorAplicacionBLL` y los 7 Forms reales.
- **T02-login-logout**: corregido. `FormLogin` tenia un campo `gestorIdioma : IdiomaBLL` que ya no
  existe en el codigo (se elimino); `IUsuarioEstadoBLL.GuardarVersionActualInterno` estaba
  documentado como `void` y devuelve `int`; la secuencia de CU02 Registro tenia el orden invertido
  respecto al codigo real (el codigo inserta primero y **recien despues** calcula y persiste el
  DVH — el comentario del propio codigo explica por que). Se corrigieron los tres puntos, se
  actualizo el JSON y se regenero el `.docx`/`.md` del requisito.
- **T03-encriptado**: faltaba por completo `T03-encriptado-secuencia.puml` (secuencia general),
  exigida por el catalogo ya que T03 no tiene CU propios. Se creo desde cero, verificada contra
  `PasswordHasher.cs` (PBKDF2, salt 16 bytes, hash 32 bytes, 20000 iteraciones).
- **T04-perfiles-usuario**: sin cambios. El diagrama Composite, el DER y las 3 secuencias de CU ya
  balanceaban exacto contra `PermisoBLL`, `PermisoService`, `PermisoSimple/PermisoGrupo`,
  `AutorizacionService` y los repositorios reales.
- **T05-multidioma**: corregido. El diagrama de clases (Observer) tenia un metodo publico
  `AplicarIdiomaSesion(idiomaId : int)` en `IdiomaBLL` que en realidad es **privado** y recibe un
  `Idioma` completo, no un `int` (se elimino del diagrama). La secuencia de CU07 (Configurar idioma
  preferido) tenia el mismo error de tipo en la llamada interna y ademas inventaba una segunda
  llamada a `ObtenerIdiomaCompleto` que el codigo no hace (el metodo privado reutiliza el objeto
  `idioma` ya obtenido). Se corrigieron ambos archivos. Ademas (detectado a pedido explicito del
  usuario, que pregunto si todos los diagramas ya tenian las capas actuales): el diagrama de
  clases mostraba REPOSITORIO pero cortaba ahi, sin bajar a DAL — a diferencia de T02/T04/T06a/
  T06b/T07, que si muestran las clases DAL concretas. Se agregaron `IdiomaDAL`,
  `UsuarioPreferenciaDAL` y `AccesoDatosSqlServer`, verificados contra `IdiomaRepositorio.cs`/
  `TraduccionRepositorio.cs`/`UsuarioPreferenciaRepositorio.cs` reales (se confirmo que
  `TraduccionRepositorio` no tiene DAL propio: delega en `IdiomaRepositorio`/`IdiomaDAL`, igual
  que ya reflejaba G06 sin un `TraduccionDAL`).
- **T06a-bitacora**: sin cambios. Clases, DER y las 2 secuencias de CU balanceaban exacto.
- **T06b-control-cambios**: corregido, el mas afectado despues de T07.
  - Habia **dos** archivos de secuencia para el mismo CU04 (`-restaurar-punto-` y
    `-restaurar-version-`). El segundo era una reliquia de la arquitectura vieja (llamadas
    directas a `AccesoDatos`/DAL, sin `REPOSITORIO` ni `IUnidadDeTrabajo`, clases inexistentes
    como `BitacoraService`). Se elimino ese `.puml` y su `.svg` (`git rm`); el que queda
    (`-restaurar-punto-`) ya balanceaba exacto contra `UsuarioBLL.RestaurarVersion`.
  - El JSON nombraba a CU04 "Restaurar version de usuario", pero el catalogo
    (`catalogo-requisitos.md`) lo llama "Restaurar punto del historial de usuario". Se corrigio el
    `id_nombre` y se regenero el `.docx`/`.md`.
  - La secuencia general (`T06b-control-cambios-secuencia.puml`) tambien era de la arquitectura
    vieja (DAL directo). Se reescribio completa contra el codigo real de
    `UsuarioEstadoBLL.ConstruirSnapshotActualInterno`/`GuardarVersionActualInterno` y
    `HistorialEstadosService`.
- **T07-digitos-verificadores**: corregido. El diagrama de clases y el DER ya estaban al dia (y el
  CU01 tambien), pero la secuencia general (`T07-digitos-verificadores-secuencia.puml`) era
  **la prueba directa de staleness** que motivo este TODO: referenciaba literalmente
  `BootstrapAplicacionBLL` (ya `InicializadorAplicacionBLL`) y describia todo el flujo con DAL
  directo (`UsuarioDAL`, `IntegridadDAL`, `AccesoDatos`), sin `REPOSITORIO`/`IUnidadDeTrabajo`. Se
  reescribio completa contra `IntegridadBLL.cs` real (`ValidarIntegridadGlobal`,
  `ActualizarDVVInterno`, `RecalcularDVVInterno`), remitiendo al CU01 para el detalle del arranque
  y enfocandose en el camino incremental de escritura que el CU01 no cubre.
- **G06-clases-por-capa**: sin cambios. Las 7 capas (abstracciones, be, bll, dal, repositorio,
  servicios, ui) ya balanceaban exacto contra el codigo real, incluidas clases internas
  (`UnidadDeTrabajoSqlServer`, `PasswordHashService`) y herencias (`FormContingenciaIntegridad`
  hereda de `FormTraducibleBase`).
- **G07-modelo-datos-integrado**: sin cambios. Las 12 tablas del DER integrado coinciden exacto
  con `script_bd.sql` (columnas, tipos, PK/FK, nullabilidad).

Se regeneraron las imagenes SVG de todos los `.puml` nuevos o modificados con
`plantuml -tsvg <archivo>` (toolchain disponible en esta sesion via `~/.local/bin`), y se
regenero el documento consolidado `diagramas/TP-especificaciones-casos-de-uso.docx`/`.md`
(6 requisitos, 18 CU) con la subskill `tp-use-case-spec`.

## Pendiente para la proxima sesion

1. **Reemplazar imagenes en el Word de catedra**: el documento formal
   (`/mnt/c/Users/agust/Desktop/Trabajo Final TD Gutierrez.docx`, fuera del repo, en Windows)
   todavia tiene las imagenes viejas de T02, T03 (nunca tuvo secuencia general), T05, T06b y T07
   pegadas de una sesion anterior. Hay que reemplazarlas por los SVG regenerados en esta sesion
   (mismo mecanismo `python-docx` que se uso para G00-G05, avisando antes al usuario que cierre el
   Word en Windows). No se toco en esta sesion porque no se confirmo el archivo abierto/cerrado.
2. **DER no verificados contra la base real en SQL Server**: esta sesion tampoco tuvo `sqlcmd` ni
   una instancia SQL Server accesible. La verificacion se hizo cruzando cada DER contra
   `script_bd.sql` (que deberia ser fiel a la base real, pero no es la misma garantia que
   inspeccionar la base en vivo per la regla de balanceo de la skill). Repetir la verificacion
   `der-desde-mssql.md` cuando haya una sesion con la base accesible.
3. **Secuencia general ausente en T02, T04, T05 y T06 (bitacora)**: el catalogo exige "clases, DER,
   secuencia general" como generales de todo modulo con CU, pero estos 4 requisitos solo tienen
   diagramas de secuencia por CU (sin un `-secuencia.puml` general). A diferencia de T03 (que no
   tiene CU y por eso la secuencia general era obligatoria y se creo esta sesion), en estos 4 casos
   los CU ya cubren el 100% del comportamiento del modulo, asi que no se genero contenido
   especulativo sin confirmar primero con el usuario si vale la pena el artefacto extra o si se
   considera cubierto por las secuencias de CU. **Preguntar al usuario antes de crear estos 4
   archivos.**
4. **Compilacion no verificada**: esta sesion no tuvo `dotnet`/`msbuild` disponibles (WSL sin
   tooling .NET). No se toco ningun archivo de codigo (`.cs`/`.csproj`), solo diagramas y
   documentacion, asi que el riesgo de rotura es minimo, pero falta correr
   `winforms-build-verification` en una sesion con el SDK de .NET disponible antes de dar el
   trabajo por definitivamente cerrado.

## Fuera de alcance de esta tarea

- No se toco `diagramas/G00-descripcion-global-producto/G00-descripcion-global-producto.md` ni
  la seccion G00-G05 ya volcada en el Word: quedaron cerradas y aprobadas por el usuario en la
  sesion del 26/08/2026.
- No se avanzo con N0X (proceso de negocio), A0X (instalador/PDF/serializacion) ni D0X
  (manuales): decision explicita del usuario, pendiente para una etapa posterior.

## Nota (29/08/2026): reconciliacion de numeracion contra la Carpeta Modelo 2023

En esta sesion se renombraron las carpetas/archivos reales para alinear la numeracion del repo
con la "Carpeta modelo IS y TD 2023" vigente (documentada en la skill `tp-carpeta-modelo`):
`G06-clases-por-capa` -> `G07-clases-por-capa`, `G07-modelo-datos-integrado` ->
`G08-modelo-datos-integrado`, y se fusiono `T06a-bitacora` + `T06b-control-cambios` en una unica
carpeta `T06-bitacora-control-cambios` (4 CU: CU01-CU02 bitacora, CU03-CU04 control de cambios).
`T08-backup-restore` se dejo sin tocar (extension no oficial del equipo, ya documentada). El Word
externo de catedra (`Trabajo Final TD Gutierrez.docx`, punto 1 de "Pendiente" arriba) todavia no
se sincronizo con estos renombres, asi que va a quedar aun mas desactualizado hasta la proxima
sesion de sincronizacion.
