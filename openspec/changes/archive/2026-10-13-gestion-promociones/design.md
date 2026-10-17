# Design

## Context

Estado actual (observado en el codigo; la motivacion esta en `proposal.md`):

- `BE/Promocion` tiene `Id`, `Descripcion`, `Descuento`, `TopeDescuento?`, `VigenciaDesde`, `VigenciaHasta` y `DiasSemana` (lista vacia = todos los dias).
- Existen DAL y repositorios para `Promocion` (con `ReemplazarDiasDePromocion`), `PromocionComercio`, `PromocionRubroComercio` y `PromocionMedioPagoEntidad`, todos con el patron `Reemplazar...DePromocion(promocionId, ids)`. `IUnidadDeTrabajo` expone solo `Promociones` y `PromocionComercio`.
- `PromocionBLL` es de solo lectura y solo conoce `ObtenerPorComercio` por la tabla `PromocionComercio`; lo consume `FormDetalleComercio` (grilla de solo lectura).
- `Comercio` tiene un unico `RubroComercio`. `MedioPago` es tipo + marca (`TipoMedioPago`: credito, debito, prepaga, billetera virtual, Cuenta DNI). `MedioPagoEntidad` es entidad + medio + segmento (texto).
- Las notificaciones/recordatorios referencian `Promocion` por `PromocionId`.
- Este change depende de `desacoplar-producto-de-comercio`: `Producto` es un catalogo independiente y `ProductoBLL.Eliminar` ya bloquea por uso.

## Goals / Non-Goals

**Goals:**
- Un ABM de promociones con todos los filtros opcionales, y una regla de alcance simple, testeable y predecible.
- Buscar por texto y por comercio, con la regla "promociones que aplican a un comercio" compartida con el detalle del comercio.

**Non-Goals:**
- Marcas, subcategorias, "excluir productos ya rebajados" y combinaciones por-entidad dentro de una misma promocion.
- Evaluar una promocion contra un carrito o calcular ahorros (el modelo lo permite; la funcion se hace aparte).
- Generacion automatica de promociones.

## Decisions

**1. Un filtro por dimension, todos opcionales, tablas de union.** Tablas: `PromocionComercio` y `PromocionRubroComercio` (ya existen), `PromocionEntidadFinanciera`, `PromocionMedioPago`, `PromocionSegmento` (`PromocionId`, `Segmento VARCHAR`), `PromocionProducto` y `PromocionCategoriaProducto` (ambas con `Excluido BIT`), mas `PromocionDiaSemana` (ya existe). PK compuesta `(PromocionId, <elemento>)`, de modo que un producto o categoria no puede estar dos veces en la misma promocion, es decir ni incluido y excluido a la vez. Alternativa descartada: tablas separadas de incluidos y excluidos, que permitirian la contradiccion y exigirian una validacion extra.

**2. Se retira `PromocionMedioPagoEntidad`.** Se reemplaza por entidad, medio de pago y segmento independientes (decision del usuario: el medio de pago puede ir sin entidad). Costo asumido: se pierde la combinacion exacta "Galicia + Visa o Macro + Mastercard" en una promocion; se cargan dos. La migracion lee primero si hay filas: si las hay, deriva entidades, medios y segmentos distintos de ellas (puede ensanchar el alcance) y lo informa; si no, solo elimina la tabla.

**3. Regla de alcance en el BLL, en un metodo puro.** `Promocion` lleva listas de ids para cada filtro (`ComercioIds`, `RubroComercioIds`, `EntidadFinancieraIds`, `MedioPagoIds`, `Segmentos`, `ProductoIncluidoIds`, `ProductoExcluidoIds`, `CategoriaIncluidaIds`, `CategoriaExcluidaIds`, `DiasSemana`). El calculo de "le aplica a este comercio", "aplica a este producto" y "aplica a este pago" vive en un metodo sin acceso a datos para poder probarlo con tests unitarios sin base. Orden de evaluacion para productos: (1) si hay inclusiones, el producto debe coincidir con alguna (producto directo o su categoria); (2) si coincide con alguna exclusion, queda fuera. La exclusion gana.

**4. Busqueda en memoria.** `PromocionBLL.Buscar(texto, comercioId?)` carga las promociones con sus filtros y filtra en memoria (el volumen es chico y se evitan consultas dinamicas en la DAL). Texto: contiene, sin distinguir mayusculas ni acentos, sobre `Descripcion`. Comercio: usa la regla del punto 3 con el rubro del comercio. Alternativa descartada: SQL con `LIKE` y subconsultas por filtro, mas dificil de probar y de mantener.

**5. Alta y edicion transaccionales.** `Crear`/`Actualizar` guardan la promocion y reemplazan cada filtro dentro de una Unidad de Trabajo con un solo `Save()`, con validaciones previas en el BLL (descripcion, descuento en (0, 100], tope > 0 si existe, desde <= hasta, existencia de cada id referenciado, sin elemento repetido como incluido y excluido). `IUnidadDeTrabajo` expone los repositorios nuevos. Se registra bitacora en cada operacion.

**6. Baja con chequeo explicito.** `Eliminar` verifica dentro de la unidad de trabajo que ninguna notificacion referencie la promocion (`ExcepcionNegocio("Error.PromotionManagement.PromocionInUse")`), borra los filtros hijos y luego la promocion. No atrapa `SqlException` ni usa `ViolacionIntegridadReferencialHelper`. `ProductoBLL.Eliminar` suma el conteo de uso por `PromocionProducto`.

**7. UI: un solo formulario (opcion A elegida por el usuario).** `FormGestionPromociones` (Krypton, MDI unico, `FormTraducibleBase`, nomenclatura de controles del repo): arriba cuadro de texto, combo de comercios con "Todos" y boton Buscar; grilla de promociones; abajo el editor (descripcion, %, tope, desde, hasta, dias) y nueve listas de seleccion multiple (comercios, rubros, entidades, medios de pago, segmentos, productos incluidos y excluidos, categorias incluidas y excluidas), con borde distinto para incluidos y excluidos y la leyenda "vacio = todos". Como la ventana es larga, el editor va en un panel con scroll (la ventana es redimensionable y respeta el minimo de apertura segun `ventanas-redimensionables`). Si un producto incluido pertenece a una categoria excluida, se muestra una advertencia no bloqueante.

**8. Catalogos para las listas.** La UI no referencia REPOSITORIO: los catalogos de las listas se piden a los BLL existentes (comercios, rubros, entidades, productos, categorias de producto) y a `PromocionBLL` para lo que no tenga BLL propio (medios de pago, segmentos distintos).

**9. Menu, permiso y traducciones.** `mnuOperadorPromociones` cuelga de `mnuOperador`, se habilita con `gestionpromociones` y se suma al calculo de `mnuOperador.Enabled`. El permiso se siembra en `script_bd.sql` para Operador y admin y se inserta en las dos bases. Las keys nuevas van en `base.es.json` y `base.in.json` y se insertan a mano en `Palabra`/`Traduccion` de ambas bases.

**10. `FormDetalleComercio`.** Su grilla de solo lectura pasa a usar `PromocionBLL.ObtenerPorComercio`, ahora con la regla del punto 3 (decision mia: misma regla en las dos pantallas, para no mostrar listas distintas del mismo comercio).

## Risks / Trade-offs

- [Filtros independientes pueden ensanchar la migracion de `PromocionMedioPagoEntidad`] → Se revisa la cantidad de filas antes de migrar y se informa el resultado; en desarrollo probablemente no hay filas.
- [Perdida de la combinacion exacta entidad+medio+segmento] → Aceptado por el usuario; se cargan dos promociones.
- [Busqueda en memoria no escala] → El volumen del TP es chico; si crece, se pasa a SQL sin cambiar la API del BLL.
- [Formulario largo con nueve listas] → Panel con scroll y listas compactas; se valida visualmente con capturas.
- [Las tablas nuevas suman FKs hacia Comercio, Rubro, Entidad, MedioPago y CategoriaProducto, y el change `retirar-helper-violacion-fk` hace que cada baja pregunte el uso de antemano en lugar de atrapar el error de la FK] → Orden de implementacion: `desacoplar-producto-de-comercio`, luego `retirar-helper-violacion-fk`, luego este change, que en la tarea 4.4 agrega a esos chequeos las tablas de promociones y retira el de `PromocionMedioPagoEntidad`.
- [Segmento es texto libre] → Se ofrece solo la lista de segmentos distintos ya cargados en `MedioPagoEntidad`, evitando valores inexistentes.
