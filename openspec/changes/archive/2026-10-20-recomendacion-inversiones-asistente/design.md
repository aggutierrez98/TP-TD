# Design

## Context

Ver `proposal.md` (Why). Estado observado:

- `MovimientoBLL.ObtenerResumenBalance(desde, hasta)` y `ObtenerTotalesPorMes(desde, hasta)` ya toman el usuario de `_sesion` y delegan en `IMovimientoRepositorio.ObtenerResumen`/`ObtenerTotalesPorMes`. `Movimiento` no tiene moneda: los montos se tratan como pesos.
- `OpenExchangeRatesProveedor` es el patron HTTP endurecido: ctor `internal` con `HttpMessageHandler`, timeout explicito por `CancellationTokenSource` vinculado y `CancellationToken`.
- `AgenteInversiones` hoy responde solo por prompt.
- Tras `guardrails-asistente`: categoria `INVERSIONES`, function calling y validador de salida con referencias permitidas.
- Fuentes verificadas en vivo el 2026-10-18, sin API key y no oficiales (sin SLA): data912.com (`/live/arg_cedears`, `/live/arg_stocks`, `/live/arg_bonds`, `/live/arg_corp`, `/live/arg_notes`, `/live/mep`, `/live/ccl`, `/historical/{stocks|cedears|bonds}/{ticker}`), api.argentinadatos.com (`/v1/finanzas/fci/{mercadoDinero|rentaFija|rentaVariable|rentaMixta|...}/ultimo` con `fondo`, `horizonte`, `fecha`, `vcp`, `ccp`, `patrimonio`; plazo fijo, cuentas remuneradas, cauciones, inflacion, riesgo pais) y criptoya.com (`/api/{coin}/ars/{volume}`).

## Goals / Non-Goals

**Goals:**
- Que el nivel, el monto y los instrumentos los decida el codigo; el modelo solo redacta.

**Non-Goals:**
- Perfil de riesgo guardado, porcentaje configurable y ejecucion de operaciones (decision del usuario).
- Proyecciones de rendimiento futuro.

## Decisions

**1. Porcentaje por function calling (decision del usuario).** Primera llamada en modo `ANY` con la unica funcion `recomendar_inversion(porcentaje?: number)`, sin parametro de usuario. El BLL valida `0 < porcentaje <= 100`; si falta o es invalido devuelve el texto fijo `Assistant.Investment.AskPercentage` sin consultar fuentes.

**2. Saldo mensual (propuesta aprobada; verificar reutilizacion).** `RecomendacionInversionesBLL` usa `IMovimientoRepositorio.ObtenerTotalesPorMes` para los 3 meses calendario completos anteriores al actual con el usuario de `_sesion`: saldo = promedio de (ingresos − egresos), gasto = promedio de egresos. Meses sin movimientos cuentan como cero. Monto = saldo × porcentaje / 100.

**3. Niveles (decision mia sobre umbrales, guia experta habitual).** Tasa de ahorro = saldo / ingresos promedio, tomando como referencia la regla 50/30/20 (20% de ahorro como objetivo):
- Negativo: saldo <= 0. No invertir; consejos de reduccion de gastos.
- Ajustado: tasa de ahorro < 10%. Fondo de emergencia de 3 a 6 meses de gasto en FCI money market y cuentas remuneradas.
- Comodo: 10% a 25%. Plazo fijo y FCI de renta fija.
- Alto: > 25%. Diversificar con CEDEARs, acciones y cripto, sumando FCI de renta variable.
Los umbrales son constantes del BLL con tests por borde.

**4. Fuentes por nivel (decision del usuario: solo los tipos del nivel).** Ajustado: FCI `mercadoDinero` y cuentas remuneradas. Comodo: plazo fijo y FCI `rentaFija`. Alto: CEDEARs y acciones (data912), cripto BTC/ETH/USDT (criptoya) y FCI `rentaVariable`. Todos los niveles suman inflacion mensual como referencia. Seleccion (decision mia): los 3 FCI de mayor rendimiento a 30 dias, la mejor tasa de plazo fijo y de cuenta remunerada, los 5 CEDEARs y 5 acciones de mayor volumen (liquidez, no rendimiento reciente) y el mejor precio por cripto.

**5. Rendimiento de FCI (decision del usuario: calcular contra historico).** `ultimo` no trae rendimiento: se pide tambien el VCP de 30 dias antes (endpoint por fecha de la misma API, a confirmar al implementar) y se calcula `(vcp_hoy / vcp_previo) − 1`. Si falta el previo, el fondo se informa sin rendimiento.

**6. Contratos y patron HTTP (decision mia en la particion).** Un contrato por fuente en ABSTRACCIONES (`IProveedorMercadoData912`, `IProveedorArgentinaDatos`, `IProveedorCriptoYa`) con implementaciones en SERVICIOS sobre el patron de `OpenExchangeRatesProveedor` (handler inyectable, timeout de 8 s, `CancellationToken`) y `System.Text.Json`. Respuestas inesperadas → excepcion tecnica de la fuente.

**7. Cache y degradacion (decision del usuario; tiempos mios).** Cache en memoria por endpoint en SERVICIOS con reloj inyectable: 15 minutos para cotizaciones y cripto, 6 horas para FCI, tasas e inflacion. Las fuentes se consultan en paralelo; el BLL marca cada fuente fallida en el resumen y la respuesta suma el texto fijo `Assistant.Investment.MissingData` con las fuentes faltantes.

**8. Aviso legal por codigo (decision del usuario).** El BLL concatena al final la key `Assistant.Investment.Disclaimer` en el idioma activo despues de validar la respuesta, en todos los niveles. La CNV considera regulado el asesoramiento personalizado; para el TP alcanza con el aviso.

**9. Validacion de salida.** Las referencias permitidas son los nombres de fondos, tickers, entidades de plazo fijo y exchanges del resumen.

## Risks / Trade-offs

- [Fuentes no oficiales cambian formato o desaparecen] → Parseo defensivo, tests con respuestas grabadas y degradacion.
- [Umbrales simplificados] → Se documentan como orientativos y el aviso legal acompana toda respuesta.
- [Promedio de 3 meses distorsionado por un ingreso extraordinario] → Aceptado; el perfil de riesgo futuro puede refinarlo.
- [Varias llamadas por pregunta] → Cache y limite de uso de `guardrails-asistente`.

## Open Questions

- Verificacion en vivo 2026-10-19 (tarea 2.2): `https://api.argentinadatos.com/v1/finanzas/fci/mercadoDinero/2026/09/18` responde VCP por fecha (`fondo`, `fecha`, `vcp`); `.../fci/mercadoDinero/ultimo` incluye fondos con fechas heterogeneas. El historico se cruza por nombre y fecha de cada fondo, nunca por indice; sin punto a 30 dias no se inventa rendimiento.
- `https://api.argentinadatos.com/v1/finanzas/tasas/plazoFijo`: `tnaClientes` es fraccion anual (0.1925 = 19.25%); se identifica como TNA para clientes, sin asumir acceso de no clientes. `https://api.argentinadatos.com/v1/finanzas/rendimientos`: rendimientos por entidad con `moneda`, `apy` ya en porcentaje y `fecha`; solo se consumen entradas ARS, sin bonuses condicionados. Las rutas supuestas `/tasas/cuentasRemuneradas` y `/tasas/depositos` dieron 404. Documentacion consultada: https://argentinadatos.com/docs/ .
- Fixtures reales recortadas sin alterar campos en `tests/SERVICIOS.Tests/Fixtures/Mercado/`: FCI ultimo/historico, plazo fijo, rendimientos, inflacion, CEDEARs/acciones y BTC. Data912 no publica timestamp ni moneda de la especie en estos endpoints: su fecha es la de consulta y no se presume moneda. El VCP tampoco declara moneda; no se presenta como importe en ARS.
