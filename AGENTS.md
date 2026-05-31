# Documentacion del Proyecto (Agentes y Desarrolladores)

Todo el conocimiento de contexto se ubica dentro del subdirectorio `.agents/`. Leer estas referencias antes de actuar y escribir codigo:

## Base del proyecto

- [Stack .NET y WinForms](.agents/01-stack-dotnet-winforms.md): restricciones del stack, version de framework y alcance tecnico.
- [Modelo de 7 Capas](.agents/02-architecture-7-layers.md): restricciones arquitectonicas del proyecto.

## Skills del repositorio

- [Architecture Dependency Guard](.agents/skills/architecture-dependency-guard/SKILL.md): obliga a respetar la matriz exacta de dependencias entre UI, BLL, DAL, BE, SERVICIOS y ABSTRACCIONES; ademas se valida automaticamente durante el build.
- [WinForms Build Verification](.agents/skills/winforms-build-verification/SKILL.md): obliga a compilar `TpIDS.sln` con MSBuild antes de dar una tarea por terminada.
- [WinForms UI Control Naming](.agents/skills/winforms-ui-control-naming/SKILL.md): define la nomenclatura obligatoria para controles visuales y handlers WinForms.
- [TP Delivery Standards](.agents/skills/tp-delivery-standards/SKILL.md): aplica restricciones tecnicas, criterios de entrega, balanceo documentacion-codigo y exigencias del TP.
- [TP Docs Diagram Sync](.agents/skills/tp-docs-diagram-sync/SKILL.md): sincroniza codigo, diagramas PlantUML en repo y documentacion formal del TP.
- [DAL SQL Access Patterns](.agents/skills/dal-sql-access-patterns/SKILL.md): centraliza acceso SQL Server, `ConfigurationManager` y el uso obligatorio de transacciones en la capa DAL.

## Skills externas instaladas y recomendadas

- `pdf` en `C:\Users\agust\.codex\skills\pdf`: util para el entregable de generacion/exportacion PDF y revision de artefactos PDF.
- `security-best-practices` en `C:\Users\agust\.codex\skills\security-best-practices`: util para login, hashing, auditoria, integridad y endurecimiento basico.

## Reglas transversales

- Todo cambio funcional, bugfix, refactor o cambio de formularios debe respetar el stack documentado y la arquitectura de 7 capas.
- Toda modificacion de referencias entre proyectos o movimiento de clases entre capas debe respetar estrictamente la matriz `UI -> BLL, BE, SERVICIOS`, `BLL -> DAL, BE, SERVICIOS`, `DAL -> BE, SERVICIOS`, `BE -> ABSTRACCIONES`, `SERVICIOS -> ABSTRACCIONES`, `ABSTRACCIONES -> sin dependencias`.
- Todo cambio sobre la solucion debe cerrar con compilacion exitosa de `TpIDS.sln`, incluyendo la validacion automatica de arquitectura.
- Todo agregado o renombre de controles visuales WinForms debe respetar la nomenclatura documentada en las skills del repositorio.
- Todo diagrama UML nuevo o modificado debe realizarse en PlantUML y guardarse como codigo fuente dentro del repo antes de reflejarse en la documentacion final.
