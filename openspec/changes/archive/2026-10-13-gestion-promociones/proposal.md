# Proposal

## Why

Las promociones hoy existen solo como datos de lectura (el detalle del comercio las lista) y no hay forma de cargarlas ni mantenerlas. El operador necesita una pantalla para dar de alta, editar, buscar y dar de baja promociones con la complejidad real de las promociones argentinas: porcentaje y tope, vigencia, dias, comercios o rubros, medio de pago, entidad financiera y alcance sobre productos y categorias con inclusiones y exclusiones.

**Dependencia**: este change se implementa **despues** de `desacoplar-producto-de-comercio` (las promociones apuntan a productos del catalogo independiente) y de `retirar-helper-violacion-fk` (las bajas pasan a consultar el uso de antemano y este change les suma las tablas de promociones).

## What Changes

- Nueva pantalla **Gestion de Promociones** (un solo formulario: buscador y grilla arriba, editor abajo) como submenu de Operador, con permiso nuevo `gestionpromociones` para los grupos Operador y admin.
- Buscador por texto (sobre la descripcion) y por comercio (combo con los comercios existentes). Al filtrar por un comercio se listan las promociones que **le aplican**: las que lo tienen cargado, y las que no restringen comercio y por lo tanto le valen, respetando el filtro de rubro.
- Alta, edicion y baja de promociones. Una promocion define: descripcion (es su nombre), porcentaje de descuento, tope de reintegro opcional, vigencia (desde/hasta), dias de la semana, y filtros opcionales de comercios, rubros de comercio, entidades financieras, medios de pago, segmentos, productos y categorias de producto.
- Todos los filtros son opcionales: **vacio significa todos**. Entre filtros distintos se deben cumplir todos; dentro de un mismo filtro alcanza con cumplir una opcion. Productos y categorias admiten **incluidos y excluidos**, y la exclusion siempre gana.
- El medio de pago pasa a ser independiente de la entidad: se puede definir una promocion para "Visa credito de cualquier banco" (medio sin entidad ni segmento), para una entidad concreta, o ambos.
- La baja de una promocion se bloquea si una notificacion/recordatorio la referencia, con chequeo explicito en el BLL (sin `ViolacionIntegridadReferencialHelper`).
- La grilla de promociones del detalle del comercio (hoy solo lectura) usa la misma regla de "promociones que aplican a un comercio".
- **BREAKING**: se reemplaza la tabla `PromocionMedioPagoEntidad` por filtros independientes de entidad, medio de pago y segmento; `PromocionBLL` deja de ser de solo lectura y cambia su API.

## Capabilities

### New Capabilities
- `gestion-promociones`: pantalla de Operador para buscar, dar de alta, editar y dar de baja promociones, con sus validaciones, bloqueo de baja, permiso y menu.
- `alcance-de-promociones`: reglas que definen a que comercios, pagos, dias y productos aplica una promocion (filtros opcionales, vacio = todos, combinacion entre y dentro de filtros, inclusiones y exclusiones) y que promociones le aplican a un comercio.

### Modified Capabilities
<!-- Ninguna: no hay specs vigentes de promociones. -->

## Impact

- **BE**: `Promocion` suma las listas de filtros (comercios, rubros, entidades, medios de pago, segmentos, productos y categorias incluidos/excluidos).
- **DAL / REPOSITORIO**: nuevos repositorios de los filtros de entidad, medio de pago, segmento, productos y categorias; se retira `PromocionMedioPagoEntidad`; `IUnidadDeTrabajo` expone los repositorios que la edicion transaccional necesita.
- **BLL**: `PromocionBLL` pasa a CRUD + busqueda; `ProductoBLL.Eliminar` suma el chequeo de uso por promociones; `CompositionRoot` se actualiza.
- **UI**: nueva `FormGestionPromociones`; `FormPrincipal` (submenu y permiso); `FormDetalleComercio` (regla de promociones que aplican).
- **BD**: `script_bd.sql` (tablas de filtros, permiso `gestionpromociones`) y migracion en `TpCatedraIDS` y `TpCatedraIDS_Test`.
- **Traducciones**: keys nuevas en `translation-seeds/base.es.json` y `base.in.json`, sincronizadas en ambas bases.
- **Tests**: se reescriben `PromocionBLLTests` y los de integracion de promociones, y se agregan los nuevos; tests UI FlaUI despues del desarrollo.
- **Documentacion**: el cambio de clases y DER queda registrado en el change `actualizar-diagramas-tp`.
- **Fuera de alcance**: marcas de producto, subcategorias, "excluir productos ya rebajados", combinaciones tipo "Galicia con Visa o Macro con Mastercard" dentro de una misma promocion (se cargan como dos), el calculo del ahorro sobre un carrito y la generacion automatica de promociones.
