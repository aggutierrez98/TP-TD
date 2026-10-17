## Why

El dashboard solo ofrece cuatro tarjetas, todas centradas en categorias y meses. No responde dos preguntas habituales: en que comercios se va el dinero y si el mes en curso va mas rapido o mas lento que el anterior.

## What Changes

- Tarjeta nueva **Gastos por comercio**: barras horizontales con los 8 comercios de mayor gasto del mes en curso.
- Tarjeta nueva **Gasto acumulado del mes**: dos lineas dia por dia (mes en curso y mes anterior) con la variacion al dia de hoy.
- Dos tipos nuevos de tarjeta en el dominio (`TipoTarjetaDashboard`), el CHECK de `TarjetaDashboard.Tipo` y el selector "Agregar tarjeta".
- Dos consultas nuevas de agregacion de egresos (por comercio y por dia) expuestas por `MovimientoBLL`.
- Los movimientos de ejemplo de `script_datos_prueba.sql` pasan a tener comercio en los egresos que corresponden, para que la tarjeta por comercio tenga datos.
- Textos nuevos traducidos (es/en).

## Capabilities

### New Capabilities
- `analitica-gastos-dashboard`: tarjetas del dashboard que analizan el gasto por comercio y el ritmo de gasto mensual.

### Modified Capabilities

Ninguna. La disposicion de las tarjetas (capacidad `tarjetas-dashboard`, change `tarjetas-dashboard-grandes`) se reutiliza sin cambios de requisitos.

## Impact

- Codigo: `BE/TipoTarjetaDashboard.cs`, `REPOSITORIO` (interfaz y repositorio de movimientos), `DAL/MovimientoDAL.cs`, `BLL/MovimientoBLL.cs` y `UI/FormDashboardAnaliticas.cs`.
- Esquema: el CHECK de `TarjetaDashboard.Tipo` en `script_bd.sql` y su `ALTER` en `TpCatedraIDS` y `TpCatedraIDS_Test`.
- Datos de prueba: `datos-de-prueba/usuario-prueba.json`, `generar_script.py` y `script_datos_prueba.sql` (comercio en egresos).
- Traducciones: seeds es/en y `Palabra`/`Traduccion` de ambas bases.
- Tests: `MovimientoBLLTests`, `DashboardBLLTests` y tests de integracion del repositorio de movimientos.
