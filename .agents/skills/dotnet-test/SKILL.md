---
name: dotnet-test
description: Test generation, quality auditing and coverage analysis for .NET. Use when generating new tests, reviewing test quality, finding untested code, or scaffolding test projects. Complements strict-tdd and test-suite-verification. Source: dotnet/skills plugin dotnet-test.
---

# dotnet-test

Skill de generacion, auditoria y analisis de cobertura de tests para .NET. Orquesta sub-skills especializadas para detectar anti-patrones, gaps de cobertura y problemas de testabilidad.

## Contexto del proyecto

- Framework de tests: **MSTest** (ver skill `csharp-mstest`).
- Proyectos de test: `tests/SERVICIOS.Tests` (unit, MSTest) y `tests/TpIDS.UiTests` (UI FlaUI).
- Ciclo de desarrollo: los tests unitarios siguen TDD estricto (ver skill `strict-tdd`); los UI tests se escriben despues del desarrollo.
- La sub-skill `writing-mstest-tests` es la principal referencia para assertions y patrones MSTest en este proyecto.

## Sub-skills disponibles

| Sub-skill | Cuando usarla |
|---|---|
| `writing-mstest-tests` | Escribir o revisar tests MSTest: orden de argumentos en `AreEqual`, reemplazar `ExpectedException` por `Assert.Throws`, patrones `DynamicData`, diagnosticos MSTEST0006/0017/0037. **Principal referencia para este proyecto.** |
| `run-tests` | Ejecutar la suite de tests y diagnosticar fallas de ejecucion (filtros, plataformas, configuracion del runner). Complementa `tests/tools/run-tests.ps1`. |
| `test-anti-patterns` | Detectar anti-patrones en los tests existentes: tests que no fallan nunca, asserts sin mensaje, dependencias de orden, state compartido. |
| `assertion-quality` | Auditar profundidad y variedad de assertions: detectar `Assert.IsTrue` donde deberia ir una assertion especifica. |
| `test-gap-analysis` | Pseudo-mutation analysis para encontrar comportamientos no cubiertos por ningun test. |
| `find-untested-sources` | Listar archivos/clases de produccion sin cobertura de tests. |
| `test-smell-detection` | Auditoria formal de smells (Eager Test, Mystery Guest, etc.). Mas profundo que `test-anti-patterns`. |
| `coverage-analysis` | Metricas cuantitativas de cobertura (.NET). |
| `crap-score` | Calcular CRAP score (complejidad ciclomatica * falta de cobertura) para priorizar que testear. |
| `detect-static-dependencies` | Encontrar dependencias estaticas que impiden la testabilidad unitaria. |
| `scaffold-dotnet-test-project` | Crear un nuevo proyecto de tests desde cero con la configuracion correcta. |
| `test-tagging` | Agregar categorias/tags a los tests para filtrado selectivo. |
| `test-gap-analysis` | Encontrar gaps en la cobertura de escenarios. |
| `grade-tests` | Puntuar la calidad general de la suite. |
| `generate-testability-wrappers` | Generar wrappers/seams para codigo con dependencias estaticas no testeables. Solo usar si la arquitectura lo permite (ver reglas transversales). |
| `migrate-static-to-wrapper` | Refactorizar dependencias estaticas a wrappers inyectables. Requiere aprobacion del usuario dado el impacto arquitectonico. |
| `filter-syntax` | Referencia de sintaxis de filtros de test (vstest `/TestCaseFilter`). |
| `platform-detection` | Manejo de tests condicionados a plataforma. |
| `mtp-hot-reload` | Hot reload durante el desarrollo iterativo de tests. |

## Sub-skills con restricciones en este proyecto

- **`generate-testability-wrappers` y `migrate-static-to-wrapper`**: requieren introducir interfaces e inyeccion de dependencias. En esta arquitectura (BLL construye dependencias con `new` directo, sin inyeccion) esto esta prohibido salvo decision explicita del usuario. Ver `strict-tdd`.
- **`code-testing-extensions`** y **`test-analysis-extensions`**: extensiones poliglotas; usar solo la parte .NET.

## Flujo de auditoria de suite

1. `test-anti-patterns` + `assertion-quality`: deteccion rapida de problemas criticos.
2. `test-gap-analysis` o `find-untested-sources`: identificar gaps.
3. `coverage-analysis` + `crap-score`: priorizar que cubrir primero.
4. `writing-mstest-tests`: implementar los tests nuevos siguiendo el ciclo TDD (`strict-tdd`).
5. Cerrar con `test-suite-verification` (gate obligatorio).

## Related

- [`../strict-tdd/SKILL.md`](../strict-tdd/SKILL.md): ciclo red-green obligatorio para SERVICIOS.
- [`../test-suite-verification/SKILL.md`](../test-suite-verification/SKILL.md): gate de cierre de tarea.
- [`../csharp-mstest/SKILL.md`](../csharp-mstest/SKILL.md): referencia de assertions MSTest.
