# Design

## Context

Ver `proposal.md` (Why). Estado observado:

- `ListaDeCompras` (varias por usuario) con `ItemLista` de `Descripcion` libre, `Cantidad` y `Comprado`. El script deja explicito que el mapeo item → producto no estaba modelado.
- `Producto` (`Nombre`, `CategoriaProductoId`) sin descripcion; `ComercioProducto` con `Precio` y `MonedaId`; `Comercio` con `Latitud`/`Longitud` nulables.
- `Promocion` con filtros N:M opcionales (dias, comercios, rubros, entidades, medios de pago, segmentos, productos y categorias incluidos/excluidos) y `BLL/AlcancePromocion` con `AplicaAComercio`, `AplicaAPago(promocion, entidadFinancieraId, medioPagoId, segmento)`, `AplicaAProducto` y `Vigente`.
- `CuentaFinanciera` del usuario con `EntidadFinanciera` y `MediosPagoEntidad` (entidad, medio, segmento). `CredencialesCifradas` nunca sale del BLL.
- Tras `guardrails-asistente` y `zonas-de-usuario`: categoria `COMPRAS`, function calling, validador de salida con contexto permitido, `ZonaUsuarioBLL` y `DistanciaGeografica`.

## Goals / Non-Goals

**Goals:**
- Que toda afirmacion de la respuesta (comercio, precio, dia, medio, cuenta, distancia) salga de un calculo del BLL.

**Non-Goals:**
- Vincular `ItemLista` a `ProductoId` (decision del usuario: sigue texto libre).
- Base vectorial o tipo `VECTOR` de SQL Server.

## Decisions

**1. RAG estructurado (decision del usuario).** `RecomendacionComprasBLL` arma un `ResumenCompras` serializado a JSON chico: por item, coincidencias, comercios candidatos (nombre, precio, moneda, distancia y zona o "sin ubicacion"), y por comercio las promociones con dias, porcentaje, tope, medio de pago y alias de cuenta. Gemini recibe ese JSON y redacta. El conjunto de referencias permitidas del validador se arma con los mismos nombres.

**2. Flujo en dos llamadas (decision mia).** Primera llamada con function calling en modo `ANY` y una unica funcion `recomendar_compras(nombreLista?)`, sin parametro de usuario, para extraer si la pregunta nombra una lista. Sin nombre: todos los items no comprados de todas las listas del usuario (decision mia). El BLL calcula y la segunda llamada, sin tools, redacta desde el JSON.

**3. Coincidencia local (decision del usuario; umbrales mios).** Normaliza minusculas, acentos, signos y plural simple (`es`/`s`), tokeniza y compara con Jaro-Winkler por token y Jaccard de trigramas contra `Producto.Nombre` y `Producto.Nombre + CategoriaProducto.Nombre`. Acepta si el mejor puntaje supera 0,85 y conserva hasta 3 productos por item. Los umbrales viven en constantes y se calibran con casos de test reales.

**4. Respaldo semantico con embeddings (decision del usuario).** Solo para items sin coincidencia local. Contrato `IServicioEmbeddings` en ABSTRACCIONES (texto → `float[]`, en lote) con implementacion Gemini en SERVICIOS, patron HTTP de `OpenExchangeRatesProveedor` (handler inyectable, timeout explicito, `CancellationToken`). Los productos se comparan por coseno en memoria en C#; se acepta por encima de 0,75 (decision mia, a calibrar). Un fallo del servicio deja el item como no encontrado.

**5. Tabla `ProductoEmbedding` (decision del usuario).** `ProductoId INT PK/FK Producto`, `Modelo VARCHAR(100)`, `TextoFuente VARCHAR(300)`, `Vector VARBINARY(MAX)` (float32 little-endian), `FechaCalculo DATETIME`. `VARBINARY` porque la instancia del laboratorio puede no tener `VECTOR`. Recalculo perezoso (decision mia): al necesitar el respaldo, se recalculan en un solo lote los productos sin fila o con `TextoFuente`/`Modelo` distintos de los actuales. Asi el ABM de productos no depende de la red.

**6. Zonas (decision del usuario).** Comercio con coordenadas es candidato si `DistanciaGeografica` lo ubica dentro del radio de alguna zona; se informa la zona mas cercana y la distancia redondeada a 100 m. Comercio sin coordenadas siempre es candidato. Sin zonas: sin filtro y texto fijo sugiriendo Mis zonas.

**7. Dias y medios (decision del usuario).** Se evalua cada dia de los proximos 7 con `Vigente` y los dias de la promocion; `AplicaAComercio` y `AplicaAProducto` con el producto coincidente; `AplicaAPago` contra cada `MedioPagoEntidad` de cada cuenta del usuario. Solo quedan promociones con al menos una cuenta habilitante. El ahorro estimado aplica el porcentaje y el `TopeDescuento` sobre el importe por item (precio por cantidad), sin acumular promociones.

**7a. Monedas y tope (acordado con el usuario el 2026-10-19).** Se reutilizan las tasas registradas en `ITasaConversionRepositorio`: par directo hacia ARS o inverso si falta el directo. Se convierte el importe a ARS, se aplica el porcentaje limitado por el tope en ARS y se expresa el ahorro y precio final en la moneda original. El resumen incluye tasa y fecha utilizadas. La comparacion de precios finales entre monedas usa su equivalente ARS; ofertas sin tasa van despues de las comparables, sin inventar conversion, ahorro ni precio final promocional. Se conserva su precio original y se indica la falta de tasa. Los topes se presentan como estimaciones por item, no como ahorro acumulable de toda la lista.

**8. Tamano del resumen (decision mia).** Hasta 3 comercios por item, ordenados por precio final y luego por distancia, y hasta 20 items por consulta; el resto se informa como omitido.

## Risks / Trade-offs

- [Umbrales mal calibrados dan falsos positivos] → La respuesta muestra el nombre del producto coincidente; los umbrales se ajustan con tests de casos reales.
- [Modelo y dimension de embeddings sin confirmar] → Ver Open Questions; el diseno no depende de la dimension.
- [Lectura de todo el catalogo y promociones en memoria] → Catalogo chico del TP; si crece, se filtran comercios por productos coincidentes en SQL.
- [Varias llamadas a Gemini por pregunta] → Cuentan contra el limite de uso de `guardrails-asistente`.

## Open Questions

- Modelo de embeddings (verificado el 2026-10-18 en https://ai.google.dev/gemini-api/docs/embeddings): `gemini-embedding-2`, endpoints `models/gemini-embedding-2:embedContent` y `:batchEmbedContents`, `outputDimensionality` 768 (normaliza solo; con `gemini-embedding-001` habria que normalizar a mano). No acepta `task_type`: se antepone `task: sentence similarity | query: {texto}` de igual forma a productos e items. Los espacios de modelos distintos son incompatibles, de ahi la columna `Modelo`. Pendiente: la pagina no informa el cupo gratuito; confirmarlo en la tarea 3.1.
- Cupo gratuito (tarea 3.1, verificado el 2026-10-19): https://ai.google.dev/gemini-api/docs/pricing lista `gemini-embedding-2` con nivel gratuito "Free of charge" para texto (pago: USD 0,20 por millon de tokens; la Batch API asincronica no tiene nivel gratuito, y no se usa). https://ai.google.dev/gemini-api/docs/rate-limits no publica numeros y remite a AI Studio. Una llamada real con la clave del proyecto devolvio 429 con `quotaId` `EmbedContentRequestsPerMinutePerUserPerProjectPerModel-FreeTier`, `quotaValue` 100: cada texto de un `batchEmbedContents` cuenta como un pedido, asi que el nivel gratuito admite 100 textos por minuto. El catalogo de `datos-de-prueba` tiene 160 productos.
- Calibracion del umbral semantico (2026-10-19, 175 textos reales en dos lotes): con el prefijo `task: sentence similarity` los cosenos se concentran arriba. Pares correctos ("birra"/cervezas 0,857-0,874, "detergente" 0,886-0,900, "pollo" 0,870-0,894, "vino tinto" 0,884-0,893); pares equivocados ("fernet"/vinos 0,833, "papel higienico"/toallitas 0,839, "galletitas"/snacks 0,815, "cafe" 0,794); mediana de no relacionados 0,76-0,80. Con 0,75 todo coincidiria.

## Decisiones de implementacion (apply)

- Umbral semantico 0,85 en vez de 0,75 (decision mia, por la calibracion de arriba; vive en una constante).
- Recalculo perezoso acotado al cupo (decision mia): por pregunta se hace a lo sumo una llamada de embeddings con hasta 90 textos (primero los items sin coincidencia local, despues los productos sin vector vigente, por `Id`). Los productos que no entran quedan para la proxima pregunta y la busqueda semantica compara solo contra productos con vector vigente. Un 429 u otro fallo deja esos items como no encontrados.
- `FK_ProductoEmbedding_Producto` con `ON DELETE CASCADE` (decision mia): el vector es un dato derivado y asi la baja de un producto no cambia su comportamiento actual.
- `TextoFuente` = nombre del producto + " " + nombre de su categoria; cambia si cambia cualquiera de los dos.
- Lecturas de la recomendacion en un repositorio de solo lectura `IRecomendacionComprasRepositorio` con consultas con JOIN (items pendientes, productos con categoria, ofertas, promociones que se superponen con los proximos 7 dias, cuentas con medios sin `CredencialesCifradas`), para no hidratar uno por uno.
- Function calling expuesto en `IAsistenteFinancieroService.ObtenerLlamadaHerramientaAsync` con contratos neutros de ABSTRACCIONES (`HerramientaAsistente`, `LlamadaHerramientaAsistente`); el especialista de Compras redacta la segunda llamada con el resumen en `ContextoAsistente.DatosJson`.
- Si el modelo no devuelve la llamada a `recomendar_compras`, o el argumento no es texto, se usa el valor por defecto (todas las listas). Si `nombreLista` no coincide con ninguna lista del usuario (sin mayusculas ni acentos), tambien se usan todas (decision mia: es lo mas util y no expone datos ajenos).
