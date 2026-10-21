# Documentacion del Proyecto (Agentes y Desarrolladores)

**Antes que nada**, el contexto permanente del proyecto (que es FINAZAPP, la Carpeta Modelo del TP, alcance, numeracion T0X, donde vive cada artefacto) esta en el campo `context:` de [openspec/config.yaml](openspec/config.yaml). Para saber que hay en curso: `openspec list` y el `tasks.md` de cada cambio (ver "Flujo de trabajo" abajo). No existe un archivo de estado o de proximos pasos aparte, y no se debe crear.

Todo el conocimiento de contexto adicional se ubica dentro del subdirectorio `.agents/`. Leer estas referencias antes de actuar y escribir codigo:

## Base del proyecto

- [Stack .NET y WinForms](.agents/01-stack-dotnet-winforms.md): restricciones del stack, version de framework y alcance tecnico.
- [Arquitectura por Capas](.agents/02-architecture-7-layers.md): restricciones arquitectonicas del proyecto.

## Restriccion inviolable de laboratorio: sin NuGet en produccion

- **La solucion productiva `TpIDS.sln` MUST poder compilarse directamente desde el codigo fuente y ejecutarse en la maquina del laboratorio de la facultad con .NET Framework 4.7.2, sin instalar ni restaurar ningun paquete NuGet y sin depender de un cache NuGet previo.** Esta restriccion es inviolable; no alcanza con entregar un ejecutable ya compilado que funcione alli.
- No agregar `PackageReference`, `packages.config`, pasos de restore ni referencias a rutas del cache NuGet en proyectos productivos. Una biblioteca externa, incluido `System.Text.Json`, debe tener una referencia compatible y reproducible que cumpla esta restriccion; un `using` no acredita su disponibilidad. Incluir DLL externas en el repositorio requiere que el usuario confirme que esa forma de distribucion esta permitida; el usuario la confirmo para `lib/` (ver `lib/README.md`), donde viven las DLL de UI y de servicios, referenciadas con `HintPath` desde los `.props`.
- **Excepcion explicita:** los proyectos de tests y `tests/TpIDS.Tests.sln` pueden usar NuGet porque se compilan y ejecutan solo en la maquina de desarrollo del usuario. No incorporar proyectos de tests ni sus dependencias a `TpIDS.sln` o a los proyectos productivos.
- La verificacion de portabilidad exige compilar la solucion productiva en un entorno limpio equivalente al laboratorio, sin restauracion ni cache NuGet. Una compilacion que aproveche paquetes del desarrollador no demuestra cumplimiento. Si se detecta una dependencia que viola esta regla, registrar su resolucion en el cambio OpenSpec correspondiente e informar la limitacion antes de afirmar portabilidad.

## Flujo de trabajo: plan antes del desarrollo (OpenSpec)

Todo cambio no trivial se planifica **antes** de escribir codigo con [OpenSpec](https://github.com/Fission-AI/OpenSpec). OpenSpec es solo tooling de desarrollo (Markdown y YAML en `openspec/`): no interviene en la compilacion ni hace falta para compilar o ejecutar.

### Donde va cada cosa

| Que | Donde | Nunca en |
|-----|-------|----------|
| Contexto permanente del proyecto (producto, alcance, convenciones del TP) | `context:` de `openspec/config.yaml`. El CLI lo inyecta en cada pedido de planificacion (`openspec instructions ...`) | Un Markdown suelto en la raiz |
| Comportamiento aceptado del sistema (lo que la app hace hoy) | `openspec/specs/<capacidad>/spec.md`. Solo se actualiza al archivar un cambio | Un spec con estado o avance: si un requisito esta en `specs/`, esta implementado |
| Un cambio en curso: por que, que, como | `openspec/changes/<cambio>/{proposal,design}.md` y `specs/<capacidad>/spec.md` (deltas `## ADDED/MODIFIED/REMOVED Requirements`) | `AGENTS.md`, `.agents/` o archivos de notas |
| Avance de un cambio | Los checkboxes de `openspec/changes/<cambio>/tasks.md`, unica fuente de verdad del progreso | Un archivo de estado, resumen de sesion o seccion de "pendientes" |
| Pendiente que un cambio deja en otro artefacto (por ejemplo, diagramas G07/G08 o el Word de la catedra) | Una tarea en el `tasks.md` del propio cambio; si no se hace ahi, una tarea en el change abierto que corresponda (los diagramas, en `actualizar-diagramas-tp` mientras siga activo) o un change nuevo | Un archivo `TODO-*.md` o una nota de "pendiente para la proxima sesion" |
| Cambios terminados | `openspec/changes/archive/AAAA-MM-DD-<cambio>/`, movidos por `openspec archive` | Borrados a mano |
| Reglas para escribir artefactos (largo, formato) | `rules:` de `openspec/config.yaml`, por artefacto | El `context:` |

### Ciclo de un cambio

1. **Explorar** (opcional): `openspec-explore` para pensar opciones sin escribir nada.
2. **Proponer**: `openspec-propose` crea `openspec/changes/<cambio>/` con `openspec new change <cambio>` y genera proposal, specs, design y tasks segun `openspec instructions <artefacto> --change <cambio>`. Cerrar con `openspec validate <cambio> --strict`. **No implementar sin aprobacion del usuario.**
3. **Implementar**: `openspec-apply-change` toma las tareas de `tasks.md` en orden y marca `- [x]` cada una al terminarla y verificarla. Si el plan cambia, `openspec-update-change` actualiza los artefactos; la tarea se edita o se quita del `tasks.md`, no se deja una nota en otro lado.
4. **Archivar**: con todas las tareas en `[x]`, `openspec-archive-change` (o `openspec archive <cambio> -y`) mueve el cambio a `archive/` y aplica sus deltas a `openspec/specs/`. Una capacidad nueva necesita su seccion `## Purpose` en el delta.

### Retomar en otra conversacion

No hace falta pegar resumenes de sesiones anteriores: `openspec list` muestra los cambios activos con su avance y `openspec status --change <cambio>` los artefactos. Leer `proposal.md` (el que y por que), `design.md` (el como) y `tasks.md` (que esta hecho y cual es la siguiente tarea), y seguir con `openspec-apply-change`. Si algo aprendido en la sesion tiene que sobrevivir, va al artefacto que corresponde (decision tecnica en `design.md`, tarea nueva en `tasks.md`, convencion permanente en `config.yaml` o en `.agents/`).

### Herramientas

- **Instalacion** (una vez por maquina de desarrollo, requiere Node 20.19+): `npm install -g @fission-ai/openspec@latest`. Ya inicializado en este repo.
- **Skills**: viven en `.agents/skills/openspec-*/` (generadas con `openspec init --tools agents`; **no hay slash commands `/opsx:*`**). Leer su `SKILL.md` antes de cada paso: `openspec-explore`, `openspec-propose`, `openspec-apply-change`, `openspec-update-change`, `openspec-sync-specs` y `openspec-archive-change`. No regenerarlas con `--tools claude` (crearia `.claude/skills` y `.claude/commands`, fuera de la estructura del proyecto).
- **CLI**: `openspec list` (cambios activos), `openspec list --specs` (capacidades vigentes), `openspec status --change <cambio>`, `openspec validate <cambio> --strict`, `openspec archive <cambio>`.
- **Idioma**: los artefactos van en espanol; los encabezados estructurales y SHALL/MUST, en ingles.

Las reglas de este archivo (capas, TDD, build, tests, composition root) siguen aplicando durante `openspec-apply-change`.

## Auto-invocar skills

Al realizar estas acciones, invocar SIEMPRE la skill correspondiente ANTES de empezar. No es opcional ni depende de que la tarea parezca chica.

| Accion | Skill |
|--------|-------|
| Cerrar cualquier cambio de codigo, formularios, recursos o `.csproj` | `winforms-build-verification` |
| Cambiar comportamiento en `SERVICIOS`, `BLL`, `REPOSITORIO` o `DAL` | `strict-tdd` |
| Cerrar un cambio funcional, o tocar tests o el harness de tests | `test-suite-verification` |
| Mover clases entre capas o tocar referencias entre proyectos | `architecture-dependency-guard` |
| Dar de alta o modificar un BLL o servicio transversal | `composition-root-guard` |
| Agregar o renombrar controles visuales y handlers WinForms | `winforms-ui-control-naming` |
| Tocar textos traducibles, tags `Form...`, encabezados de grilla o keys de idioma | `translation-base-seed-sync` |
| Escribir o modificar acceso SQL Server en la capa DAL | `dal-sql-access-patterns` |
| Crear o modificar diagramas UML o especificaciones de casos de uso | `tp-uml-y-casos-de-uso` |
| Trabajar data binding, `ErrorProvider`, async o thread safety en formularios | `winforms` |
| Diagnosticar errores de compilacion o builds lentos | `dotnet-msbuild` |
| Diagnosticar lentitud, memory leaks o hangs en la UI | `dotnet-diag` |

## Skills del repositorio

Viven en el repo bajo `.agents/skills/`. Las validadas en build fallan la compilacion de `TpIDS.sln` si se violan.

| Skill | Que resuelve | Cuando cargarla | Ruta |
|-------|--------------|-----------------|------|
| `architecture-dependency-guard` | Matriz exacta de dependencias entre UI, BLL, REPOSITORIO, DAL, BE, SERVICIOS y ABSTRACCIONES. Validada en build | Al mover clases entre capas o tocar referencias entre proyectos | [SKILL.md](.agents/skills/architecture-dependency-guard/SKILL.md) |
| `composition-root-guard` | Disciplina de composicion/inyeccion: los BLL se construyen solo desde `CompositionRoot`, sin `new XxxBLL()` fuera del root ni ctores por defecto, y ninguno actua como Service Locator. Validada en build. Complementa a `architecture-dependency-guard` | Al dar de alta o modificar un BLL o servicio transversal | [SKILL.md](.agents/skills/composition-root-guard/SKILL.md) |
| `winforms-build-verification` | Obliga a compilar `TpIDS.sln` con MSBuild antes de dar una tarea por terminada | Al cerrar cualquier cambio de codigo | [SKILL.md](.agents/skills/winforms-build-verification/SKILL.md) |
| `strict-tdd` | Ciclo red-green estricto: baseline en verde, test primero (unit en `SERVICIOS.Tests`/`BLL.Tests`, integracion en `TpIDS.IntegrationTests` para REPOSITORIO/DAL), verlo fallar, implementar, verlo pasar. Los tests UI FlaUI van despues del desarrollo, nunca dentro del ciclo | Ante todo cambio de comportamiento en SERVICIOS, BLL, REPOSITORIO o DAL | [SKILL.md](.agents/skills/strict-tdd/SKILL.md) |
| `test-suite-verification` | Cierre con la suite en verde via `tests/tools/run-tests.ps1 -IncludeIntegration`, reportando contadores reales del TRX. Define el protocolo de la suite UI FlaUI (avisar que toma el escritorio ~5 minutos y esperar confirmacion) y de las corridas con `-UiFilter`. Complementa a `winforms-build-verification`, no lo reemplaza | Al cerrar un cambio funcional o tocar el harness de tests | [SKILL.md](.agents/skills/test-suite-verification/SKILL.md) |
| `winforms-ui-control-naming` | Nomenclatura obligatoria de controles visuales y handlers WinForms | Al agregar o renombrar controles | [SKILL.md](.agents/skills/winforms-ui-control-naming/SKILL.md) |
| `tp-carpeta-modelo` | Fuente oficial vigente (2023) de la Carpeta Modelo de la catedra: formato de entrega, estructura completa G0X/N0X/T0X/A0X/D0X y plantilla oficial de especificacion de CU. Referencia de solo lectura, consultada por las demas skills del TP | Ante duda de que exige textualmente la catedra (numeracion de un grupo, alcance de un requisito, formato de entrega) | [SKILL.md](.agents/skills/tp-carpeta-modelo/SKILL.md) |
| `tp-delivery-standards` | Restricciones tecnicas, criterios de entrega, balanceo documentacion-codigo y exigencias del TP | Al preparar o revisar material de entrega | [SKILL.md](.agents/skills/tp-delivery-standards/SKILL.md) |
| `tp-uml-y-casos-de-uso` | Punto de entrada para diagramar o especificar: que artefactos exige cada requisito (T0X/G0X/N0X), su ubicacion bajo `diagramas/<requisito>/` y su nomenclatura. Orquesta las subskills de PlantUML y de casos de uso, sincroniza con la documentacion formal y valida los artefactos. Se ejecuta a demanda y preguntando primero al usuario | Al crear o modificar diagramas UML o especificaciones de casos de uso | [SKILL.md](.agents/skills/tp-uml-y-casos-de-uso/SKILL.md) |
| `plantuml` (subskill) | Crear, editar, convertir o validar diagramas UML con PlantUML. Orquestada por `tp-uml-y-casos-de-uso` | Solo cuando la tarea realmente requiera diagramacion PlantUML | [SKILL.md](.agents/skills/tp-delivery-standards/embedded-skills/plantuml/SKILL.md) |
| `tp-use-case-spec` (subskill) | Genera especificaciones de casos de uso con el formato estandar del TP (`.docx` + `.md` espejo) a partir de un JSON. Orquestada por `tp-uml-y-casos-de-uso` | Al documentar, crear o actualizar casos de uso | [SKILL.md](.agents/skills/tp-use-case-spec/SKILL.md) |
| `openspec-*` (6 skills) | Flujo de planificacion con OpenSpec: explorar, proponer, implementar, actualizar, sincronizar y archivar un cambio en `openspec/changes/`. Generadas por el CLI; no editarlas a mano | Antes de planificar o implementar cualquier cambio no trivial (ver "Flujo de trabajo") | [openspec-propose](.agents/skills/openspec-propose/SKILL.md), [openspec-apply-change](.agents/skills/openspec-apply-change/SKILL.md) |
| `dal-sql-access-patterns` | Acceso SQL Server, `ConfigurationManager` y uso obligatorio de transacciones en la capa DAL | Al escribir o modificar acceso a datos | [SKILL.md](.agents/skills/dal-sql-access-patterns/SKILL.md) |
| `translation-base-seed-sync` | Sincroniza el catalogo base de traducciones de la UI con el bloque auto-generado de `script_bd.sql` | Al tocar textos traducibles, tags `Form...`, encabezados de grilla o keys de idioma | [SKILL.md](.agents/skills/translation-base-seed-sync/SKILL.md) |

## Skills .NET generales

Skills de la coleccion publica [dotnet/skills](https://github.com/dotnet/skills), almacenadas en el repo bajo `.agents/skills/`. Cargar unicamente la que corresponda al trabajo actual.

| Skill | Que resuelve | Cuando cargarla | Ruta |
|-------|--------------|-----------------|------|
| `dotnet-msbuild` | Diagnostico de fallas de build, optimizacion de MSBuild, anti-patrones en `.csproj` y referencias entre proyectos | Ante errores de compilacion, builds lentos o problemas de configuracion en `TpIDS.sln` | [SKILL.md](.agents/skills/dotnet-msbuild/SKILL.md) |
| `dotnet-diag` | Performance y debugging de apps .NET desktop: scan de anti-patrones, trazas, dumps, microbenchmarks | Ante lentitud, memory leaks o hangs en la UI WinForms | [SKILL.md](.agents/skills/dotnet-diag/SKILL.md) |
| `dotnet-test` | Generacion de tests, auditoria de calidad y analisis de cobertura con MSTest. Complementa `strict-tdd` y `test-suite-verification` | Al generar, revisar o auditar la suite de tests | [SKILL.md](.agents/skills/dotnet-test/SKILL.md) |
| `winforms` | Data binding con `BindingSource`, validacion con `ErrorProvider`, async/await en UI y thread safety. **Leer primero sus restricciones**: MVP prohibido; DI solo via `CompositionRoot` manual (no `IServiceCollection`); patrones .NET 6+ no aplican | Al trabajar con controles, binding o validacion de formularios | [SKILL.md](.agents/skills/winforms/SKILL.md) |

## Skills de buenas practicas C#

Referencia generica de C#. Cargar unicamente la que corresponda al trabajo actual; no leerlas todas.

| Skill | Que resuelve | Cuando cargarla | Ruta |
|-------|--------------|-----------------|------|
| `csharp-async` | Sufijo `Async`, `async/await`, manejo de `Task` | Al escribir o revisar codigo asincrono | [SKILL.md](.agents/skills/csharp-async/SKILL.md) |
| `csharp-docs` | Documentacion de tipos con comentarios XML y buenas practicas de documentacion | Al documentar APIs publicas/internas | [SKILL.md](.agents/skills/csharp-docs/SKILL.md) |
| `csharp-mstest` | Testing unitario con MSTest 3.x/4.x y APIs de assertion modernas | **Es el framework del proyecto**: usar esta y no las otras tres | [SKILL.md](.agents/skills/csharp-mstest/SKILL.md) |
| `csharp-xunit` | Testing unitario con XUnit, incluyendo tests data-driven | Solo si el proyecto migrara a XUnit | [SKILL.md](.agents/skills/csharp-xunit/SKILL.md) |
| `csharp-nunit` | Testing unitario con NUnit, incluyendo tests data-driven | Solo si el proyecto migrara a NUnit | [SKILL.md](.agents/skills/csharp-nunit/SKILL.md) |
| `csharp-tunit` | Testing unitario con TUnit, incluyendo tests data-driven | Solo si el proyecto migrara a TUnit | [SKILL.md](.agents/skills/csharp-tunit/SKILL.md) |

> Las cuatro skills de testing son alternativas mutuamente excluyentes. El proyecto usa MSTest.

## Skills externas instaladas y recomendadas

Estas skills viven fuera del repo (no se clonan con el). Instalarlas bajo el home del usuario (`~/.codex/skills/`); las rutas se expresan relativas al home para que sean validas en cualquier maquina.

> Actualmente no hay skills externas instaladas.

## Reglas transversales

- Todo cambio funcional, bugfix, refactor o cambio de formularios debe respetar el stack documentado y la arquitectura por capas.
- Toda modificacion de referencias entre proyectos o movimiento de clases entre capas debe respetar estrictamente la matriz `UI -> ABSTRACCIONES, BLL, BE, SERVICIOS`, `CRONS -> ABSTRACCIONES, BLL, BE, SERVICIOS`, `BLL -> ABSTRACCIONES, REPOSITORIO, BE, SERVICIOS`, `REPOSITORIO -> ABSTRACCIONES, DAL, BE, SERVICIOS`, `DAL -> ABSTRACCIONES, BE, SERVICIOS`, `BE -> ABSTRACCIONES`, `SERVICIOS -> ABSTRACCIONES`, `ABSTRACCIONES -> sin dependencias`. Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos) para consumir las utilidades por interfaz (DIP); `ABSTRACCIONES` no depende de nada y solo contiene interfaces y enums (contratos sin comportamiento).
- Las clases de todas las capas se comunican pasando entidades, no IDs: firmas como `Actualizar(Categoria categoria)`, altas que devuelven la entidad, sin entidades cascaron `new X { Id = ... }` en BLL ni FKs `int xxxId` en BE. Unica excepcion: `ObtenerPorId(int id)` en REPOSITORIO y DAL. Solo SERVICIOS y DAL leen `ConfigurationManager`. Ver "Contratos entre clases" en [Arquitectura por Capas](.agents/02-architecture-7-layers.md); lo valida `verify-contratos-capas.ps1` en el build.
- Todo cambio sobre la solucion debe cerrar con compilacion exitosa de `TpIDS.sln`, incluyendo la validacion automatica de arquitectura (matriz de capas), del Composition Root y de los contratos entre capas.
- Todo cambio de comportamiento en `SERVICIOS`, `BLL`, `REPOSITORIO` o `DAL` debe desarrollarse con TDD estricto segun la skill `strict-tdd`: baseline en verde, primero el test en rojo (fallo observado, no supuesto), despues la implementacion en verde. Los tests UI FlaUI se actualizan despues del desarrollo y no sustituyen el ciclo unitario/integracion.
- Todo cambio funcional, bugfix, refactor, cambio de tests o cambio del harness de tests debe cerrar con `tests/tools/run-tests.ps1 -IncludeIntegration` en verde (exit code 0) segun la skill `test-suite-verification`, ademas de la compilacion de `TpIDS.sln`. La suite UI completa solo se corre tras avisar al usuario que toma el escritorio ~5 minutos y recibir su confirmacion explicita; `-UiFilter` solo se usa a pedido del usuario, advirtiendo que una corrida filtrada no cubre regresiones en los flujos no ejecutados. Si SQL Server no esta disponible, correr al menos los unit tests e informar la limitacion; nunca reportar exito no verificado.
- Todo alta o cambio de un BLL o servicio transversal debe pasar por `CompositionRoot` segun la skill `composition-root-guard`: construir el BLL solo desde el root (sin `new XxxBLL()` fuera de el ni constructores por defecto), inyectar dependencias por el ctor `internal`, y no usar `CompositionRoot` como Service Locator dentro de los BLL.
- Todo agregado o renombre de controles visuales WinForms debe respetar la nomenclatura documentada en las skills del repositorio.
- Todo formulario o control nuevo debe usar el tema Krypton descrito en [Stack .NET y WinForms](.agents/01-stack-dotnet-winforms.md) (punto 4): controles Krypton, paleta de `UI/Tema/` y nativos solo en las excepciones documentadas.
- Todo cambio sobre textos traducibles, tags `Form...`, encabezados de grillas o nuevas keys de idioma en la UI debe ejecutar la skill `translation-base-seed-sync` y dejar actualizado `translation-seeds/base.es.json` en el mismo cambio. `script_bd.sql` solo debe cambiar cuando cambie el bloque generador o la ruta resuelta al archivo externo.
- Todo diagrama UML nuevo o modificado debe realizarse en PlantUML y guardarse como codigo fuente dentro del repo antes de reflejarse en la documentacion final.
