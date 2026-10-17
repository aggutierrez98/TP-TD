# Design

## Context

Ver `proposal.md` para la motivacion. `AccesoDatosSqlServer.EnsureConnectionOpen()` recorre `TpIDS` y `TpIDS-UAI` e intenta abrir ambas ante un fallo SQL. `BackupDAL.ResolverConnectionString()` recorre los mismos nombres pero solo verifica que tengan contenido. `UI/App.config` define SQL Express y la instancia predeterminada; el arranque carga idioma e integridad antes del login. La configuracion de integracion tambien contiene ambas entradas y el stager UI reescribe las entradas existentes sin exigir dos.

## Goals / Non-Goals

**Goals:** preservar transacciones, manejo de errores y composicion actuales al hacer determinista la seleccion del destino en ambos consumidores DAL.

**Non-Goals:** introducir parametros de linea de comandos, deteccion de entorno, nuevos proveedores, pantallas de configuracion o modificaciones del flujo de integridad.

## Decisions

1. **Conservar el nombre `TpIDS`.** Es la entrada principal actual y la usada directamente por las pruebas de backup. La referencia del usuario a la URL se interpreta como la cadena SQL completa, no como una URL HTTP. Se conserva su valor actual; el servidor que se quiera usar se indica en `Data Source`.
2. **Leer exclusivamente `ConfigurationManager.ConnectionStrings["TpIDS"]` en ambos consumidores DAL.** Eliminar arreglos de candidatos y bucles; verificar ausencia o contenido vacio antes de abrir. Mantener `InvalidOperationException` para configuracion faltante y `ExcepcionTecnica("Error.Technical.ConnectionUnavailable", ex)` para fallo SQL de acceso normal, liberando la conexion fallida. No se necesita un servicio transversal para dos lecturas simples ni cambiar referencias entre capas.
3. **Mantener el arranque existente.** No agregar un sondeo previo: la carga de idioma/integridad usa la conexion unica y el manejo de excepciones actual informa el fallo e interrumpe el arranque.
4. **Verificacion de regresion con configuraciones aisladas.** En la suite de integracion, usar un AppDomain de .NET Framework con archivo de configuracion propio para no cambiar el cache global de ConfigurationManager ni la base compartida. Probar principal inaccesible con alternativa valida, principal ausente/vacia con alternativa valida y principal valida. Para observar el fallo antiguo, ejecutar una lectura inocua mediante AccesoDatosSqlServer con principal ausente y alternativa de pruebas valida. Verificar tambien la resolucion de backup sin ejecutar RESTORE sobre la base compartida. Los casos de conexion inaccesible usan un timeout corto definido solo en el config de prueba.
5. **Configuracion unica distribuida.** Eliminar `TpIDS-UAI` de `UI/App.config` y del config de integracion. Los tests aislados pueden incluirla expresamente como entrada ignorada de regresion. El stager UI ya soporta una sola entrada.

## Risks / Trade-offs

- Una instalacion que dependia del fallback dejara de conectar → configurar `TpIDS` con su servidor real antes de ejecutar.
- Contaminacion de la configuracion o datos de pruebas → aislar configuraciones por AppDomain y usar consultas de solo lectura sobre `TpCatedraIDS_Test`.
- SQL Server no disponible para la verificacion → registrar la limitacion y ejecutar al menos los unit tests segun las reglas del repositorio; no declarar integracion verificada.

## Migration Plan

1. Aplicar el cambio con TDD y verificar build productivo y suite de integracion.
2. Configurar la unica cadena `TpIDS` para cada instalacion; `UI/App.config` es el origen y `TpIDS.exe.config` es la configuracion efectiva distribuida.
3. Ante una regresion, revertir conjuntamente los cambios de codigo y configuracion. No hay migracion de esquema ni datos.
