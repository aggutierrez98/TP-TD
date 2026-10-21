# Tasks

## 1. Preparacion aislada por caso

- [ ] 1.1 Registrar reproduccion de alta de categoria seguida del caso sin categorias y el falso verde del runner; verificar evidencia TRX con caso inconcluso y exit code cero, con autorizacion previa para tomar el escritorio.
- [ ] 1.2 Incorporar preparacion comun por caso en UiTestBase reutilizando prepare-test-database.ps1 y sembrando credenciales despues de cada reset; mantener staging inicial y ejecucion secuencial. Verificar que dos casos consecutivos parten del mismo seed y que un error de preparacion impide arrancar la app; documentar ciclo en tests/README.md.
- [ ] 1.3 Aislar archivos, configuracion mutable y recursos de cada caso; revisar lecturas estaticas de base y preservar capturas antes del cierre. Verificar que un fallo tras una escritura no contamina el siguiente caso y que las rutas/conexiones siguen apuntando solo a pruebas.

## 2. Fixtures independientes

- [ ] 2.1 Adaptar FIN_ProductosSinCategoriasTests para afirmar la precondicion preparada y retirar Inconclusive/comentario de ejecucion solitaria; verificar aprobado despues de altas de categorias y al repetirlo, solicitando autorizacion UI y filtros explicitos cuando correspondan.
- [ ] 2.2 Revisar todos los fixtures UI y hacer explicitos sus datos particulares sobre el seed por caso, incluidos usuarios, idiomas, integridad y backup; verificar casos representativos en ambos ordenes y documentar el patron de preparacion en tests/README.md.

## 3. Resultado estricto del runner

- [ ] 3.1 Probar y endurecer Assert-TestRunSucceeded con fixtures TRX de aprobado, fallido, inconcluso/NotExecuted con contadores cero, totales inconsistentes y seleccion vacia; verificar que solo la ejecucion completa aprobada retorna cero.
- [ ] 3.2 Publicar contadores efectivos, nombres/motivos de no aprobados y tiempos de preparacion/ejecucion; verificar salida contra TRX y documentar interpretacion del resultado en tests/README.md.

## 4. Verificacion integrada

- [ ] 4.1 Ejecutar tests/tools/run-tests.ps1 -IncludeIntegration y compilar TpIDS.sln; verificar exit code cero y guardias productivas en verde.
- [ ] 4.2 Avisar duracion real de referencia y pedir confirmacion para suite UI completa; ejecutarla con cronometro y verificar todos los seleccionados aprobados sin inconclusos, informando contadores reales y overhead del aislamiento frente a la referencia de 27 min 54,750 s.
