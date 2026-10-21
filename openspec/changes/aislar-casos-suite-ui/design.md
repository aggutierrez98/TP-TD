# Design

## Context

Ver motivacion en proposal.md. `run-tests.ps1` prepara la base una vez antes de integracion/UI. `AssemblySetup` stagea la app y siembra credenciales una vez; `UiTestBase` solo cierra la app al terminar. `FIN_CategoriasProductoTests` deja datos; `FIN_ProductosSinCategoriasTests` se declara inconcluso si encuentra categorias. `Assert-TestRunSucceeded` solo valida exit code, total positivo y failed=0; el TRX observado tenia total=140, executed=139 y un resultado individual NotExecuted aunque el contador notExecuted era cero.

## Goals / Non-Goals

**Goals:** independencia por caso para toda la suite UI, aislamiento aun despues de fallos y deteccion de cobertura incompleta en el runner. Medir costo antes/despues.

**Non-Goals:** cambios funcionales productivos, redisenar los tests de integracion ni paralelizar FlaUI sobre un escritorio compartido. Este cambio queda planificado para implementacion futura.

## Decisions

### D1. Preparacion central por caso

Agregar inicializacion comun antes de cualquier acceso a la aplicacion: regenerar la base de tests mediante `prepare-test-database.ps1`, sembrar credenciales conocidas y preparar recursos locales mutables. Mantener staging del binario una vez, pero mover la siembra ligada a la base al ciclo por caso. La app debe estar cerrada antes del reset; conservar capturas antes del Dispose y aislar archivos de backup/exportacion por caso. Mantener ejecucion secuencial explicita y evitar corridas concurrentes sobre la misma base/escritorio.

Se elige regeneracion completa por correccion y reutilizacion del script existente, incluidas sus restricciones de nombre de base y seeds. Una transaccion en el proceso de tests no engloba las conexiones del ejecutable WinForms. Limpiar tablas puntualmente es fragil ante FKs y firmas DVH/DVV. Snapshot/restore puede evaluarse despues como optimizacion compatible, sin condicionar esta entrega a privilegios o ediciones de SQL Server adicionales.

### D2. Fixtures autocontenidos

Cada caso crea solo sus datos particulares sobre el seed conocido; retirar suposiciones de datos dejados por otros. Revisar accesos estaticos a traducciones/estado para que no consulten una base aun no preparada o conserven estado de otro caso. El caso sin categorias debe comprobar la precondicion como assertion y fallar si la preparacion esta mal; eliminar el Inconclusive y la instruccion de correr la clase sola.

### D3. Gate TRX estricto

Contrastar contadores y resultados individuales, sin confiar solo en counters.notExecuted/inconclusive: la evidencia muestra que pueden ser cero mientras existe un UnitTestResult NotExecuted. Rechazar cualquier resultado seleccionado distinto de Passed, discrepancias de totales y seleccion vacia. Probar el validador con fixtures TRX de aprobado, fallido, inconcluso/no ejecutado y contadores inconsistentes.

### D4. Validacion de independencia

Reproducir primero el defecto con alta de categoria seguida del caso vacio; verificar ambos ordenes y repeticion despues del cambio. Las corridas UI requieren aviso y confirmacion del usuario; filtros solo si los pide explicitamente. Cerrar con suite completa, conteos efectivos y duracion total y UI. Referencia del 2026-10-20: completa 27 min 54,750 s, 139 UI aprobados y uno inconcluso; aislada 36,372 s total, caso 10,828 s aprobado. No prometer cinco minutos basandose en la estimacion previa.

## Risks / Trade-offs

- [Regenerar por caso aumenta tiempo] → Medir overhead de preparacion por caso y corrida; priorizar aislamiento correcto, optimizar posteriormente conservando contrato.
- [Conexiones o procesos bloquean reset] → Cierre determinista de app y conexiones, error explicito si no se puede preparar; no continuar con datos anteriores.
- [Tests dependian de seed alterado por otros] → Adaptar fixtures a precondiciones propias y validar orden alternativo.
- [Backups/configuracion mutable escapan al reset SQL] → Directorios por caso y restauracion de configuracion de staging en el mismo ciclo de vida.

## Migration Plan

Implementar solo tras aprobacion. Introducir ciclo comun, adaptar fixtures y endurecer el runner; verificar antes de adoptar el nuevo gate. Cambios solo en infraestructura de desarrollo; rollback mediante reversion de esos cambios, sin migracion productiva.
