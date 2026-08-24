# T06a/T06b Checklist de Integracion

## Separacion conceptual

- `Bitacora`: registra operaciones, fallos de negocio y excepciones tecnicas.
- `Historial de Usuario`: versiona el estado de `Usuario`, permisos y preferencia de idioma.
- `BE`: solo estado, sin metodos.

## Responsabilidades por capa

- `TpIDS`: muestra mensajes, consulta bitacora y administra restauraciones desde formularios.
- `BLL`: orquesta transacciones, mapea `Usuario <-> UsuarioHistorialSnapshot` y decide cuando registrar.
- `DAL`: persiste estado actual, bitacora e historiales usando `AccesoDatosSqlServer`.
- `SERVICIOS`: expone `Sesion` como fachada de bitacora y `HistorialEstadosService` como servicio de recuperacion.
- `ABSTRACCIONES`: define contratos, sin dependencias hacia capas superiores.

## Flujo esperado

- Falla de negocio:
  - `BLL` valida y puede registrar warning funcional.
  - UI muestra mensaje de negocio.
  - No hay rollback por excepcion tecnica.
- Excepcion tecnica:
  - `DAL`/`BLL` propaga `ExcepcionTecnica`.
  - UI o `Program` la registra en bitacora tecnica.
  - La transaccion se revierte.

## Persistencia obligatoria

- `Usuarios.Activo` resuelve la baja logica.
- `Bitacora` persiste trazas cronologicas.
- `UsuarioHistorial` y `UsuarioHistorialPermiso` persisten snapshots restaurables.
