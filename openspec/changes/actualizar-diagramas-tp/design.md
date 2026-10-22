# Design

## Context

La revision del 26-27/08/2026 dejo T01-T07, G06 y G07 balanceados contra el codigo de ese momento: se corrigieron T02, T03, T05, T06b y T07, y el resto estaba al dia. Ademas se regeneraron los SVG y el consolidado de casos de uso. Desde entonces:

- **Renumeracion (29/08/2026):** `G06-clases-por-capa` -> `G07-clases-por-capa`, `G07-modelo-datos-integrado` -> `G08-modelo-datos-integrado`, y T06a + T06b se fusionaron en `T06-bitacora-control-cambios`. El Word no se sincronizo.
- **`desacoplar-producto-de-comercio`:**
  - G07: `Producto` queda con `Id`, `CategoriaProducto` y `Nombre`. Se suma la clase BE `ComercioProducto` (`Comercio`, `Producto`, `Moneda`, `Precio`), con `ComercioProductoDAL`, `ComercioProductoRepositorio`, `IComercioProductoRepositorio` e `IUnidadDeTrabajo.ComercioProductos`. En BLL estan `ProductoBLL` (catalogo) y `ComercioProductoBLL`; en UI, `FormGestionProductos`.
  - G08: `dbo.Producto` sin `ComercioId`, `MonedaId` ni `Precio`. Tabla nueva `dbo.ComercioProducto`: PK `ComercioId`+`ProductoId`, FK a Comercio, Producto y Moneda, `Precio > 0`.
  - G00: Comercio-Producto pasa a N:M con atributos.
- **`gestion-promociones`:**
  - G07: `Promocion` suma listas de filtros (`ComercioIds`, `RubroComercioIds`, `EntidadFinancieraIds`, `MedioPagoIds`, `Segmentos`, `ProductoIncluidoIds`, `ProductoExcluidoIds`, `CategoriaIncluidaIds`, `CategoriaExcluidaIds`). En BLL, `AlcancePromocion` (nuevo) y `PromocionBLL` con `Buscar`, `Crear`, `Actualizar` y `Eliminar`. Repositorios nuevos de filtros: `PromocionEntidadFinanciera`, `PromocionMedioPago`, `PromocionSegmento`, `PromocionProducto` y `PromocionCategoriaProducto`, en `IUnidadDeTrabajo`. Se retira `PromocionMedioPagoEntidad`. En UI, `FormGestionPromociones`.
  - G08: esas cinco tablas nuevas (las dos ultimas con `Excluido`); se retira `dbo.PromocionMedioPagoEntidad`.
- **Modulos sin registro previo de impacto en diagramas:** entidades financieras, medios de pago, cuentas financieras, reglas de categorizacion, comercios y rubros, monedas, notificaciones, lista de compras, dashboard y balance. G07 y G08 pueden no incluirlos.

## Goals / Non-Goals

**Goals:**
- G07 y G08 reflejan exactamente el codigo y la base implementados, sin anticipar nada que no exista (convencion G0X: G07/G08 son solo lo implementado).
- El Word de la catedra queda sincronizado con los `.puml` del repo.

**Non-Goals:**
- Artefactos N0X adicionales a las secuencias CUN06-CUN87 y a los siete diagramas de clases por proceso aprobados; artefactos A0X o D0X no autorizados expresamente.
- Reescribir G00-G05 (aprobados el 26/08/2026), salvo el modelo de dominio Comercio-Producto.

## Decisions

**Skill de publicacion aprobada (21/10/2026).** `.agents/skills/publicar-diagramas-google-docs/` coordina cualquier `.puml`, sin limitarse a secuencias/CUN. Un script Python prepara PNG a 200 DPI, respeta includes relativos y fuentes originales, oculta leyendas en el render, limita pixeles y crea un manifiesto de artefactos (no un archivo de progreso). Un modulo JavaScript puro planifica/verifica inserciones desde snapshots JSON: destino explicito por ruta de encabezados y placeholder, identificadores UTF-16 y tabId reales, ancho/alto segun pagina, conservacion de imagenes, deteccion de ambiguedad, guardia de revision y orden descendente. La skill indica la carpeta local, espera el enlace de Drive tras la carga manual y prioriza conectores nativos por operacion; One es fallback descubierto dinamicamente, sin IDs de conexiones hardcodeados. No automatiza permisos ni subidas binarias. Las pruebas son offline con snapshots sinteticos y renderizados reales en carpeta temporal; no editan el Google Doc del TP. Integracion mediante referencia en la skill central e indice AGENTS; sin dependencias ni cambios de compilacion productiva.

**Clases integradas por negocio (correccion aprobada).** La entrega solicitada comprende siete diagramas, uno por N01.1-N01.7 y con la asignacion de CUN indicada en la decision de negocio de abajo. Cada fuente vive en su carpeta N01.* como `<proceso>-clases.puml`, con su SVG. Se integran sin duplicados las clases, operaciones y relaciones participantes de todos los CU del proceso, contrastadas contra el codigo vigente. Los paquetes UI, BLL, REPOSITORIO, DAL, BE, SERVICIOS y ABSTRACCIONES se distinguen por rotulos y colores consistentes cuando participan. Se muestra una vista de los miembros relevantes al proceso, evitando copiar capas completas. Se revisan cobertura, firmas, relaciones reales, sintaxis y proporcion/legibilidad. La entrega de imagenes contiene exactamente siete PNG a 200 DPI en `diagramas/exportaciones/clases-negocio-png/`.

**Exportacion y presentacion de los CUN (pedido posterior del usuario).** Las fuentes PlantUML de los 164 diagramas CUN06-CUN87 no llevan bloques `legend`/`endlegend`, tanto en secuencias como en clases. Los SVG se regeneran desde esas fuentes. Las 82 secuencias se exportan ademas como PNG a 200 DPI en una unica carpeta `diagramas/exportaciones/secuencias-png/`, conservando sus nombres CUN y sin subcarpetas por proceso.

**Diagramas de negocio aprobados (20/10/2026).** Se agregan 82 pares de diagramas (164 fuentes PlantUML y sus SVG): CUN06-CUN87, sin CUN01-CUN05. Carpetas bajo `diagramas/`: `N01.1-operaciones-financieras` (06-09,11-22), `N01.2-evaluacion-financiera` (10,72-75), `N01.3-planificacion-compras` (68-71,78-80), `N01.4-orientacion-financiera` (76-77,87), `N01.5-identificacion-valoracion-financiera` (23-43), `N01.6-oferta-comercial` (44-67), `N01.7-comunicacion-usuarios` (81-86). Los archivos se llaman `CUNnn-<slug>-{secuencia,clases}.puml`; los titulos identifican solo el CUN y su nombre. Cada clase/operacion de la secuencia se contrasta con el codigo y se refleja en el diagrama de clases del mismo CUN. La vista de clases es una proyeccion de las operaciones y entidades participantes, no una copia completa de cada capa. Las especificaciones NEGxx existentes se conservan como referencia de los escenarios. No se requiere DER ni editar el Word externo para esta entrega. Se valida cobertura exacta, sintaxis, renderizado SVG, proporciones y balanceo de nombres/firmas, y se compila la solucion al cierre.

**Especificaciones de negocio aprobadas (20/10/2026).** El usuario aprobo la muestra CUN01 y solicito el mismo detalle para CUN01-CUN87. Un grupo equivale a una carpeta `diagramas/NEGxx-<slug>/`, en el orden de su listado (22 grupos), con un JSON y un Word/Markdown generado por grupo. Se conservan exactamente los identificadores CUN; el generador se ejecuta en modo puntual con `--sin-renumerar`. El escenario muestra exclusivamente acciones del actor y respuestas observables de la interfaz, tratando el sistema como caja negra. Las validaciones se contrastan contra el codigo vigente. Esta entrega solo comprende especificaciones: la generacion del consolidado, los diagramas y la edicion del Word externo requieren otro pedido. La consulta del perfil guardado requiere autenticacion; la posibilidad de completar el cuestionario inmediatamente despues del registro se describe en CUN77.

**Un unico change para todos los diagramas atrasados.** Es mas simple que un change por modulo, porque comparten G07, G08 y el Word.

**De aca en adelante:** un change que altere clases o tablas actualiza G07/G08 dentro de sus propias tareas. Si eso no es posible, agrega una tarea al change de diagramas abierto. Nunca se anota en un archivo de pendientes suelto.

**La fuente de verdad es el codigo y la base real.** Los `.puml` se verifican contra las clases (`.cs`) y contra la base viva con `sqlcmd` (receta en la memoria del proyecto), no contra `script_bd.sql`.

## Risks / Trade-offs

- [El Word esta abierto en Windows y `python-docx` falla con `PermissionError`] → Pedirle al usuario que lo cierre antes de la tarea 4.
- [G07 crece mucho con los modulos nuevos] → Mantener un `.puml` por capa, como hoy.
