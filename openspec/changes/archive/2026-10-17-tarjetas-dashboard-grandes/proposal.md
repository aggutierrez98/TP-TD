## Why

Las tarjetas del dashboard miden 380x300 px fijos: los graficos quedan chicos, sin leyenda ni montos, y es dificil seguirlos. Con pantallas anchas sobra espacio y con pantallas chicas el usuario no tiene un comportamiento claro.

## What Changes

- Las tarjetas se redimensionan segun el ancho disponible: 2 por fila cuando hay lugar para dos tarjetas de ancho minimo, y 1 por fila cuando no; en ese caso el dashboard hace scroll vertical y nunca horizontal.
- Cada tarjeta es mas alta que la actual y muestra mas informacion: periodo y total, leyenda con montos (y porcentaje en las tortas), ejes con montos formateados y tooltips con el valor exacto.
- Textos nuevos traducidos (es/en) para el periodo, el total y los estados sin datos.
- No cambia el modelo de tarjetas (agregar, quitar, orden) ni el esquema SQL.

## Capabilities

### New Capabilities
- `tarjetas-dashboard`: disposicion responsiva de las tarjetas del dashboard y informacion minima que muestra cada una.

### Modified Capabilities

Ninguna. No hay spec vigente del dashboard en `openspec/specs/`.

## Impact

- Codigo: `UI/FormDashboardAnaliticas.cs` y su Designer. Si hace falta un dato nuevo (por ejemplo el total del periodo), se calcula en la UI con lo que ya devuelve `MovimientoBLL`; no se agregan metodos de BLL salvo que el diseno lo requiera.
- Traducciones: seeds es/en y `Palabra`/`Traduccion` de `TpCatedraIDS` y `TpCatedraIDS_Test`.
- Tests UI FlaUI de la pantalla (si existen) y capturas de verificacion visual.
