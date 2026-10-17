# Tasks

## 1. Acceso SQL sin fallback

- [x] 1.1 Ejecutar la baseline de tests segun strict-tdd y registrar los resultados reales antes de modificar comportamiento; verificar unitarios e integracion en verde o documentar el bloqueo SQL. Resultado: exit 0, 490 unitarios y 161 de integracion aprobados (TRX).
- [x] 1.2 Agregar pruebas de integracion con configuraciones aisladas por AppDomain para principal ausente/vacia o inaccesible y alternativa valida, mas el caso principal disponible; ejecutar y observar el fallo de las nuevas expectativas contra el fallback actual. Rojo observado: 4 assertions fallidas, 162 aprobadas; todos los fallos nuevos devolvian el catalogo alternativo en vez del error.
- [x] 1.3 Cambiar AccesoDatosSqlServer para leer solamente `TpIDS`, conservar transacciones, liberar conexiones fallidas y conservar el error tecnico de conexion; verificar las pruebas de 1.2 en verde. Runner completo: exit 0 tras el cambio.

## 2. Backup y configuracion unica

- [x] 2.1 Agregar pruebas aisladas de resolucion de conexion de BackupDAL para `TpIDS` ausente/vacia con alternativa y para principal valida; observar el fallo previo a implementar sin ejecutar RESTORE contra la base compartida. Rojo observado: 3 assertions nuevas fallidas y 167 aprobadas; backup devolvia la cadena alternativa.
- [x] 2.2 Cambiar BackupDAL para aceptar exclusivamente `TpIDS` y rechazar ausencia o contenido vacio; verificar las regresiones de 2.1 y el test existente de backup en verde. Runner completo: exit 0 tras el cambio.
- [x] 2.3 Eliminar `TpIDS-UAI` de UI/App.config y tests/TpIDS.IntegrationTests/App.config, conservando los valores actuales de `TpIDS`; verificar ambos XML, el destino de tests y la compatibilidad del stager UI con una entrada. XML validado: una entrada TpIDS en cada archivo; el stager itera las entradas sin exigir dos.

## 3. Verificacion integrada

- [x] 3.1 Compilar TpIDS.sln con MSBuild segun winforms-build-verification; verificar exit code 0 y guards de arquitectura/composicion, y confirmar que el config generado contiene solamente la conexion productiva `TpIDS`. Build Debug: exit 0, 0 errores, guards OK; config generado validado. Advertencia MSB3277 System.ValueTuple preexistente en la baseline. Esta corrida local no verifica portabilidad offline en laboratorio.
- [x] 3.2 Ejecutar tests/tools/run-tests.ps1 -IncludeIntegration segun test-suite-verification y reportar contadores TRX; verificar exit code 0 o informar explicitamente la limitacion SQL con unitarios ejecutados. Corrida final exit 0: 490/490 unitarios y 170/170 de integracion, sin fallos (TRX). UI no ejecutada: requiere confirmacion de uso del escritorio.
- [x] 3.3 Verificar el recorrido de arranque con la conexion unica y su salida por error antes del login cuando no esta disponible, mediante revision del flujo y pruebas DAL; validar los artefactos con openspec validate conexion-sql-unica-configurada --strict. Revision Program/Inicializador y regresiones DAL confirman el flujo; validacion estricta ejecutada.
