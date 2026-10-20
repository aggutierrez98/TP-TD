# Tasks

El progreso de este change vive solo en este archivo. Requiere `guardrails-asistente` y `zonas-de-usuario` aplicados. Los cambios de comportamiento en BLL, SERVICIOS, REPOSITORIO y DAL siguen la skill `strict-tdd`.

## 1. Lectura de datos

- [x] 1.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` como baseline y anotar exit code y contadores del TRX. (2026-10-19: exit 0, unit 849/849, integracion 198/198; una primera corrida fallo solo por el test inestable conocido del limitador de Nominatim.)
- [x] 1.2 Tests de integracion de las lecturas necesarias (items no comprados por usuario, productos con categoria, `ComercioProducto` con comercio y moneda, promociones con filtros) en rojo; implementar las consultas faltantes en DAL (skill `dal-sql-access-patterns`) y REPOSITORIO y verlas en verde.

## 2. Coincidencia local

- [x] 2.1 Tests de normalizacion y similitud (acentos, plurales, mayusculas, tokens desordenados, umbral, maximo 3 productos) con casos reales del catalogo en rojo; implementar en BLL y verlos en verde.

## 3. Respaldo semantico

- [x] 3.1 Verificar en ai.google.dev el modelo de embeddings, dimension y cupo gratuito y registrarlo en la seccion Open Questions de `design.md`; verificar que el registro cita la pagina consultada.
- [x] 3.2 Agregar `ProductoEmbedding` a `script_bd.sql` y a `TpCatedraIDS`/`TpCatedraIDS_Test`; tests de integracion de DAL/REPOSITORIO (guardar y leer `VARBINARY` float32, reemplazo) en rojo; implementar y verlos en verde; verificar la tabla por consulta en ambas bases.
- [x] 3.3 Tests de `IServicioEmbeddings` Gemini con handler falso (cuerpo en lote, parseo, timeout, error HTTP) en rojo; implementar en SERVICIOS y verlos en verde.
- [x] 3.4 Tests del respaldo en BLL (coseno, umbral, recalculo por `TextoFuente` o `Modelo` distintos, fallo del servicio deja el item sin coincidencia) en rojo; implementar y verlos en verde. (2026-10-19: retomados tests incompletos; rojo observado: 8 fallos del respaldo, 876 correctos. Verde: runner -IncludeIntegration exit 0, unit 884/884 e integracion 207/207.)

## 4. Recomendacion

- [x] 4.1 Tests de `RecomendacionComprasBLL` (cercania por zona, comercio sin coordenadas, usuario sin zonas, dias de los proximos 7 con vigencia, promociones solo con cuentas del usuario, tope de descuento, conversion directa e inversa a ARS con tasa y fecha, tope alcanzado en otra moneda, tasa faltante sin ahorro inventado y orden entre monedas, items no encontrados, limites de 3 comercios y 20 items, datos de otro usuario ausentes, sin credenciales) en rojo; implementar y verlos en verde. (2026-10-19: rojo 13 fallos nuevos, 885 correctos; verde runner unit exit 0, 898/898.)
- [x] 4.2 Tests del ruteo `COMPRAS` en `AsistenteFinancieroBLL` (function call `recomendar_compras` con y sin `nombreLista`, segunda llamada sin tools, referencias permitidas cargadas en el validador, respuesta con comercio ajeno descartada) en rojo; implementar y verlos en verde. (2026-10-19: rojo 7 fallos nuevos y 898 correctos; verde runner unit exit 0, 905/905.)
- [x] 4.3 Dar de alta servicio de embeddings, repositorio y `RecomendacionComprasBLL` en `CompositionRoot` segun `composition-root-guard`; verificar la guardia compilando. (2026-10-19: MSBuild TpIDS.sln /t:Build /p:Configuration=Debug /m exit 0, guardias de arquitectura y Composition Root correctas.)
- [x] 4.4 Agregar los textos fijos (sin zonas, items no encontrados) a los seeds es/en, ejecutar `translation-base-seed-sync` e insertarlos en ambas bases; verificar por consulta. (2026-10-19: avisos traducidos agregados despues de validar al modelo y persistidos; rojo observado 1 fallo nuevo, 905 correctos; verde -IncludeIntegration exit 0, unit 906/906 e integracion 207/207. Sync ejecutado y ambas keys verificadas en es/en en TpCatedraIDS y TpCatedraIDS_Test.)
- [x] 4.5 Probar en la app con datos de `datos-de-prueba` la pregunta "donde y cuando me conviene comprar lo de mi lista?" y verificar con captura que la respuesta cita comercio, zona, dia, medio y cuenta reales. (2026-10-19: smoke del formulario real, CompositionRoot y Gemini contra TpCatedraIDS, usuario semilla `user`; el login se cubrio separadamente en la suite UI. Como el dataset solo tiene ofertas online, se asigno temporalmente a Coto Abasto un producto/precio de Coto Digital y se agregaron un item y la zona Prueba compras a 600 m; fixture eliminado al terminar. Captura `TestResults/ui-screenshots/FIN_RecomendacionCompras_Real.png`: Coto Abasto a 600 m de Prueba compras, Jumbo Online, fechas 19/20/22 de octubre, medios y cuentas propios. La primera respuesta invento 27/10/2026: se agrego validacion de fechas explicitas contra las del resumen y se repitio exitosamente la prueba.)

## 5. Cierre

- [x] 5.1 Compilar `TpIDS.sln` con MSBuild (skills `winforms-build-verification` y `architecture-dependency-guard`); verificar exit code 0. (2026-10-19: build final tras ajustes de redondeo, aislamiento de sesion y validacion de fechas, exit 0, ambas guardias correctas. Build local sin restore productivo; no reemplaza la verificacion de portabilidad en entorno limpio del change correspondiente.)
- [x] 5.2 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` (skill `test-suite-verification`); verificar exit code 0 y reportar contadores reales. (2026-10-19: gate final exit 0, TRX unit 913/913 e integracion 207/207. Redondeo: rojo 1 fallo nuevo/906 correctos, verde 907/907; aislamiento del Id capturado ante cambio de sesion durante la red: rojo 1 fallo nuevo/907 correctos, verde 908/908; fechas inventadas en redaccion: rojo 3 fallos nuevos/910 correctos, verde 913/913. Suite UI completa autorizada ejecutada antes del ultimo ajuste de validacion: exit 0, 134 aprobados y 1 omitido preexistente `FIN_Productos_SinCategorias_OrientaYDeshabilitaAlta`, seguida de smoke exitoso del chat real sobre la version final.)
- [x] 5.3 Registrar en `openspec/changes/actualizar-diagramas-tp/tasks.md` el pendiente de G07 (`RecomendacionComprasBLL`, coincidencia, `IServicioEmbeddings` y su implementacion, `ProductoEmbedding`) y G08 (`ProductoEmbedding` con FK a `Producto`), y la secuencia de la recomendacion a acordar con `tp-uml-y-casos-de-uso`; verificar que quedo registrado. (2026-10-19: tareas 2.9 y 3.8 registradas.)
