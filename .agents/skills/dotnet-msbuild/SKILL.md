---
name: dotnet-msbuild
description: MSBuild diagnostics and optimization for .csproj projects. Use when investigating build failures, slow builds, project reference errors, or anti-patterns in .csproj files. Source: dotnet/skills plugin dotnet-msbuild.
---

# dotnet-msbuild

Skill de diagnostico y optimizacion de MSBuild. Cubre fallas de build, rendimiento, organizacion de proyectos y patrones de configuracion para soluciones `.csproj`.

## Contexto del proyecto

Este proyecto usa el **formato clasico de .csproj** (no SDK-style): `TargetFrameworkVersion v4.7.2`, archivos `<Compile Include>` explicitos, `packages.config` (si aplica). Esto afecta que sub-skills son utiles:

- **`msbuild-modernization`**: solo aplicar si se decide migrar a SDK-style. No ejecutar de oficio.
- **`AP-05` (anti-patron "Manual File Listing")**: no aplica; en formato clasico los includes explicitos son obligatorios.
- **`directory-build-organization`**: aplica si se agrega un `Directory.Build.props` compartido.

## Sub-skills disponibles

| Sub-skill | Cuando usarla |
|---|---|
| `binlog-failure-analysis` | Error de build poco claro o cascada de errores. Requiere un `.binlog` existente; usa el MCP server `binlog` para queriearlo (no `cat` ni `strings`). |
| `binlog-generation` | Generar el `.binlog` de una corrida fallida: `msbuild TpIDS.sln /bl`. |
| `msbuild-antipatterns` | Revision de `.csproj` buscando anti-patrones (paths hardcodeados, condiciones sin comillas, targets monoliticos, etc.). |
| `resolve-project-references` | Errores de referencia entre proyectos (`<ProjectReference>` roto, dependencias circulares). |
| `incremental-build` | Build completo cuando no deberia serlo; diagnosticar por que los targets no son incrementales. |
| `build-perf-diagnostics` | Build lento: identificar targets costosos, I/O innecesario. |
| `build-perf-baseline` | Establecer baseline de tiempos antes de optimizar. |
| `build-parallelism` | Habilitar o ajustar construccion en paralelo (`/m` flag). |
| `property-patterns` | Buenas practicas para propiedades MSBuild: alcance, override, condiciones. |
| `item-management` | Manejo de `<ItemGroup>`, globbing, metadata. |
| `copy-to-output-directory` | Archivos que no llegan al directorio de salida, o se copian de mas. |
| `extension-points` | Tareas custom, `.props`/`.targets` importados, hooks de build. |
| `target-authoring` | Escribir targets MSBuild correctos con inputs/outputs para soporte incremental. |
| `including-generated-files` | Incluir archivos generados (T4, source generators) en el build. |
| `eval-performance` | Lentitud en la fase de evaluacion de propiedades/items. |
| `check-bin-obj-clash` | Conflictos entre carpetas `bin/`/`obj/` de distintos proyectos. |
| `msbuild-server` | Uso del MSBuild server para reducir tiempo de startup. |
| `msbuild-modernization` | Migracion de formato clasico a SDK-style (solo si el usuario lo solicita explicitamente). |

## Flujo recomendado ante un build failure

1. Si el error es claro en el log de consola: actuar directamente.
2. Si no: generar `.binlog` con `binlog-generation`, luego analizar con `binlog-failure-analysis` via MCP tools.
3. Verificar que `TpIDS.sln` compile exitosamente antes de cerrar la tarea (ver skill `winforms-build-verification`).

## Constraint

Esta skill NO reemplaza la verificacion de arquitectura ni la suite de tests. Al terminar, la skill `winforms-build-verification` sigue siendo obligatoria.
