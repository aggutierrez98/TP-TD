---
name: dotnet-diag
description: Performance diagnosis and debugging for .NET desktop apps. Use when investigating slow UI, high memory usage, hangs, CLR activation errors, or when writing microbenchmarks. Source: dotnet/skills plugin dotnet-diag.
---

# dotnet-diag

Skill de diagnostico de rendimiento y debugging profundo para aplicaciones .NET. Cubre analisis de anti-patrones de performance, recoleccion de trazas y dumps, microbenchmarking y debugging de activacion del CLR.

## Contexto del proyecto

Aplicacion WinForms desktop en .NET Framework 4.7.2. Las herramientas de diagnostico modernas (`dotnet-trace`, `dotnet-dump`) requieren el CLI de .NET instalado en la maquina; en .NET Framework el equivalente es WinDbg + SOS o PerfView. Tener esto en cuenta al elegir la sub-skill.

## Sub-skills disponibles

| Sub-skill | Cuando usarla |
|---|---|
| `analyzing-dotnet-performance` | Scan estatico de ~50 categorias de anti-patrones de performance en codigo C# (async/await, memoria, strings, LINQ, regex, colecciones, I/O). Entrada: codigo fuente. No requiere correr la app. |
| `dotnet-trace-collect` | Capturar trazas de CPU y eventos del runtime en una app corriendo. En .NET Framework usar PerfView como alternativa. |
| `dump-collect` | Capturar un memory dump para analisis de memoria o hangs. En .NET Framework usar ProcDump o Task Manager. |
| `microbenchmarking` | Escribir benchmarks con BenchmarkDotNet para medir con precision. Util antes de optimizar un algoritmo critico. |
| `clr-activation-debugging` | Diagnosticar errores de activacion del CLR: version mismatch, binding failures, `HRESULT` en startup. |

## Sub-skills NO relevantes para este proyecto

- `android-tombstone-symbolication`: solo Android.
- `apple-crash-symbolication`: solo Apple.

## Flujo recomendado ante problema de performance

1. Correr `analyzing-dotnet-performance` sobre el codigo sospechoso (scan estatico, sin costo de ejecucion).
2. Si el problema persiste o no se detecta estaticamente: capturar traza con `dotnet-trace-collect` / PerfView.
3. Para memory leaks o hangs: `dump-collect` + analisis con WinDbg/SOS o dotMemory.
4. Si se va a optimizar un algoritmo puntual: escribir primero el benchmark con `microbenchmarking`.

## Constraint

Las optimizaciones de performance no deben romper la arquitectura de 6 capas ni introducir dependencias no permitidas. Verificar compilacion y suite de tests al terminar.
