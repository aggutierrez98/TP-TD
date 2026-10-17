## Context

Las tarjetas se construyen por codigo en `FormDashboardAnaliticas` a partir de `TipoTarjetaDashboard`; los totales salen de `MovimientoBLL` (`ObtenerTotalesPorCategoria`, `ObtenerTotalesPorMes`) sobre consultas agregadas en `MovimientoDAL`. `TarjetaDashboard.Tipo` se persiste como texto con un CHECK de valores validos. Los movimientos de ejemplo de `script_datos_prueba.sql` no tienen comercio, por lo que la tarjeta por comercio saldria vacia.

## Goals / Non-Goals

**Goals:**
- Dos tarjetas nuevas con datos reales del usuario, sin tocar la disposicion existente.

**Non-Goals:**
- Una tarjeta de tasa de ahorro u otros tipos.
- Filtros de periodo configurables: ambas usan el mes en curso (y el anterior).
- Cambiar la UI de agregar tarjeta mas alla de listar los tipos nuevos.

## Decisions

- **Dos consultas nuevas en el DAL, agregadas en SQL**: `ObtenerTotalesPorComercio(usuarioId, tipo, desde, hasta)` (JOIN a `Comercio`, `GROUP BY` nombre, solo `ComercioId IS NOT NULL`) y `ObtenerTotalesPorDia(usuarioId, tipo, desde, hasta)` (`GROUP BY Fecha`). Modelos `TotalPorComercio` y `TotalPorDia` junto a `TotalPorCategoria` en SERVICIOS. Alternativa descartada: traer todos los movimientos y agrupar en la UI, porque la UI no debe procesar datos de dominio y escala peor.
- **El BLL arma el resultado de negocio**: `ObtenerTopComerciosGasto(desde, hasta, cantidad)` devuelve el top N ordenado y el total sin comercio; `ObtenerGastoAcumuladoDiario(mes)` rellena los dias sin egresos con el acumulado previo. La UI solo dibuja. Asi la regla de "relleno" y el top N se prueban con TDD en `BLL.Tests`.
- **Variacion calculada en el BLL** con el acumulado del mes anterior al mismo dia; si el mes anterior mide menos dias que el dia de hoy (por ejemplo hoy es 31), se toma su ultimo dia. Sin egresos en el anterior no hay variacion (evita dividir por cero).
- **Barras horizontales con `RowSeries<double>`** (eje Y con los nombres, eje X desde 0 con la moneda abreviada); el monto completo va como etiqueta de datos en cada barra, igual que en las columnas existentes. **Lineas con `LineSeries<double>`** para el acumulado, con etiqueta solo en el ultimo punto de cada serie para no saturar.
- **Enum y esquema**: `GastosPorComercio` y `GastoAcumuladoMensual` en `TipoTarjetaDashboard`; el CHECK de `script_bd.sql` incluye los seis valores y se aplica `ALTER TABLE ... DROP/ADD CONSTRAINT` a `TpCatedraIDS` y `TpCatedraIDS_Test` (el script de base recrea la BD, asi que las bases existentes necesitan el ALTER).
- **Datos de prueba**: se asigna comercio a los egresos de ejemplo cuyo texto corresponde a un comercio del catalogo (por ejemplo Coto, YPF, Starbucks, Cinemark) en `usuario-prueba.json`; `generar_script.py` resuelve el comercio por nombre y, ademas de insertar, hace `UPDATE` de `ComercioId` en los movimientos de ejemplo ya cargados que lo tengan NULL, para que el script siga siendo idempotente y la base ya poblada se corrija.
- **Claves nuevas** (es/en): `FormDashboard.CardType.GastosPorComercio`, `FormDashboard.CardType.GastoAcumuladoMensual`, `FormDashboard.WithoutCommerceLabel`, `FormDashboard.CurrentMonthLabel`, `FormDashboard.PreviousMonthLabel`, `FormDashboard.VariationLabel`.

## Risks / Trade-offs

- [Los nombres de comercio largos recortan las barras horizontales] → el eje Y reserva ancho y los nombres se truncan con puntos suspensivos a 24 caracteres; el nombre completo queda en el tooltip.
- [El CHECK distinto entre `script_bd.sql` y bases existentes] → el `ALTER` se aplica a las dos bases dev y queda como tarea explicita; las bases de otros clones se recrean con `script_bd.sql`.
- [Mapear movimientos de ejemplo a comercios por texto puede asignar uno equivocado] → se asigna solo cuando la descripcion coincide con un comercio del catalogo; el resto queda sin comercio y alimenta el "sin comercio" del resumen.
