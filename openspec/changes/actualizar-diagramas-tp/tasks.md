# Tasks

## 1. Modelo de datos (G08)

- [ ] 1.1 Comparar `diagramas/G08-modelo-datos-integrado` contra la base real `TpCatedraIDS` (tablas, columnas, PK/FK, nulabilidad) con `sqlcmd` y corregir el DER, incluidos los cambios de `desacoplar-producto-de-comercio` y `gestion-promociones` del `design.md`; verificar regenerando el SVG sin errores de PlantUML
- [ ] 1.2 Verificar contra la base real los DER por requisito (`T0X-*-datos.puml`) y corregir los que difieran; verificar regenerando sus SVG

## 2. Clases (G07) y dominio (G00)

- [ ] 2.1 Comparar cada `.puml` de `diagramas/G07-clases-por-capa` contra las clases reales de su capa y sumar lo que falte (modulos de negocio, `ComercioProducto`, filtros de promociones, forms nuevos), incluyendo `gestion-categorias-producto`: `FormGestionCategoriasProducto`, retiro del ABM de categorias de `FormDetalleComercio` y validacion de nombre unico en `CategoriaProductoBLL`; este cambio no altera el DER G08. Sin anticipar nada que no exista; verificar regenerando los SVG
- [ ] 2.2 Actualizar `diagramas/G00-descripcion-global-producto/G00-modelo-dominio.puml`: Comercio-Producto pasa a N:M con precio y moneda; verificar regenerando el SVG
- [ ] 2.3 Incorporar mis-notificaciones-usuario en G07: FiltroNotificacion, PaginaNotificaciones, operaciones personales de NotificacionBLL/Repositorio/DAL, FormMisNotificaciones, FormDetalleNotificacion, ActualizadorContadorNotificaciones y MenuNotificacionesItem; contrastar con código implementado y regenerar SVG. G08 no incorpora cambios porque se reutiliza Notificacion sin alterar esquema.
- [ ] 2.4 Incorporar busqueda-textual-ubicacion-comercio en G07 una vez implementado: `IBusquedaUbicacionesService`, `ConsultaUbicacion`, `ResultadoBusquedaUbicacion`, `LimitesGeograficos`, `EstadoBusquedaUbicacion` y `BusquedaUbicacionException` (ABSTRACCIONES), `NominatimBusquedaUbicacionesService` (SERVICIOS), su alta en `CompositionRoot` (BLL) y, en UI, `FormSeleccionarUbicacionComercio` con `CoordinadorBusquedaUbicacion` inyectado desde `FormGestionComercios`; contrastar con el codigo implementado y regenerar los SVG. G08 no cambia porque no hay cambios de esquema ni persistencia.

- [ ] 2.5 Incorporar exportar-movimientos-pdf en G07: IReporteTabularPdf/IReporteTabularPdfService (ABSTRACCIONES), ReporteMovimientosPdf (BE), MovimientoBLL.ExportarPdf y wiring de CompositionRoot (BLL), ReporteTabularPdfService (SERVICIOS) y seleccion/captura/ArchivoPdf de FormMovimientos (UI). Contrastar nombres y dependencias con el codigo y regenerar SVG. G08 no cambia: no hay cambios de esquema.

- [ ] 2.6 Incorporar actualizacion-automatica-tasas una vez implementado. En G07: `EjecucionActualizacionTasas`, `ActualizacionTasaDetalle` y los tres enums nuevos (BE), los campos `EsManual`, `ProtegidaPorUsuarioId` y `FechaProteccion` de `TasaConversion`, `IProveedorTasasCambio` (ABSTRACCIONES), `OpenExchangeRatesProveedor` (SERVICIOS), `ActualizacionTasasBLL` con su alta en `CompositionRoot`, repositorios y DAL del historial, el formulario de actualizacion de tasas y el proyecto `ActualizadorTasas`. En G08: `EjecucionActualizacionTasas`, `ActualizacionTasaDetalle` y las columnas nuevas de `TasaConversion` (ya existen en la base). Contrastar con el codigo y la base reales y regenerar los SVG.

## 3. Secuencias generales

- [ ] 3.4 Coordinar con el usuario los artefactos A02 de exportar-movimientos-pdf: CU de buscar por descripcion, marcar resultados y exportar PDF (Guardar como, cancelacion y errores), su secuencia y reflejo en las especificaciones consolidadas. Contrastar con FormMovimientos, MovimientoBLL.ExportarPdf y ReporteTabularPdfService; generar los artefactos autorizados mediante tp-uml-y-casos-de-uso y validar PlantUML/CU.

- [ ] 3.1 Preguntar al usuario si T02, T04, T05 y T06 llevan secuencia general (hoy solo tienen secuencias por caso de uso, que cubren todo el modulo); si dice que si, crearlas contra el codigo real con la skill `tp-uml-y-casos-de-uso`; si dice que no, registrar la decision en este `design.md`
- [ ] 3.2 Acordar mediante tp-uml-y-casos-de-uso los artefactos de notificaciones personales (consulta paginada, lectura y contador periódico), actualizar diagramas/CU acordados frente al código y verificar PlantUML y especificaciones generadas.
- [ ] 3.3 Acordar mediante tp-uml-y-casos-de-uso la especificacion del CU de ubicacion de comercio (gestion de comercios, ubicar en el mapa, busqueda textual con eleccion explicita de coincidencia y ajuste manual del marcador, errores del proveedor sin alterar la seleccion): hoy `diagramas/` no tiene un CU de comercios; crear o actualizar el CU acordado y sus artefactos frente al codigo implementado de `busqueda-textual-ubicacion-comercio` y verificar PlantUML y especificaciones generadas. No modificar G08.

- [ ] 3.5 Acordar mediante tp-uml-y-casos-de-uso los artefactos de actualizacion-automatica-tasas: secuencia de la corrida programada (`ActualizadorTasas.exe`), de "Actualizar ahora" con tope, y los CU de ver el historial y de proteger o liberar una tasa manual; contrastar con el codigo implementado, generar los artefactos autorizados y validar PlantUML y CU.

## 4. Documento de la catedra

- [ ] 4.3 Acordar y reflejar exportar-movimientos-pdf (A02) en el documento formal y las instrucciones D01/D02 aplicables, con el Word cerrado y consentimiento previo. Incorporar capturas y explicar seleccion del resultado vigente, filtros aplicados, PDF multipagina y Guardar como; verificar con el usuario.

- [ ] 4.1 Con el Word cerrado (confirmarlo con el usuario), reemplazar con `python-docx` las imagenes viejas (T02, T03, T05, T06, T07, G07, G08, G00) por los SVG regenerados y alinear la numeracion G07/G08/T06; verificar abriendo el documento con el usuario
- [ ] 4.2 Acordar y sincronizar Mis notificaciones en el documento de la cátedra y las especificaciones consolidadas, incluyendo filtros solo por fecha, 10 por página, ambos recordatorios y lectura automática; verificar documento con el usuario tras cerrar Word antes de editar.

- [ ] 4.4 Acordar y reflejar actualizacion-automatica-tasas en el documento formal y las especificaciones consolidadas (actualizacion horaria, historial, proteccion manual y limite de requests), con el Word cerrado y consentimiento previo; verificar con el usuario.

## 5. Cierre

- [x] 5.1 Confirmar que `diagramas/TODO-actualizacion-diagramas.md` ya no existe y que ningun archivo del repo lo referencia (`grep -r TODO-actualizacion-diagramas`, salvo changes archivados)
