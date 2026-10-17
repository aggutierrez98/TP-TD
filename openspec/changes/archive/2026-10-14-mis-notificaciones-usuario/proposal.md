# Proposal

## Why

Actualmente el operador puede crear notificaciones dirigidas a un usuario, pero el destinatario no tiene una bandeja personal para consultarlas. Completar ese circuito permite leer mensajes y recordatorios y conocer cuántos siguen sin leer mientras se utiliza FINAZAPP.

## What Changes

- Nueva entrada superior «Mis notificaciones», junto a «Sesión», disponible para el usuario autenticado con contador de no leídas.
- Bandeja personal con notificaciones manuales, recordatorios de compra y recordatorios de inversión, ordenada por fecha descendente y paginada de a 10.
- Filtros opcionales Desde/Hasta y botón Buscar, siguiendo el patrón visual de Movimientos. Sin buscador de texto ni filtro por estado.
- Detalle del mensaje y marcado automático como leído al abrirlo; acción explícita de marcado desde el listado.
- Contador actualizado al iniciar, cada 30 segundos y tras leer una notificación, con consultas en segundo plano.
- Aislamiento por usuario tanto para consultas como para cambios de lectura.

## Capabilities

### New Capabilities

- `notificaciones-personales`: consulta paginada, detalle, lectura y contador de mensajes y recordatorios del destinatario autenticado.

### Modified Capabilities

Ninguna.

## Impact

- UI: `FormPrincipal`, nueva bandeja MDI y diálogo de detalle, con Krypton y traducciones.
- BLL y CompositionRoot: casos de uso personales y sesión inyectada.
- BE, REPOSITORIO y DAL: filtros, resultado paginado, consulta unificada y actualización restringida al destinatario sobre la tabla existente `Notificacion`.
- Tests unitarios e integración y catálogo `translation-seeds/base.es.json`. Los diagramas y documentos del TP se gestionan exclusivamente en `actualizar-diagramas-tp`.
- Se utiliza .NET Framework 4.7.2/C# 7.3 y ADO.NET; sin nuevas dependencias NuGet productivas.
