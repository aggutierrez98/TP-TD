## 1. Dominio y consultas (TDD)

- [x] 1.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline y anotar exit code y contadores del TRX. Baseline 2026-10-16: unit 703/703 e integracion 190/190, exit code 0.
- [x] 1.2 Agregar a `MovimientoBLLTests` los tests de `ObtenerTopComerciosGasto` (top 8 ordenado, suma por comercio, total sin comercio, sin datos) y de `ObtenerGastoAcumuladoDiario` (relleno de dias sin egresos, acumulado, variacion al mismo dia, mes anterior mas corto, mes anterior sin egresos, sin datos) y a `DashboardBLLTests` que los tipos nuevos se ofrecen y no se repiten; verlos fallar. Rojo observado: el build corto por la API inexistente (seam deliberado) y, con el seam minimo, 8 tests nuevos fallaron por assertion (706/714). Cambia a 6 tipos la expectativa de `DashboardBLLTests`.
- [x] 1.3 Agregar tests de integracion de `ObtenerTotalesPorComercio` y `ObtenerTotalesPorDia` (aislados por usuario, rango de fechas, solo egresos) en los tests de integracion del repositorio de movimientos; verlos fallar. Escritos junto con 1.2; el rojo de integracion quedo cubierto solo por el corte de build (el runner no llego a integracion con el rojo unitario).
- [x] 1.4 Implementar enum, modelos `TotalPorComercio`/`TotalPorDia`, consultas en `MovimientoDAL`, repositorio e interfaz, y los metodos del BLL; verificar 1.2 y 1.3 en verde sin regresiones. Verde 2026-10-16: unit 714/714, integracion 192/192, exit code 0 (una corrida previa fallo un test de `NominatimBusquedaUbicacionesLimitadorTests` ajeno a este change por tiempo de espera; la repeticion paso).

## 2. Esquema y datos de prueba

- [x] 2.1 Actualizar el CHECK de `TarjetaDashboard.Tipo` en `script_bd.sql` con los dos valores nuevos y aplicar el `ALTER TABLE` a `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar con una consulta que se puede insertar un tipo nuevo y que uno invalido se rechaza. Hecho: CHECK ampliado en `script_bd.sql` y en las dos bases; verificado en transaccion que los dos tipos nuevos se insertan y uno invalido se rechaza.
- [x] 2.2 Asignar comercio a los egresos de ejemplo en `usuario-prueba.json`, extender `generar_script.py` (resolucion por nombre y `UPDATE` de `ComercioId` NULL), regenerar `script_datos_prueba.sql`, ejecutarlo dos veces sobre `TpCatedraIDS` y comprobar que los conteos no cambian y que hay egresos con comercio. Hecho: 33 egresos con comercio en `usuario-prueba.json`; el script se ejecuto dos veces sobre `TpCatedraIDS` con conteos identicos, 79 movimientos de `user` y 33 con comercio (antes 0).

## 3. Pantalla y traducciones

- [x] 3.1 Agregar a `FormDashboardAnaliticas` la construccion de las dos tarjetas (barras horizontales y lineas acumuladas, con resumen, etiquetas y texto de sin datos) y el titulo de los tipos nuevos en el selector; verificar que compila. Hecho: `ConstruirContenidoGastoPorComercio` y `ConstruirContenidoGastoAcumulado`; `TpIDS.sln` compila (exit 0).
- [x] 3.2 Agregar las keys nuevas a `translation-seeds/base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de ambas bases; verificar con una consulta. Hecho: 6 keys es/en en seeds y verificadas por consulta en `TpCatedraIDS` y `TpCatedraIDS_Test`.
- [x] 3.3 Revisar con el usuario las dos tarjetas con los datos de prueba (con y sin comercio, con el mes anterior mas largo y mas corto) y ajustar lo que haga falta. Verificado visualmente por el usuario (2026-10-16); se corrigio el corte del valor del ultimo punto con un margen derecho de 70 px.

## 4. Cierre

- [x] 4.1 Cerrar con compilacion de `TpIDS.sln` y `tests/tools/run-tests.ps1 -IncludeIntegration` en verde, informando los contadores reales. 2026-10-16: `TpIDS.sln` exit 0; `run-tests.ps1 -IncludeIntegration` exit 0, unit 714/714 e integracion 192/192.
