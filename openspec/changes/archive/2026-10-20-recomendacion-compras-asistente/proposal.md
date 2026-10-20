# Proposal

## Why

El asistente no usa los datos de FINAZAPP: ante "donde y cuando me conviene comprar lo de mi lista?" solo puede dar consejos genericos. El sistema ya tiene listas de compras, catalogo de productos con precio por comercio, comercios geolocalizados, promociones con reglas de alcance y cuentas financieras del usuario; falta cruzarlos y que el asistente responda del estilo "Podes comprar en X o Y, que te quedan cerca de tus zonas habituales, y podes comprarlo el dia Z porque hay un 20% de descuento con este medio de pago de esta cuenta financiera que te aplica".

## What Changes

- Recomendacion de compras por RAG estructurado, sin base vectorial: el BLL calcula de forma determinista los candidatos cruzando items pendientes de la lista, productos, precios por comercio, cercania a las zonas del usuario, promociones vigentes por dia y cuentas del usuario que habilitan cada promocion. Gemini solo redacta a partir de un resumen JSON chico y no puede citar nada fuera de el.
- La pregunta es libre y solo desde el chat del asistente (categoria `COMPRAS` de `guardrails-asistente`); no hay boton en Lista de Compras.
- `ItemLista` sigue siendo texto libre. Coincidencia item ↔ producto en dos pasos: similitud local determinista y, solo si no alcanza, embeddings de Gemini comparados en memoria.
- Nueva tabla `ProductoEmbedding` con el vector precalculado de cada producto, recalculado cuando cambia su texto fuente o el modelo.
- Comercios con coordenadas son candidatos si caen dentro del radio de alguna zona del usuario; los comercios sin ubicacion fisica siempre son candidatos; sin zonas se recomienda sin filtro de cercania y se sugiere cargarlas.
- Los items sin coincidencia se informan como no encontrados; nunca se inventan.
- Los precios en otra moneda se convierten con tasas registradas para calcular el descuento hasta el tope en ARS y comparar ofertas. Sin tasa disponible se muestra el precio original sin estimar ahorro.

## Capabilities

### New Capabilities
- `recomendacion-compras`: armado determinista de recomendaciones de comercio, dia, medio de pago y cuenta para la lista del usuario y su respuesta en el asistente.
- `coincidencia-items-producto`: asociacion de un item de texto libre con productos del catalogo, local primero y semantica como respaldo.

### Modified Capabilities
Ninguna. `alcance-de-promociones` se reutiliza sin cambiar sus reglas.

## Impact

- **Depende de**: `guardrails-asistente` (categoria, function calling, validacion de salida, aislamiento) y `zonas-de-usuario` (zonas y distancia).
- **BLL**: `RecomendacionComprasBLL` nuevo, uso de `AlcancePromocion`, `ZonaUsuarioBLL`, listas, catalogo y cuentas; `AsistenteFinancieroBLL` rutea `COMPRAS`.
- **ABSTRACCIONES/SERVICIOS**: contrato de embeddings e implementacion Gemini.
- **SQL**: `ProductoEmbedding` en `script_bd.sql` y en las dos bases; DAL y REPOSITORIO nuevos y consultas de lectura sobre las tablas existentes.
- **Traducciones**: textos fijos del asistente (sin zonas, items no encontrados).
- **Fuera de alcance**: vincular `ItemLista` a `ProductoId`, boton en Lista de Compras, base vectorial y scraping de promociones.
