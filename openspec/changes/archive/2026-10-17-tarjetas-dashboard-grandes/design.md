## Context

`FormDashboardAnaliticas` arma cada tarjeta por codigo (`ConstruirPanelTarjeta`) con tamano fijo (`AnchoTarjeta = 380`, `AltoTarjeta = 300`) dentro de un `FlowLayoutPanel` con `AutoScroll`. Los graficos son LiveCharts (SkiaSharp) sin leyenda ni formateo de montos.

## Goals / Non-Goals

**Goals:**
- Tarjetas mas grandes, 2 por fila o 1 si no hay lugar, con scroll vertical.
- Mas informacion por tarjeta, sin tocar el modelo de tarjetas.

**Non-Goals:**
- Reordenar, redimensionar a mano o configurar tarjetas; nuevos tipos de tarjeta.
- Cambiar consultas SQL o el esquema.

## Decisions

- **Ancho por calculo, no por tamano fijo.** Una funcion pura `CalcularColumnas(anchoDisponible)` devuelve 2 si `anchoDisponible >= 2 * 420 + margenes`, si no 1; el ancho de tarjeta es `(anchoDisponible - margenes) / columnas`. Se recalcula en el `Resize` del `FlowLayoutPanel` y reajusta las tarjetas existentes sin reconstruirlas. Se descarta un `TableLayoutPanel` porque obliga a recrear filas al cambiar de columnas.
- **El ancho disponible descuenta la barra de scroll vertical** para que nunca aparezca scroll horizontal; `HorizontalScroll.Visible = false` con `AutoScroll` del flow.
- **Alto fijo de 380 px** (minimo del requisito); si el ancho de una tarjeta es mayor el grafico simplemente se estira.
- **Tarjeta = encabezado (titulo + periodo + total + Quitar) y grafico debajo.** El periodo y el total salen de los mismos datos que alimentan el grafico, sin consultas nuevas.
- **Tortas con leyenda lateral** (LiveCharts `LegendPosition.Right`) y nombre de serie "Categoria - monto (porcentaje)"; barras y area con leyenda abajo y eje Y con `Labeler` de moneda. Los valores se dibujan siempre sobre el grafico (etiquetas de datos de LiveCharts en barras, puntos del area y porciones de torta con porcentaje) y el tooltip queda solo como complemento: el usuario pidio ver todo de una, sin seleccionar cada parte.
- **Textos nuevos** (`FormDashboard.PeriodLabel`, `.TotalLabel`, `.NoData`) en es/en, sincronizados con `translation-base-seed-sync`.

- **Legibilidad (captura del usuario):** el eje Y de barras y area fija `MinLimit = 0`; `Labeler` de moneda con `CultureInfo` es-AR (`$ 3.068.500`) para tooltips y etiquetas de datos, y abreviado `0,0 M` solo en las marcas del eje; padding de 12 px entre el borde de la tarjeta, el encabezado y el grafico, y `DrawMarginFrame`/margenes del grafico para que las etiquetas no se corten. El tooltip se posiciona con `TooltipPosition` fuera de las barras cuando es posible.

## Risks / Trade-offs

- [Leyenda de torta con muchas categorias achica el grafico] → alto fijo de 380 px y leyenda con scroll propio de LiveCharts; se revisa con captura usando los datos de prueba (26 categorias).
- [Reajustar tarjetas al redimensionar puede parpadear] → `SuspendLayout/ResumeLayout` y reasignar solo `Width`.
- [Las dimensiones minimas son decision de diseno, no de usuario] → valores en constantes faciles de ajustar.
