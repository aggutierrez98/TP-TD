# Tasks

Cierre de cada grupo a partir del 3: los llamados desde UI y `CRONS/Tareas/` estan adaptados, `TpIDS.sln` compila (incluidas las tres guardias), `tests/tools/run-tests.ps1 -IncludeIntegration` termina con exit code 0 y los contadores del TRX no bajan respecto del grupo anterior, y las lineas del modulo ya no estan en `contratos-capas.baseline.txt`.

## 1. Reglas y guardia

- [x] 1.1 Agregar a `.agents/02-architecture-7-layers.md` la seccion "Contratos entre clases" (entidades y no IDs, alta que devuelve la entidad, excepcion `ObtenerPorId`, UI que conserva la entidad en el control) y reescribir las reglas 7 y 8 como "ABSTRACCIONES contiene solo interfaces y enums, sin comportamiento". Agregar `CRONS` a la matriz como segundo punto de entrada (`CRONS -> ABSTRACCIONES, BLL, BE, SERVICIOS`). Verificar que coincida con `verify-architecture-dependencies.ps1` y con `references/dependency-matrix.md`
- [x] 1.2 Sumar la regla de contratos por entidades a "Reglas transversales" de `AGENTS.md` y agregar `CRONS` a la matriz que esa seccion repite y los patrones prohibidos nuevos a `.agents/skills/architecture-dependency-guard/SKILL.md`. Verificar que los dos remitan a `.agents/02-architecture-7-layers.md`
- [x] 1.3 Escribir `.agents/scripts/verify-contratos-capas.ps1` con las cinco reglas de D5 y el trinquete (falla ante una violacion nueva y ante una entrada de linea base obsoleta). Verificarlo agregando a mano una firma con `int categoriaId` en un BLL (falla) y quitandola (pasa)
- [x] 1.4 Generar `.agents/scripts/contratos-capas.baseline.txt` con las violaciones actuales y conectar la guardia en `Directory.Build.targets` junto a las otras dos. Verificar que `TpIDS.sln` compila y que la guardia aparece en la salida de MSBuild

## 2. ABSTRACCIONES, sesion y limite de uso

- [x] 2.1 Mover `ProveedorTasasException` y `BusquedaUbicacionException` a SERVICIOS y `DigitoVerificadorAttribute` a BE, y ajustar los `using`. Verificar con la compilacion
- [x] 2.2 Convertir los 12 DTOs de ABSTRACCIONES en interfaces `I*` de solo lectura con su implementacion en SERVICIOS (D2) y adaptar BLL, SERVICIOS, UI y tests. Verificar que ABSTRACCIONES solo declara `interface` y `enum` y que la regla 1 de la guardia ya no tiene entradas en la linea base
- [x] 2.3 Quitar `ISesion.GetUsuarioActualId()` y `Sesion.IniciarSesion(int, string)`, y agregar en BLL el punto de acceso interno que devuelve el `Usuario` en sesion (D3), con un test en `BLL.Tests` para el caso con sesion y el caso sin sesion. Reemplazar los 60 usos. Verificar con la compilacion y con `rg GetUsuarioActualId` sin resultados
- [x] 2.4 Mover `LimiteUsoAsistente` a SERVICIOS detras de `ILimiteUsoAsistente` (D4), llevar `LimiteUsoAsistenteTests` a `SERVICIOS.Tests` y quitar `ConfigurationManager` de `BLL/CompositionRoot.cs`. Verificar que esos tests pasan y que la regla 5 de la guardia no tiene entradas
- [x] 2.5 Cerrar el grupo con la compilacion y la suite en verde

## 3. Piloto: Categorias

- [x] 3.1 Migrar `CategoriaDAL`, `ICategoriaRepositorio`/`CategoriaRepositorio`, `CategoriaBLL` y `FormGestionCategorias` a entidades (`Crear` devuelve `Categoria`, `Actualizar(Categoria)`, `Eliminar(Categoria)`, visibles para el usuario en sesion) y adaptar sus tests. Verificar con el cierre de grupo
- [x] 3.2 Revisar la guardia con lo aprendido en el piloto (falsos positivos y negativos) y ajustar D1/D5 en `design.md` si cambia algo. Verificar que la guardia sigue rechazando el caso de 1.3

## 4. Catalogos de comercio y productos

- [x] 4.1 Migrar Moneda y MedioPago. Verificar con el cierre de grupo
- [x] 4.2 Migrar RubroComercio, Comercio y CategoriaProducto. Verificar con el cierre de grupo
- [x] 4.3 Migrar Producto, ProductoEmbedding (`ProductoId` pasa a `Producto`) y ComercioProducto (`Asignar`, `ActualizarPrecio` y `Quitar` reciben `ComercioProducto`). Verificar con el cierre de grupo
- [x] 4.4 Migrar EntidadFinanciera y MedioPagoEntidad. Verificar con el cierre de grupo

## 5. Movimientos y categorizacion

- [x] 5.1 Migrar Movimiento (incluido `ReporteMovimientosPdf`, que recibe el `Usuario`) y FormMovimientos. Verificar con el cierre de grupo
- [x] 5.2 Migrar ReglaCategorizacion y ComercioReglaCategorizacion (las listas de `comercioIds` pasan a `IEnumerable<Comercio>`). Verificar con el cierre de grupo
- [x] 5.3 Migrar Dashboard y TarjetaDashboard. Verificar con el cierre de grupo

## 6. Cuentas, listas, zonas y recordatorios

- [x] 6.1 Migrar CuentaFinanciera y CuentaFinancieraMedioPagoEntidad. Verificar con el cierre de grupo
- [x] 6.2 Migrar ListaDeCompras e ItemLista. Verificar con el cierre de grupo
- [x] 6.3 Migrar ZonaUsuario, RecordatorioCompra y RecordatorioInversion. Verificar con el cierre de grupo

## 7. Tasas de conversion

- [x] 7.1 Migrar TasaConversion (`ProtegidaPorUsuarioId` pasa a `Usuario`) y FormGestionMonedas. Verificar con el cierre de grupo
- [x] 7.2 Migrar ActualizacionTasas, EjecucionActualizacionTasas y ActualizacionTasaDetalle (`EjecucionId` pasa a referencia). Verificar con el cierre de grupo

## 8. Promociones

- [x] 8.1 Migrar Promocion y sus tablas de alcance (PromocionComercio, PromocionRubroComercio, PromocionProducto, PromocionCategoriaProducto, PromocionEntidadFinanciera, PromocionMedioPago, PromocionSegmento, PromocionDiaSemana): la promocion se guarda con sus colecciones. Verificar con el cierre de grupo
- [x] 8.2 Migrar `AlcancePromocion.AplicaAProducto` para que reciba `Promocion` y `Producto`. Verificar con los tests de alcance de promociones en verde

## 9. Notificaciones

- [x] 9.1 Migrar Notificacion (`LeerPersonal`, `MarcarLeida`, `Eliminar` y `CrearManual` reciben entidades) y sus formularios. Verificar con el cierre de grupo
- [x] 9.2 Migrar GeneracionNotificaciones y DetalleGeneracionNotificacion (`CorridaId`, `UsuarioId` y `NotificacionId` pasan a referencias). Verificar con el cierre de grupo

## 10. Usuarios, permisos, idiomas e historial

- [x] 10.1 Migrar Usuario, UsuarioEstado, UsuarioHistorial, `IUsuarioSnapshot`/`UsuarioHistorialSnapshot` e `IServicioHistorialEstados`/`HistorialEstadosService` (`RestaurarVersion` y relacionados reciben el snapshot). Verificar con el cierre de grupo
- [x] 10.2 Migrar Permiso, UsuarioPermiso, `IPermiso.Contiene` (recibe `IPermiso`), PermisoSimple y PermisoGrupo. Verificar con el cierre de grupo
- [x] 10.3 Migrar Idioma, Traduccion, UsuarioPreferencia e IdiomaCatalogoService (`GuardarPreferencia(Idioma)` usa el usuario en sesion). Verificar con el cierre de grupo
- [x] 10.4 Migrar `ContratosInternosBLL` y `ExcepcionIntegridad` (recibe la entidad `IIntegrable`). Verificar con el cierre de grupo

## 11. Asistente financiero

- [x] 11.1 Migrar AsistenteFinanciero, ConsultaChatbot (`UsuarioId` y `ChatbotFinancieroId` pasan a referencias) y ContinuacionPerfilInversor. Verificar con el cierre de grupo
- [x] 11.2 Migrar RecomendacionCompras, RecomendacionInversiones y PerfilInversor (`UsuarioId` y `PreguntaId` pasan a referencias). Verificar con el cierre de grupo

## 12. Cierre

- [x] 12.1 Confirmar que `contratos-capas.baseline.txt` quedo vacio, eliminarlo y quitar el soporte de linea base del script. Verificar que `TpIDS.sln` compila con la guardia estricta
- [x] 12.2 Avisar al usuario que la suite UI FlaUI toma el escritorio unos 5 minutos y, con su confirmacion, correr la suite completa. Verificar exit code 0 y reportar los contadores reales del TRX. Corrida completa autorizada del 2026-10-20: exit 0; 1279 unitarios y 265 de integracion aprobados; UI 140 descubiertos, 139 ejecutados/aprobados y 1 inconcluso (`FIN_Productos_SinCategorias_OrientaYDeshabilitaAlta`, requiere base recien regenerada sin categorias). Tiempo total cronometrado: 27 min 54,750 s; tramo UI segun TRX: 27 min 22,099 s. Build productivo posterior: exit 0.
- [x] 12.3 Agregar al `tasks.md` de `actualizar-diagramas-tp` la actualizacion de las firmas en los diagramas de clases G07/G08. Verificar que la tarea quedo registrada ahi

Validacion complementaria autorizada del 2026-10-20: `run-tests.ps1 -UiFilter FIN_Productos_SinCategorias_OrientaYDeshabilitaAlta`, base regenerada, exit 0; UI 1 ejecutado/aprobado, 0 fallidos e inconclusos. Tiempo total 36,372 s; duracion del caso 10,828 s segun TRX. Completa la verificacion funcional del caso inconcluso de 12.2 en una corrida separada, sin afirmar independencia en la corrida completa. El aislamiento y el gate TRX estricto se planifican para el futuro en `aislar-casos-suite-ui`.
