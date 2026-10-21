# Design

## Context

Ver proposal.md (Why). Estado actual que condiciona el diseno:

- La matriz de `.csproj` se cumple y la validan `verify-architecture-dependencies.ps1` y `verify-composition-root.ps1`, que corren desde `Directory.Build.targets`. Ninguna mira firmas de metodos ni el contenido de ABSTRACCIONES.
- Las entidades BE ya modelan sus relaciones con objetos (`Movimiento.Categoria`, `Comercio.RubroComercio`, `ComercioProducto.Producto`). Las excepciones son `ConsultaChatbot.UsuarioId`/`ChatbotFinancieroId`, `PerfilInversor.UsuarioId`/`PreguntaId`, `DetalleGeneracionNotificacion.CorridaId`/`UsuarioId`/`NotificacionId`, `ActualizacionTasaDetalle.EjecucionId`, `ProductoEmbedding.ProductoId`, `TasaConversion.ProtegidaPorUsuarioId` y `ReporteMovimientosPdf.UsuarioId`.
- `Sesion` guarda la instancia `BE.Usuario` que `UsuarioBLL` le pasa en el login (`_sesion.IniciarSesion(usuarioBD)`), tipada como `IUsuarioAutenticado`. Existe un segundo `IniciarSesion(int, string)` que arma un `UsuarioAutenticadoSesion`.
- SERVICIOS solo ve ABSTRACCIONES, y BE solo ve ABSTRACCIONES. Un tipo que necesiten los dos solo puede vivir en ABSTRACCIONES.
- `CRONS` (consola que dispara el Programador de tareas) referencia ABSTRACCIONES, BE, BLL y SERVICIOS, igual que la UI. La guardia de la matriz ya la contempla, pero la documentacion de capas no.
- Hay un precedente del patron de DTOs: `IUsuarioSnapshot` en ABSTRACCIONES con `UsuarioHistorialSnapshot` en SERVICIOS.

## Goals / Non-Goals

**Goals:**

- Que las firmas no privadas de BLL, REPOSITORIO, DAL, SERVICIOS y ABSTRACCIONES reciban entidades o interfaces, no IDs.
- Que ABSTRACCIONES contenga solo `interface` y `enum`.
- Que el build impida volver atras.

**Non-Goals:**

- Cambiar el esquema de base de datos o el comportamiento visible de la aplicacion.
- Cambiar la matriz de dependencias entre proyectos.
- Revisar metodos `private`: la regla es sobre contratos entre clases, no sobre detalles internos.

## Decisions

### D1. Regla de contratos: entidades de punta a punta

Toda firma no privada recibe la entidad (`Actualizar(Categoria categoria)`, `Asignar(ComercioProducto asignacion)`, `GuardarPermisosUsuario(Usuario usuario, IEnumerable<IPermiso> permisos)`), y los metodos de alta devuelven la entidad creada con su `Id` cargado, no el `int`. La UI conserva la entidad en el control (por ejemplo en `DataBoundItem`, en el `Tag` o como campo de las clases `Fila*`) y la pasa tal cual. DAL es el unico lugar donde `entidad.Id` se convierte en un `SqlParameter`.

**Unica excepcion: `ObtenerPorId(int id)` en REPOSITORIO y DAL.** Sirve para rehidratar una entidad cuando el `Id` ya viene de la base o de la sesion, y para que BLL vuelva a leer una entidad recibida antes de validarla (por ejemplo, que la categoria sea visible para el usuario). La guardia la permite solo con ese nombre exacto.

- Alternativa descartada: permitir IDs en REPOSITORIO y DAL y exigir entidades solo en BLL. Deja la mitad del problema (unas 400 firmas) y la regla se vuelve "depende de la capa".
- Alternativa descartada: prohibir tambien `ObtenerPorId`. Obliga a inventar firmas como `Obtener(Categoria referencia)`, que es un ID disfrazado.

### D2. ABSTRACCIONES: solo interfaces y enums, sin comportamiento

- Los enums se quedan: no tienen comportamiento y BE y SERVICIOS los comparten (`TipoMovimiento`), sin otro proyecto comun.
- `ProveedorTasasException` y `BusquedaUbicacionException` pasan a SERVICIOS. Las lanza SERVICIOS y las capturan BLL y UI, que ya referencian SERVICIOS.
- `DigitoVerificadorAttribute` pasa a BE. `SERVICIOS/IntegridadService` lo descubre por reflexion y no puede ver BE, asi que el atributo implementa `IDigitoVerificador` (ABSTRACCIONES, solo `int Orden { get; }`) y el servicio lo busca por esa interfaz. El calculo del DVH no cambia.
- Cada DTO pasa a ser una interfaz de solo lectura `I*` en ABSTRACCIONES (`ISolicitudAsistente`, `IContextoAsistente`, `IReferenciaAsistente`, `IIntercambioChatbot`, `IHerramientaAsistente`, `ILlamadaHerramientaAsistente`, `IDatoMercado`, `ITicketEscaneado`, `ITasasCambioObtenidas`, `IConsultaUbicacion`, `IResultadoBusquedaUbicacion`, `ILimitesGeograficos`). La implementacion va a SERVICIOS, siguiendo el precedente `IUsuarioSnapshot`/`UsuarioHistorialSnapshot`. `ContextoAsistente.Vacio` queda en la clase concreta.
- Alternativa descartada: llevar los DTOs a BE. BE es el modelo de dominio y SERVICIOS no lo puede ver, asi que igual harian falta las interfaces.

### D3. Sesion: el usuario, no su ID

- Se quitan `ISesion.GetUsuarioActualId()` y `Sesion.IniciarSesion(int, string)`. Asi la sesion siempre guarda el `BE.Usuario` del login.
- BLL tiene un unico punto de acceso interno que devuelve el `Usuario` en sesion (un metodo de extension `internal` sobre `ISesion`). La conversion de `IUsuarioAutenticado` a `Usuario` vive solo ahi y falla con `ExcepcionNegocio` si no hay sesion.
- Alternativa descartada: releer el usuario de la base en cada operacion. Agrega una consulta por llamada sin ningun beneficio.

### D4. Limite de uso del asistente en SERVICIOS

`LimiteUsoAsistente` pasa a SERVICIOS detras de `ILimiteUsoAsistente` (ABSTRACCIONES) con `bool IntentarConsumir(IUsuarioAutenticado usuario)`. Lee `AsistenteMaxConsultasPorMinuto` con el mismo patron de los servicios Gemini: un constructor que recibe la funcion de configuracion y otro por defecto que usa `ConfigurationManager`. El diccionario interno sigue indexado por `Id`, porque es un detalle privado y no un contrato. El CompositionRoot deja de leer configuracion.

- Alternativa descartada: dejarlo en BLL y que lea `ConfigurationManager`. Mete un detalle tecnico (la cuota de Gemini) en la capa de negocio y rompe la convencion de que solo SERVICIOS y DAL leen configuracion.

### D5. Guardia `verify-contratos-capas.ps1` con trinquete

Un script nuevo en `.agents/scripts/`, conectado en `Directory.Build.targets` igual que los otros dos y con el mismo recorrido de archivos (sin `bin`, `obj`, `.git` ni `tests`). Reglas:

1. ABSTRACCIONES: ninguna declaracion `class`, `struct` ni `record`.
2. BLL, REPOSITORIO, DAL, SERVICIOS y ABSTRACCIONES: ningun parametro de un metodo o constructor no privado de tipo `int`, `long`, `Guid` (o nullable) llamado `id` o `*Id`, ni una coleccion de enteros llamada `*Ids`. Se permite `ObtenerPorId(int id)` en REPOSITORIO y DAL.
3. BLL: ningun inicializador `new X { Id = ... }`.
4. BE: ninguna propiedad publica `int xxxId` (la propiedad `Id` propia esta permitida).
5. `ConfigurationManager` solo en SERVICIOS y DAL.

Es un analisis por expresiones regulares: lo suficiente para estas formas, que son regulares en el repo. Para no frenar el build mientras dura la migracion, las violaciones actuales se congelan en `.agents/scripts/contratos-capas.baseline.txt` (una linea por archivo y firma). La guardia falla ante una violacion que no esta en la linea base y tambien ante una entrada de la linea base que ya no existe, para que cada modulo migrado tenga que borrar sus lineas. Al final la linea base queda vacia y se elimina.

- Alternativa descartada: activar la guardia recien al final. Durante la migracion, los cambios en curso podrian sumar firmas con IDs sin que nadie se entere.
- Alternativa descartada: un analizador Roslyn. Exige un paquete NuGet en el build productivo, y eso esta prohibido.

### D6. Migracion por rebanadas verticales de modulo

Cada grupo de tareas migra un modulo completo (DAL, REPOSITORIO, BLL, UI y tests) y cierra con la solucion compilando y la suite en verde. Cambiar firmas sin cambiar comportamiento no es un ciclo red-green: los tests existentes se adaptan a las firmas nuevas y tienen que seguir pasando con los mismos contadores. `strict-tdd` aplica solo si en el camino cambia alguna regla de negocio.

### D7. Patron fijado por el piloto (Categorias)

Cada modulo sigue la forma de `CategoriaBLL`, `ICategoriaRepositorio` y `FormGestionCategorias`:

- **Lectura del usuario en sesion**: si todos los que llaman pasan el usuario en sesion, el metodo del BLL no recibe parametro (`ObtenerVisiblesParaUsuarioActual()`) y lo obtiene con `_sesion.ObtenerUsuarioActual()`. Si hace falta otro usuario (administracion, CRONS), recibe `Usuario`.
- **Alta**: `X Crear(X entidad)`. La UI arma la entidad nueva con sus datos; el BLL valida, completa lo que no viene de la UI (el `Usuario` en sesion, nunca un cascaron) y devuelve la misma instancia con su `Id` cargado.
- **Modificacion**: `bool Actualizar(X entidad)`. La UI escribe los valores nuevos en la entidad que ya tiene. El BLL relee la persistida con `ObtenerPorId(entidad.Id)`, valida la propiedad y las reglas sobre la persistida (nunca confia en el `Usuario` ni en otros campos no editables recibidos), copia solo los campos editables y guarda la persistida.
- **Baja**: `bool Eliminar(X entidad)`. El BLL relee, valida sobre la persistida y elimina la persistida.
- **Entidad nula**: el BLL lanza la `ExcepcionNegocio` que ya usaba el modulo para ese caso (dato requerido en el alta y la modificacion, no encontrado en la baja), nunca una `ArgumentNullException` que llegue a la UI.
- **REPOSITORIO y DAL**: reciben entidades (`Eliminar(X)`, filtros con `Usuario`). Un parametro "opcional" que antes era `int?` pasa a ser la entidad nullable con el mismo significado (`ExisteNombreEnScope(nombre, Usuario usuario, X excluida)`, `null` = alcance global o sin exclusion). `InsertarYObtenerId(X)` y `Actualizar(X)` no cambian.
- **UI**: la clase `Fila*` de la grilla guarda la entidad en una propiedad que no se muestra (las grillas usan `AutoGenerateColumns = false`) y la pasa tal cual. Si el BLL rechaza una modificacion, la entidad en memoria puede quedar con el valor rechazado: la UI no la reutiliza para otra operacion sin recargar la lista. Si otro control muestra el mismo objeto (por ejemplo el combo de rubros en `FormGestionComercios`), la UI restaura los valores anteriores al capturar el error.
- **Clave compuesta**: una relacion sin `Id` propio se busca por sus entidades (`ObtenerPorComercioYProducto(Comercio, Producto)`). No es un ID disfrazado: recibe las entidades que forman la clave.
- **Filtros de movimientos**: `FiltroMovimiento` vive en BE y conserva `Categoria` como referencia opcional (`null` = todas), en lugar de transportar `CategoriaId` dentro del filtro. No es consumido por SERVICIOS; moverlo a BE permite tipar el criterio del dominio sin agregar la dependencia prohibida `SERVICIOS -> BE`. DAL convierte esa referencia en el parametro SQL nullable. `ReporteMovimientosPdf` conserva el `Usuario` de la consulta y Dashboard recibe la `TarjetaDashboard` que conserva el control, releyendo la persistida antes de validar su propietario.
- **Tests**: fuera del alcance de la guardia. Pueden armar entidades con solo el `Id` para preparar datos.

### D8. Aplicacion del patron al grupo 6

- `CuentaFinanciera` transporta la entidad financiera y su coleccion `MediosPagoEntidad`. El BLL valida el catalogo persistido, elimina repetidos y conserva solo sus instancias validas. Al editar relee la cuenta y conserva su usuario y sus credenciales cifradas; al crear devuelve la misma instancia y fija el usuario de sesion y las credenciales nulas.
- `AgregarItem(ItemLista)` recibe el item con su lista, relee y valida la lista persistida y devuelve la misma instancia. `ActualizarCantidad(ItemLista, int)` y `MarcarComprado(ItemLista, bool)` reciben la entidad y el valor de la operacion; no copian descripcion, lista ni propietario desde la entidad recibida.
- Las bajas de zonas y recordatorios reciben las entidades. La creacion de zonas conserva los argumentos de coordenadas nullable que distinguen un punto no elegido de las coordenadas validas `(0, 0)`, y utiliza directamente el usuario en sesion.
- Adaptacion de consumidores aun no migrados: `GeneracionNotificacionesBLL.TienePendientes` obtiene el usuario con `unidad.Usuarios.ObtenerPorId` y pasa la lista a la consulta de items. `RecomendacionComprasBLL` recibe `IUsuarioRepositorio` desde `CompositionRoot` para resolver el usuario capturado antes de consultar zonas; su contrato de entrada con IDs sigue asignado al grupo 11. No se agregan cascarones ni se vuelve a leer la sesion tras una espera asincrona.

### D9. Aplicacion del patron al grupo 7

- `TasaConversion` conserva `ProtegidaPorUsuario` como referencia opcional; DAL mapea la FK y REPOSITORIO hidrata el usuario con el mismo acceso transaccional. La edicion y liberacion releen la tasa persistida y mantienen las reglas de cambio real, proteccion manual e idempotencia.
- Los contratos de consulta por par, actualizacion automatica, proteccion, liberacion y baja reciben entidades. `FormGestionMonedas` conserva la tasa en su fila y pasa las monedas seleccionadas.
- `ActualizacionTasaDetalle.Ejecucion` referencia la ejecucion guardada. Al consultar detalles, DAL conserva la instancia de ejecucion recibida desde la fila del historial; REPOSITORIO hidrata las monedas. No se agrega una consulta de ejecucion por cada detalle.

### D10. Aplicacion del patron al grupo 8

- `Promocion` conserva colecciones de `Comercio`, `RubroComercio`, `EntidadFinanciera`, `MedioPago`, `Producto` y `CategoriaProducto`. Dias y segmentos mantienen sus tipos enum y string. La normalizacion deduplica las referencias por su identidad persistida, no por igualdad de instancia, y compara las inclusiones/exclusiones por esa misma identidad.
- El alta devuelve la instancia recibida y normalizada. La edicion relee la promocion y copia sus campos editables y colecciones; la baja relee, comprueba el uso en notificaciones y limpia las tablas puente antes de eliminar, dentro de la misma unidad de trabajo.
- Los repositorios de alcance devuelven entidades hidratadas con el acceso transaccional compartido; `PromocionRepositorio.Hidratar` centraliza la composicion de filtros y tambien lo utiliza `RecomendacionComprasRepositorio`, evitando duplicar el mapeo del agregado.
- Las opciones de las listas del editor conservan las entidades del catalogo y la fila conserva la promocion. Las consultas por comercio y las reglas de pago/producto reciben esas entidades. `AlcancePromocion.AplicaAProducto(Promocion, Producto)` obtiene la categoria desde el producto y mantiene la prioridad de las exclusiones.

### D11. Asistente, perfil inversor y cierre estricto

- El asistente y los calculos de recomendaciones reciben el `Usuario` capturado al iniciar la operacion; las consultas persistidas conservan referencias a usuario y chatbot.
- `PerfilInversor.Usuario` y `RespuestaPerfilInversor.Pregunta` sustituyen las FKs del modelo de objetos. El catalogo fijo de preguntas se expone desde BLL y el formulario conserva esas preguntas al construir las respuestas; DAL sigue mapeando las columnas fisicas existentes.
- La continuacion de onboarding conserva un `Usuario` releido dentro de la transaccion de alta, independiente de la instancia de entrada. Modificar el Id de la entrada despues del alta no altera al propietario de la continuacion.
- Eliminada la linea base vacia y todo su soporte en el guard: cualquier violacion detectada falla el build. La actualizacion de G07 y revision de G08 queda registrada en la tarea 2.13 de `actualizar-diagramas-tp`; no hay migracion de esquema SQL.

## Risks / Trade-offs

- [CRONS llama a los mismos BLL que la UI y se rompe con las firmas nuevas] → Cada grupo adapta tambien los llamados desde `CRONS/Tareas/` y verifica que `CRONS.exe tasas` y `CRONS.exe notificaciones` siguen compilando dentro de `TpIDS.sln`.
- [Los changes en curso tocan modulos afectados y van a chocar con las firmas nuevas] → Migrar primero los modulos que esos changes no tocan; el trinquete evita que sumen violaciones nuevas mientras tanto.
- [Una entidad recibida desde la UI puede estar desactualizada o manipulada] → BLL vuelve a leerla con `ObtenerPorId(entidad.Id)` cuando la regla de negocio depende de su estado actual, igual que hoy valida a partir del ID.
- [La guardia por expresiones regulares puede dar falsos positivos o negativos] → Las reglas apuntan a formas concretas y la revision del piloto ajusta los patrones antes de migrar el resto.
- [Volumen alto y riesgo de regresiones en la UI] → Rebanadas chicas, suite de integracion por modulo y la suite UI FlaUI completa al cierre, con confirmacion del usuario.
- [La conversion de `IUsuarioAutenticado` a `Usuario` depende de que el login siempre guarde un `Usuario`] → Se quita la sobrecarga que armaba otro tipo y la conversion vive en un solo lugar, con un test que la cubre.

## Migration Plan

1. Reglas, documentacion y guardia con la linea base.
2. ABSTRACCIONES, sesion y limite de uso.
3. Piloto Categorias para fijar el patron y ajustar la guardia.
4. El resto de los modulos, de a uno por grupo.
5. Linea base vacia y eliminada, suite completa en verde.

No hay datos que migrar. La vuelta atras de cualquier etapa es descartar sus cambios de codigo, porque la base de datos no cambia.
