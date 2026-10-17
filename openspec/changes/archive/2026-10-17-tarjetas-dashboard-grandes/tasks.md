## 1. Disposicion responsiva

- [x] 1.1 Extraer a un metodo estatico la logica de columnas y ancho de tarjeta (`CalcularColumnas`, `CalcularAnchoTarjeta`), reemplazar las constantes fijas de `FormDashboardAnaliticas` y recalcular en el `Resize` del flow sin reconstruir tarjetas; verificar que compila. Hecho: `CalcularColumnas`/`CalcularTamanoTarjeta` y reajuste en `SizeChanged`; `TpIDS.sln` compila (exit 0).
- [x] 1.2 Garantizar scroll solo vertical (sin scroll horizontal) y alto de tarjeta de 380 px; verificar con capturas a 1900 px (2 por fila), a 700 px (1 por fila con scroll) y al maximizar/restaurar. Verificado visualmente por el usuario (2026-10-16).

## 2. Informacion de la tarjeta

- [x] 2.1 Agregar a cada tarjeta el periodo y el total, y el texto traducido de "sin datos" cuando corresponda. Hecho: resumen de periodo/total y texto de sin datos en cada tarjeta.
- [x] 2.2 Tortas: leyenda lateral con categoria, monto y porcentaje; barras y area: leyenda y eje Y con montos en moneda local. En todos los graficos los valores se ven escritos sobre el grafico sin pasar el mouse (etiquetas de datos); el tooltip es solo complemento. Hecho: leyendas, ejes y etiquetas de datos en los cuatro graficos; compila.
- [x] 2.3 Agregar las keys nuevas a `translation-seeds/base.es.json` y `base.in.json`, ejecutar `translation-base-seed-sync` e insertarlas en `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar con una consulta que existen en los dos idiomas. Hecho 2026-10-16: 5 keys `FormDashboard.*` en es/en, seeds sincronizados y verificadas por consulta en ambas bases.

- [x] 2.4 Legibilidad: eje Y desde 0, formato de moneda uniforme (`$ 3.068.500`) en tooltips y etiquetas, abreviado solo en el eje, y padding entre borde, encabezado, grafico y leyenda; verificar con captura que nada se superpone con el comparativo mensual de los datos de prueba. Verificado visualmente por el usuario (2026-10-16).

## 3. Cierre

- [x] 3.1 Revisar con capturas las 4 tarjetas con los datos de prueba (incluida la torta con 26 categorias) y cambiar el idioma con la pantalla abierta. Verificado visualmente por el usuario (2026-10-16); el usuario indico que no hacen falta tests UI nuevos para el dashboard.
- [x] 3.2 Cerrar con compilacion de `TpIDS.sln` y `tests/tools/run-tests.ps1 -IncludeIntegration` en verde, informando los contadores reales. Estado 2026-10-16: `TpIDS.sln` exit 0; `run-tests.ps1 -IncludeIntegration` exit 0, unit 703/703 e integracion 190/190 (UI pendiente).
