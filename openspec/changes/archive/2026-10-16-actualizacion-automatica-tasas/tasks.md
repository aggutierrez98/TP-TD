# Tasks

## 1. Esquema y entidades

- [x] 1.1 Agregar `EsManual`, `ProtegidaPorUsuarioId` y `FechaProteccion` a `TasaConversion` y crear `EjecucionActualizacionTasas` y `ActualizacionTasaDetalle` en `script_bd.sql`, aplicandolos por SQL directo en `TpCatedraIDS` y `TpCatedraIDS_Test`; verificar con una consulta que las columnas y tablas existen
- [x] 1.2 Crear las entidades en BE y compilar; verificar con la compilacion de `TpIDS.sln` (skill `winforms-build-verification`)

## 2. Persistencia (TDD, skill `strict-tdd`)

- [x] 2.1 Test de integracion en rojo y luego DAL/REPOSITORIO para: actualizar el valor sin tocar `EsManual`, marcar y liberar una tasa, insertar ejecuciones y detalles, consultar el ultimo exito y contar requests del mes; verificar con los tests de `TpIDS.IntegrationTests` en verde
- [x] 2.2 Exponer los repositorios en `IUnidadDeTrabajo`; verificar con la compilacion y los tests existentes de la unidad de trabajo

## 3. Proveedor de tasas

- [x] 3.1 Crear `IProveedorTasasCambio` en ABSTRACCIONES y `OpenExchangeRatesProveedor` en SERVICIOS con TLS 1.2, `async/await`, lectura de la clave y errores de dominio (red, respuesta invalida, rechazo, clave faltante), con tests unitarios en rojo primero sobre respuestas simuladas; verificar con `SERVICIOS.Tests` en verde
- [x] 3.2 Crear `secrets.config.example` con la clave vacia y la regla de `.gitignore` del archivo real del ejecutable; verificar con `git status` que el real no aparece

## 4. Logica de negocio (TDD)

- [x] 4.1 `ActualizacionTasasBLL`: una request por corrida, calculo de cruces, solo pares existentes, salto de manuales y de pares sin dato, tope de 60 minutos medido desde el inicio del ultimo exito, con chequeo dentro de la transaccion, registro de consumo y de fallas sin reintento; tests en `BLL.Tests` en rojo primero y luego en verde
- [x] 4.2 `TasaConversionBLL`: editar marca como manual y registra en bitacora, crear no marca, liberar con bitacora; tests en `BLL.Tests` en verde
- [x] 4.3 Dar de alta el BLL y el proveedor en `CompositionRoot` sin `new` fuera del root; verificar con la validacion de Composition Root del build (skill `composition-root-guard`)
- [x] 4.4 Consulta de historial para la pantalla (ultimas ejecuciones, detalle, requests del mes); verificar con tests de BLL en verde

## 5. Proceso programado

- [x] 5.1 Crear el proyecto `ActualizadorTasas` (consola, net472, sin NuGet) con su `App.config`, incluirlo en `TpIDS.sln` y registrarlo en la matriz de dependencias; verificar que el build valida la arquitectura (skill `architecture-dependency-guard`)
- [x] 5.2 Implementar `Main` que invoca la BLL con origen Programada, espera la tarea y devuelve un codigo de salida segun el resultado; verificar ejecutandolo contra `TpCatedraIDS_Test` con un proveedor simulado y revisando el historial
- [x] 5.3 Crear el script `schtasks` de registro y de baja con una tarea horaria; verificar registrandola, ejecutandola una vez con `schtasks /run` y eliminandola
- [x] 5.4 Documentar en `README.md` el registro de la tarea y la clave de la API; verificar siguiendo el texto en limpio

## 6. Pantalla y permisos

- [x] 6.1 Permiso nuevo en `script_bd.sql` y en las dos bases, asignado a Operador y administrador; verificar con una consulta de permisos por grupo
- [x] 6.2 Formulario "Actualizacion de Tasas" con grilla de ejecuciones, detalle, requests del mes y boton "Actualizar ahora" asincrono, con tema Krypton y nomenclatura de controles (skill `winforms-ui-control-naming`); verificar abriendolo en la app y con la compilacion
- [x] 6.3 Agregar a `FormGestionMonedas` la columna de estado manual y el boton de liberar; verificar a mano editando, liberando y viendo el estado en la grilla
- [x] 6.4 Opcion de menu en `FormPrincipal` condicionada al permiso; verificar con un usuario con y sin el permiso
- [x] 6.5 Textos traducibles en `translation-seeds/base.es.json` y `base.in.json`, sincronizados con `script_bd.sql` y a mano con las tablas `Palabra` y `Traduccion` de ambas bases (skill `translation-base-seed-sync`); verificar con el script de sincronizacion y la pantalla en los dos idiomas

## 7. Cierre

- [x] 7.1 Tests UI FlaUI de la pantalla nueva y del estado manual, y ajuste de los existentes si cambian los controles de `FormGestionMonedas`; avisar al usuario antes de correr la suite UI y verificar con su resultado
- [x] 7.2 Cerrar con `tests/tools/run-tests.ps1 -IncludeIntegration` en verde y la compilacion de `TpIDS.sln` en un entorno sin cache NuGet (skills `test-suite-verification` y `winforms-build-verification`)