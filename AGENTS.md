# Documentacion del Proyecto (Agentes y Desarrolladores)

Todo el conocimiento de contexto se ubica dentro del subdirectorio `.agents/`. Leer estas referencias antes de actuar y escribir codigo:

## Base del proyecto

- [Stack .NET y WinForms](.agents/01-stack-dotnet-winforms.md): restricciones del stack, version de framework y alcance tecnico.
- [Arquitectura por Capas](.agents/02-architecture-7-layers.md): restricciones arquitectonicas del proyecto.

## Skills del repositorio

- [Architecture Dependency Guard](.agents/skills/architecture-dependency-guard/SKILL.md): obliga a respetar la matriz exacta de dependencias entre UI, BLL, REPOSITORIO, DAL, BE, SERVICIOS y ABSTRACCIONES; ademas se valida automaticamente durante el build.
- [WinForms Build Verification](.agents/skills/winforms-build-verification/SKILL.md): obliga a compilar `TpIDS.sln` con MSBuild antes de dar una tarea por terminada.
- [WinForms UI Control Naming](.agents/skills/winforms-ui-control-naming/SKILL.md): define la nomenclatura obligatoria para controles visuales y handlers WinForms.
- [TP Delivery Standards](.agents/skills/tp-delivery-standards/SKILL.md): aplica restricciones tecnicas, criterios de entrega, balanceo documentacion-codigo y exigencias del TP.
- [TP UML y Casos de Uso](.agents/skills/tp-uml-y-casos-de-uso/SKILL.md): punto central de diagramas UML y especificaciones de casos de uso del TP. Define que artefactos exige cada requisito (T0X/G0X/N0X), su ubicacion bajo `diagramas/<requisito>/` y su nomenclatura; orquesta las subskills de PlantUML y de especificacion de casos de uso; sincroniza con la documentacion formal y valida los artefactos. Se ejecuta a demanda, con desarrollo completo y preguntando primero al usuario. Es el punto de entrada para diagramar o especificar.
- [PlantUML Embebido en TP Delivery Standards](.agents/skills/tp-delivery-standards/embedded-skills/plantuml/SKILL.md): subskill (orquestada por `tp-uml-y-casos-de-uso`) para crear, editar, convertir o validar diagramas UML con PlantUML. Cargar solo cuando la tarea realmente requiera diagramacion PlantUML.
- [TP Use Case Spec](.agents/skills/tp-use-case-spec/SKILL.md): subskill (orquestada por `tp-uml-y-casos-de-uso`) que genera especificaciones de casos de uso con el formato estandar del TP (.docx + .md espejo) a partir de un JSON. Cargar cuando haya que documentar, crear o actualizar casos de uso.
- [DAL SQL Access Patterns](.agents/skills/dal-sql-access-patterns/SKILL.md): centraliza acceso SQL Server, `ConfigurationManager` y el uso obligatorio de transacciones en la capa DAL.
- [Translation Base Seed Sync](.agents/skills/translation-base-seed-sync/SKILL.md): sincroniza el catalogo base de traducciones de la UI con el bloque auto-generado de `script_bd.sql`.

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

- `pdf` en `~/.codex/skills/pdf`: util para el entregable de generacion/exportacion PDF y revision de artefactos PDF.
- `security-best-practices` en `~/.codex/skills/security-best-practices`: util para login, hashing, auditoria, integridad y endurecimiento basico.

## Reglas transversales

- Todo cambio funcional, bugfix, refactor o cambio de formularios debe respetar el stack documentado y la arquitectura por capas.
- Toda modificacion de referencias entre proyectos o movimiento de clases entre capas debe respetar estrictamente la matriz `UI -> BLL, BE, SERVICIOS`, `BLL -> REPOSITORIO, BE, SERVICIOS`, `REPOSITORIO -> DAL, BE, SERVICIOS`, `DAL -> BE, SERVICIOS`, `BE -> ABSTRACCIONES`, `SERVICIOS -> ABSTRACCIONES`, `ABSTRACCIONES -> sin dependencias`.
- Todo cambio sobre la solucion debe cerrar con compilacion exitosa de `TpIDS.sln`, incluyendo la validacion automatica de arquitectura.
- Todo agregado o renombre de controles visuales WinForms debe respetar la nomenclatura documentada en las skills del repositorio.
- Todo cambio sobre textos traducibles, tags `Form...`, encabezados de grillas o nuevas keys de idioma en la UI debe ejecutar la skill `translation-base-seed-sync` y dejar actualizado `translation-seeds/base.es.json` en el mismo cambio. `script_bd.sql` solo debe cambiar cuando cambie el bloque generador o la ruta resuelta al archivo externo.
- Todo diagrama UML nuevo o modificado debe realizarse en PlantUML y guardarse como codigo fuente dentro del repo antes de reflejarse en la documentacion final.
