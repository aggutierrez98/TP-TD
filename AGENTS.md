# Documentacion del Proyecto (Agentes y Desarrolladores)

Todo el conocimiento de contexto se ubica dentro del subdirectorio `.agents/`. Leer estas referencias antes de actuar y escribir codigo:

## Base del proyecto

- [Stack .NET y WinForms](.agents/01-stack-dotnet-winforms.md): restricciones del stack, version de framework y alcance tecnico.
- [Arquitectura por Capas](.agents/02-architecture-7-layers.md): restricciones arquitectonicas del proyecto.

## Skills del repositorio

- [Architecture Dependency Guard](.agents/skills/architecture-dependency-guard/SKILL.md): obliga a respetar la matriz exacta de dependencias entre UI, BLL, REPOSITORIO, DAL, BE, SERVICIOS y ABSTRACCIONES; ademas se valida automaticamente durante el build.
- [Composition Root Guard](.agents/skills/composition-root-guard/SKILL.md): obliga a la disciplina de composicion/inyeccion dentro de esas capas: los BLL se construyen solo desde `CompositionRoot` (nada de `new XxxBLL()` fuera del root ni ctores por defecto), ninguno actua como Service Locator, y define como dar de alta un BLL o servicio en el grafo de dependencias. Se valida automaticamente durante el build. Complementa a `architecture-dependency-guard`.
- [WinForms Build Verification](.agents/skills/winforms-build-verification/SKILL.md): obliga a compilar `TpIDS.sln` con MSBuild antes de dar una tarea por terminada.
- [Strict TDD](.agents/skills/strict-tdd/SKILL.md): obliga al ciclo red-green estricto para todo cambio de comportamiento en SERVICIOS, BLL, REPOSITORIO o DAL: verificar baseline en verde, escribir primero el test (unit en `SERVICIOS.Tests`/`BLL.Tests`, integracion en `TpIDS.IntegrationTests` para REPOSITORIO/DAL), verlo fallar con `tests/tools/run-tests.ps1`, implementar y verlo pasar. Los tests UI FlaUI se escriben o actualizan despues del desarrollo, nunca dentro del ciclo.
- [Test Suite Verification](.agents/skills/test-suite-verification/SKILL.md): obliga a cerrar todo cambio funcional con la suite de tests en verde via `tests/tools/run-tests.ps1 -IncludeIntegration`, reportando contadores reales del TRX; define el protocolo de la suite UI FlaUI (avisar que toma el escritorio ~5 minutos y esperar confirmacion del usuario) y de las corridas filtradas con `-UiFilter` (solo a pedido explicito del usuario y advirtiendo el riesgo de regresiones no cubiertas). Complementa a `winforms-build-verification`, no lo reemplaza.
- [WinForms UI Control Naming](.agents/skills/winforms-ui-control-naming/SKILL.md): define la nomenclatura obligatoria para controles visuales y handlers WinForms.
- [TP Delivery Standards](.agents/skills/tp-delivery-standards/SKILL.md): aplica restricciones tecnicas, criterios de entrega, balanceo documentacion-codigo y exigencias del TP.
- [TP UML y Casos de Uso](.agents/skills/tp-uml-y-casos-de-uso/SKILL.md): punto central de diagramas UML y especificaciones de casos de uso del TP. Define que artefactos exige cada requisito (T0X/G0X/N0X), su ubicacion bajo `diagramas/<requisito>/` y su nomenclatura; orquesta las subskills de PlantUML y de especificacion de casos de uso; sincroniza con la documentacion formal y valida los artefactos. Se ejecuta a demanda, con desarrollo completo y preguntando primero al usuario. Es el punto de entrada para diagramar o especificar.
- [PlantUML Embebido en TP Delivery Standards](.agents/skills/tp-delivery-standards/embedded-skills/plantuml/SKILL.md): subskill (orquestada por `tp-uml-y-casos-de-uso`) para crear, editar, convertir o validar diagramas UML con PlantUML. Cargar solo cuando la tarea realmente requiera diagramacion PlantUML.
- [TP Use Case Spec](.agents/skills/tp-use-case-spec/SKILL.md): subskill (orquestada por `tp-uml-y-casos-de-uso`) que genera especificaciones de casos de uso con el formato estandar del TP (.docx + .md espejo) a partir de un JSON. Cargar cuando haya que documentar, crear o actualizar casos de uso.
- [DAL SQL Access Patterns](.agents/skills/dal-sql-access-patterns/SKILL.md): centraliza acceso SQL Server, `ConfigurationManager` y el uso obligatorio de transacciones en la capa DAL.
- [Translation Base Seed Sync](.agents/skills/translation-base-seed-sync/SKILL.md): sincroniza el catalogo base de traducciones de la UI con el bloque auto-generado de `script_bd.sql`.

## Skills .NET generales (dotnet/skills — cargar solo la relevante a la tarea)

Skills de la coleccion publica [dotnet/skills](https://github.com/dotnet/skills), almacenadas en el repo bajo `.agents/skills/`. Cargar unicamente la que corresponda al trabajo actual.

- [dotnet-msbuild](.agents/skills/dotnet-msbuild/SKILL.md): diagnostico de fallas de build, optimizacion de MSBuild, anti-patrones en `.csproj` y referencias entre proyectos. Usar ante errores de compilacion, builds lentos o problemas de configuracion en `TpIDS.sln`.
- [dotnet-diag](.agents/skills/dotnet-diag/SKILL.md): diagnostico de performance y debugging de aplicaciones .NET desktop (scan de anti-patrones, trazas, dumps, microbenchmarks). Usar ante lentitud, memory leaks o hangs en la UI WinForms.
- [dotnet-test](.agents/skills/dotnet-test/SKILL.md): generacion de tests, auditoria de calidad y analisis de cobertura con MSTest. Complementa `strict-tdd` y `test-suite-verification`; cargar cuando haya que generar, revisar o auditar la suite de tests.
- [WinForms Patterns](.agents/skills/winforms/SKILL.md): data binding con `BindingSource`, validacion con `ErrorProvider`, async/await en UI y thread safety. **Leer primero las restricciones del skill**: MVP esta prohibido; DI aplica solo via `CompositionRoot` manual (no `IServiceCollection`); patrones .NET 6+ no aplican.

## Skills de buenas practicas C# (cargar solo la relevante a la tarea)

Skills de referencia generica de C#. Cargar unicamente la que corresponda al trabajo actual; no leerlas todas.

- [C# Async](.agents/skills/csharp-async/SKILL.md): buenas practicas de programacion asincronica en C# (sufijo `Async`, `async/await`, manejo de `Task`). Usar al escribir o revisar codigo asincrono.
- [C# Docs](.agents/skills/csharp-docs/SKILL.md): documentacion de tipos con comentarios XML y buenas practicas de documentacion. Usar al documentar APIs publicas/internas.
- [C# XUnit](.agents/skills/csharp-xunit/SKILL.md): buenas practicas de testing unitario con XUnit, incluyendo tests data-driven.
- [C# NUnit](.agents/skills/csharp-nunit/SKILL.md): buenas practicas de testing unitario con NUnit, incluyendo tests data-driven.
- [C# MSTest](.agents/skills/csharp-mstest/SKILL.md): buenas practicas de testing unitario con MSTest 3.x/4.x y APIs de assertion modernas.
- [C# TUnit](.agents/skills/csharp-tunit/SKILL.md): buenas practicas de testing unitario con TUnit, incluyendo tests data-driven.

> Para tests, cargar solo la skill del framework efectivamente usado en el proyecto; las demas de testing son alternativas mutuamente excluyentes.

## Skills externas instaladas y recomendadas

Estas skills viven fuera del repo (no se clonan con el). Instalarlas bajo el home del usuario (`~/.codex/skills/`); las rutas se expresan relativas al home para que sean validas en cualquier maquina.

> Actualmente no hay skills externas instaladas.

## Reglas transversales

- Todo cambio funcional, bugfix, refactor o cambio de formularios debe respetar el stack documentado y la arquitectura por capas.
- Toda modificacion de referencias entre proyectos o movimiento de clases entre capas debe respetar estrictamente la matriz `UI -> ABSTRACCIONES, BLL, BE, SERVICIOS`, `BLL -> ABSTRACCIONES, REPOSITORIO, BE, SERVICIOS`, `REPOSITORIO -> ABSTRACCIONES, DAL, BE, SERVICIOS`, `DAL -> ABSTRACCIONES, BE, SERVICIOS`, `BE -> ABSTRACCIONES`, `SERVICIOS -> ABSTRACCIONES`, `ABSTRACCIONES -> sin dependencias`. Todas las capas pueden depender de `ABSTRACCIONES` (nucleo de contratos) para consumir las utilidades por interfaz (DIP); `ABSTRACCIONES` no depende de nada y solo contiene contratos estables.
- Todo cambio sobre la solucion debe cerrar con compilacion exitosa de `TpIDS.sln`, incluyendo la validacion automatica de arquitectura (matriz de capas) y del Composition Root.
- Todo cambio de comportamiento en `SERVICIOS`, `BLL`, `REPOSITORIO` o `DAL` debe desarrollarse con TDD estricto segun la skill `strict-tdd`: baseline en verde, primero el test en rojo (fallo observado, no supuesto), despues la implementacion en verde. Los tests UI FlaUI se actualizan despues del desarrollo y no sustituyen el ciclo unitario/integracion.
- Todo cambio funcional, bugfix, refactor, cambio de tests o cambio del harness de tests debe cerrar con `tests/tools/run-tests.ps1 -IncludeIntegration` en verde (exit code 0) segun la skill `test-suite-verification`, ademas de la compilacion de `TpIDS.sln`. La suite UI completa solo se corre tras avisar al usuario que toma el escritorio ~5 minutos y recibir su confirmacion explicita; `-UiFilter` solo se usa a pedido del usuario, advirtiendo que una corrida filtrada no cubre regresiones en los flujos no ejecutados. Si SQL Server no esta disponible, correr al menos los unit tests e informar la limitacion; nunca reportar exito no verificado.
- Todo alta o cambio de un BLL o servicio transversal debe pasar por `CompositionRoot` segun la skill `composition-root-guard`: construir el BLL solo desde el root (sin `new XxxBLL()` fuera de el ni constructores por defecto), inyectar dependencias por el ctor `internal`, y no usar `CompositionRoot` como Service Locator dentro de los BLL.
- Todo agregado o renombre de controles visuales WinForms debe respetar la nomenclatura documentada en las skills del repositorio.
- Todo cambio sobre textos traducibles, tags `Form...`, encabezados de grillas o nuevas keys de idioma en la UI debe ejecutar la skill `translation-base-seed-sync` y dejar actualizado `translation-seeds/base.es.json` en el mismo cambio. `script_bd.sql` solo debe cambiar cuando cambie el bloque generador o la ruta resuelta al archivo externo.
- Todo diagrama UML nuevo o modificado debe realizarse en PlantUML y guardarse como codigo fuente dentro del repo antes de reflejarse en la documentacion final.
