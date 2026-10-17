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
- Diagramas N0X, A0X o D0X: van en sus propios changes cuando el usuario los pida.
- Reescribir G00-G05 (aprobados el 26/08/2026), salvo el modelo de dominio Comercio-Producto.

## Decisions

**Un unico change para todos los diagramas atrasados.** Es mas simple que un change por modulo, porque comparten G07, G08 y el Word.

**De aca en adelante:** un change que altere clases o tablas actualiza G07/G08 dentro de sus propias tareas. Si eso no es posible, agrega una tarea al change de diagramas abierto. Nunca se anota en un archivo de pendientes suelto.

**La fuente de verdad es el codigo y la base real.** Los `.puml` se verifican contra las clases (`.cs`) y contra la base viva con `sqlcmd` (receta en la memoria del proyecto), no contra `script_bd.sql`.

## Risks / Trade-offs

- [El Word esta abierto en Windows y `python-docx` falla con `PermissionError`] → Pedirle al usuario que lo cierre antes de la tarea 4.
- [G07 crece mucho con los modulos nuevos] → Mantener un `.puml` por capa, como hoy.
