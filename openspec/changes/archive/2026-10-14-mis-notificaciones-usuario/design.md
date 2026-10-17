# Design

## Context

Motivación y alcance en `proposal.md`; contrato en `specs/notificaciones-personales/spec.md`.

`FormPrincipal` abre exclusivamente `FormGestionNotificaciones` bajo Operador y el permiso `gestionnotificaciones`. Esa pantalla usa `NotificacionBLL.ObtenerTodas()` y permite cambiar Leida globalmente. `NotificacionBLL` no recibe sesión ni expone consulta personal; su marcado existente admite ambos estados y cualquier destinatario. Los repositorios tienen consultas individuales por usuario, pero separadas por subtipo.

Los tres tipos comparten `dbo.Notificacion`, discriminador `TipoNotificacion`, `UsuarioId`, `Fecha`, `Mensaje` y `Leida`; existe el índice `IX_Notificacion_UsuarioId`. `ISesion.GetUsuarioActualId()` y la inyección manual en `CompositionRoot` están disponibles. `FormMovimientos` implementa fechas opcionales con checkbox y `.Value.Date`. Los tests existentes de notificaciones cubren administración y consultas individuales.

Restricciones: .NET Framework 4.7.2/C# 7.3, arquitectura de siete capas, ADO.NET transaccional, Krypton y traducciones. La solución productiva debe compilar sin restore ni caché NuGet; este desarrollo no agrega dependencias externas. El cambio abierto `migrar-json-system-text-sin-nuget` trata una dependencia productiva ajena a esta funcionalidad; una compilación con caché no acredita portabilidad de toda la solución.

## Goals / Non-Goals

**Goals:**
- Consultas personales acotadas y verificadas también en BLL/DAL, sin depender de la visibilidad del menú.
- Paginación real en SQL y contador ligero, sin descargar todo el historial.
- Ciclo de actualización seguro ante concurrencia, fallos y cierre de sesión.

**Non-Goals:**
- Crear procesos automáticos que generen recordatorios; se consultan los ya persistidos.
- Incorporar buscador de texto, filtros de estado, eliminación personal, marcado como no leída o marcado masivo.
- Actualizar automáticamente la grilla cada 30 segundos: el polling corresponde al contador; abrir la bandeja, Buscar o navegar vuelven a consultar el listado.

## Decisions

### 1. Casos de uso personales en el BLL existente

Inyectar `ISesion` en `NotificacionBLL` desde CompositionRoot. Añadir operaciones personales de consulta paginada, contador, detalle con lectura y marcado; obtienen el destinatario de la sesión, no de un parámetro elegido por UI. Capturar y validar el id al comienzo de cada operación, antes de usar repositorios. Mantener diferenciadas las operaciones de administración existentes, con regresión de sus tests. Alternativa descartada: exponer ObtenerTodas y filtrar en UI, que entrega datos ajenos y descarga todo el historial.

### 2. Consulta unificada sobre la tabla existente

Ampliar `INotificacionRepositorio`, su implementación y `NotificacionDAL` con operaciones personales explícitas que incluyen los tres discriminadores. Añadir en BE `FiltroNotificacion` y `PaginaNotificaciones` (elementos, total, página y tamaño fijo 10), ajustando nombres a convenciones locales. Proyectar el subtipo por discriminador sin hidratar usuarios/comercios/promociones innecesarios para una grilla que muestra mensaje, fecha, tipo y lectura.

Usar parámetros SQL, filtro `UsuarioId`, fechas y `ORDER BY Fecha DESC, Id DESC`, con `OFFSET/FETCH` y `COUNT` bajo el mismo alcance de filtro. Mantener total y filas consistentes dentro de la transacción de lectura; usar aislamiento adecuado para que una inserción concurrente no produzca combinaciones inconsistentes dentro de esa respuesta. Normalizar páginas fuera del rango a la última disponible; vacío: página interna 1, cero páginas totales, navegación deshabilitada.

Desde es inclusivo; Hasta se convierte en límite exclusivo del día siguiente, con protección para fecha máxima. BLL valida rango y página. No reutilizar la consulta existente de tipo base cambiando sus semánticas, porque sus tests exigen excluir recordatorios. Alternativa descartada: concatenar tres listas completas y paginar en memoria.

### 3. Lectura restringida y transaccional

Consultar detalle con `Id` y `UsuarioId`; notificación ajena e inexistente se tratan como no disponible. La actualización personal usa `WHERE Id = @Id AND UsuarioId = @UsuarioId`, modifica únicamente `Leida` y no permite reasignar el destinatario. Para detalle, la consulta y transición a leída suceden en una unidad de trabajo, devolviendo el mensaje tras confirmar Save. Auditar una transición efectiva con claves personales; un registro ya leído no genera cambios ni auditorías duplicadas. Reutilizar el patrón de bitácora transaccional del BLL actual.

Alternativa descartada: llamar directamente a `MarcarLeida(id, true)` administrativo, que no comprueba propiedad y actualiza campos adicionales mediante el mapeo de entidad.

### 4. Bandeja MDI y detalle modal

Nueva `FormMisNotificaciones` abierta con el patrón `AbrirHijoMdiUnico<T>`; nueva `FormDetalleNotificacion` modal, ambas `FormTraducibleBase`. Grilla de solo lectura (Estado, Fecha, Tipo y Mensaje), fila seleccionable, Ver detalle, Marcar como leída, Anterior/Siguiente y total. Doble clic válido abre detalle. El mensaje completo se presenta en un control de texto multilínea de solo lectura; se conservan saltos de línea y no se interpreta HTML.

Filtros Desde/Hasta según Movimientos, inicialmente desactivados. Buscar aplica filtros y vuelve a página 1; navegación conserva filtros aplicados aunque se hayan editado controles sin pulsar Buscar. Estados vacíos y acciones sin selección deshabilitadas. Tras leer, actualizar la fila y notificar al shell mediante un evento del formulario, sin convertir BLL en Service Locator.

### 5. Badge nativo y polling

Agregar `mnuMisNotificaciones` junto a Sesión. Acceso para cualquier sesión activa, independiente del permiso administrativo. Decisión técnica propuesta: badge dibujado con el renderer del MenuStrip nativo y el tema existente, reservando espacio para el número sin perder traducción o foco. Cantidad cero: ocultar el badge y conservar el nombre del menú. Exponer el contador en descripción accesible.

`MenuNotificacionesItem.GetPreferredSize` reserva el ancho medido del texto, el badge y los márgenes. Al pintar, se usa el renderer vigente para fondo y texto, con tres píxeles entre el área de texto y el badge y siete píxeles libres a su derecha para separarlo de Finanzas. El Padding por sí solo no delimita el texto del ToolStripMenuItem: provocó la superposición y se reemplazó por estas áreas explícitas. Una regresión de render verifica español/inglés y cantidades 1, 25 y 1000.

`FormPrincipal` administra un `System.Windows.Forms.Timer` de 30 segundos y una operación asíncrona de consulta usando `Task.Run` sobre BLL síncrono existente. Todas las asignaciones de controles se realizan al regresar al hilo UI. Usar una única operación de contador en vuelo; una lectura producida durante ella solicita una actualización pendiente, para no perder el refresco inmediato tras lectura. Las respuestas quedan asociadas al id de sesión y a una generación/versión del shell, descartándose si la sesión/ventana terminó o si son obsoletas respecto de una lectura. Cada consulta utiliza su propia conexión/unidad de trabajo, sin compartir recursos SQL entre hilos.

Detener/disponer timer al logout y al cierre por cualquier vía; invalidar generación antes de finalizar. Un fallo de polling mantiene último contador confirmado y reintenta en el siguiente intervalo, registrando el error con el mecanismo técnico disponible sin diálogos repetitivos. Antes de la primera consulta válida no se inventa un cero confirmado. Alternativa descartada: SQL push/Service Broker o dependencias de notificaciones remotas, innecesarias para latencia acordada de 30 segundos.

### 6. Traducciones y verificación

Todos los textos, tipos visibles, errores, pluralización, detalle y formato paginado usan keys traducibles; sincronizar catálogo base. Tests unitarios de BLL y de coordinación de refresco con dobles deterministas, integración SQL de aislamiento, filtros, paginado y transición transaccional, mediante TDD estricto para comportamiento BLL/DAL. Los tests UI se desarrollan después y se ejecutan según autorización de escritorio.

Los detalles técnicos anteriores son decisiones propuestas del autor; el usuario acordó navegación, badge de no leídas, lectura automática, marcado explícito, intervalo 30 s, filtros por fecha y 10 por página, incluyendo los tres tipos. La última corrección elimina expresamente el campo de texto del diseño.

La actualización de diagramas, casos de uso y documento de cátedra pertenece exclusivamente a `actualizar-diagramas-tp`, tareas 2.3, 3.2 y 4.2; no integra las tareas de este cambio.

## Risks / Trade-offs

- [Inserciones entre páginas] → orden estable dentro de cada consulta; no se promete snapshot de toda la sesión de navegación. Reconsulta total y corrige página si hubo bajas administrativas.
- [Respuesta antigua restaura contador previo a una lectura] → versionar respuestas y programar refresco pendiente tras lectura.
- [Polimorfismo y joins innecesarios] → proyección personal liviana y tests que comprueban los tres discriminadores.
- [Dibujado del badge en traducciones largas/DPI alto] → validar espacio, foco, contraste y número de varias cifras en el renderer existente.
- [SQL lento] → operaciones en segundo plano, timeout existente, consulta parametrizada y reuso del índice disponible; no agregar índices sin evidencia.
- [Otros cambios editan FormPrincipal/CompositionRoot] → revisar diff actual antes de aplicar e integrar conservando navegación y dependencias de los cambios abiertos.

## Migration Plan

No se requiere nueva tabla ni modificación destructiva del esquema. Desplegar la aplicación y su catálogo actualizado sobre la tabla existente; las notificaciones y sus estados conservan sus valores. Revertir la funcionalidad revierte código/recursos; las lecturas persistidas siguen válidas. Verificar build productivo y suite de desarrollo; comprobar portabilidad en entorno limpio y reportar cualquier bloqueo del cambio independiente de JSON.
