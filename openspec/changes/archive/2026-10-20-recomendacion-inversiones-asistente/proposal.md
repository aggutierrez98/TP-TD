# Proposal

## Why

Ante "quiero invertir el 20% de lo que me sobra" el asistente hoy responde de memoria, sin conocer el saldo del usuario ni el mercado actual, y deja que el modelo decida que recomendar. La recomendacion tiene que salir de reglas expertas en codigo, del saldo real del usuario y de datos publicos vigentes, con el modelo limitado a redactar.

## What Changes

- El porcentaje a invertir se indica en cada pregunta del chat. Gemini lo extrae como dato estructurado (parametro 0-100), el codigo lo valida y, si falta, el asistente lo pide sin asumir.
- El BLL calcula el saldo mensual del usuario de la sesion (promedio de los ultimos 3 meses) y el monto a invertir (saldo × porcentaje).
- Reglas expertas en el BLL definen el nivel: saldo negativo (no invertir, consejos de ahorro), ajustado (fondo de emergencia en instrumentos liquidos), comodo (plazo fijo y FCI de renta fija) y alto (diversificar con CEDEARs, acciones y cripto).
- Datos publicos sin API key de data912.com, api.argentinadatos.com y criptoya.com, solo de los tipos del nivel calculado, con cache y degradacion: si una fuente falla se recomienda con lo disponible y se informa que faltaron esos datos.
- Gemini redacta solo desde ese resumen; toda respuesta de inversiones termina con "Informacion orientativa, no constituye asesoramiento financiero", agregado por codigo.

## Capabilities

### New Capabilities
- `recomendacion-inversiones`: monto a invertir a partir del saldo del usuario, nivel por reglas expertas, instrumentos sugeridos y respuesta con aviso legal.
- `datos-mercado-publicos`: obtencion de cotizaciones, tasas, fondos y cripto de fuentes publicas no oficiales con cache y degradacion ante fallas.

### Modified Capabilities
Ninguna.

## Impact

- **Depende de**: `guardrails-asistente` (categoria `INVERSIONES`, function calling, validacion de salida, aislamiento). Independiente de `zonas-de-usuario` y de `recomendacion-compras-asistente`.
- **ABSTRACCIONES/SERVICIOS**: contratos e implementaciones HTTP de las tres fuentes con el patron de `OpenExchangeRatesProveedor` y `System.Text.Json` ya referenciado sin NuGet.
- **BLL**: `RecomendacionInversionesBLL` nuevo, uso de `MovimientoBLL`/repositorio de movimientos, ruteo en `AsistenteFinancieroBLL`, alta en `CompositionRoot`.
- **Traducciones**: aviso legal, pedido de porcentaje, aviso de datos faltantes.
- **Sin cambios de esquema SQL.**
- **Fuera de alcance**: perfil de riesgo guardado (tarea futura del usuario), porcentaje configurable, compra o ejecucion de instrumentos desde la app y fuentes oficiales pagas.
