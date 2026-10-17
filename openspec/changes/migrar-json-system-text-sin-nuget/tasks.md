# Tasks

## 1. Restriccion e inventario

- [x] 1.1 Agregar a AGENTS.md la regla inviolable de compilacion y ejecucion productiva en laboratorio sin instalacion/restauracion NuGet ni cache previo, con excepcion explicita para tests de desarrollo; verificar que el texto exija compilar desde fuente y no solo ejecutar binarios.
- [x] 1.2 Corregir tests/README.md para distinguir la restriccion requerida del estado actual, incluyendo las cuatro referencias NuGet de UI y el nativo SkiaSharp; verificar el inventario contra los csproj y mantener los tests fuera de TpIDS.sln.

## 2. Referencia JSON reproducible

- [x] 2.1 Identificar los ensamblados netfx de System.Text.Json disponibles en MSBuild local y registrar en design.md version, fuente y dependencias; verificar que referencien mscorlib de .NET Framework, sin exigir acceso al proyecto del laboratorio ni redistribuir DLL.
- [x] 2.2 Con la referencia resuelta, agregarla explicitamente a SERVICIOS y al proyecto de tests segun su alcance, sin nuevo PackageReference productivo ni HintPath al cache; verificar que JsonSerializer y JsonDocument compilen para net472 con C# 7.3 y registrar el comando reproducible en design.md.

## 3. Catalogos de idiomas

- [x] 3.1 Ejecutar baseline unitario en verde y verificar el contrato de catalogos string/string con los tests existentes (acentos, UTF-8 sin BOM, null, catalogo vacio y JsonInvalidFormat); documentar el parsing estandar de System.Text.Json en design.md. Migracion de motor sobre el contrato existente, sin test nuevo de comportamiento.
- [x] 3.2 Migrar IdiomaCatalogoService y sus tests a System.Text.Json conservando errores y bytes exportados; verificar el conjunto de IdiomaCatalogoServiceTests en verde, incluido el round-trip y la causa JsonException, y documentar en design.md las opciones/conversiones necesarias.

## 4. Gemini y tests

- [x] 4.1 Migrar GeminiChatClient y sus tests preservando historial, roles, systemInstruction y extraccion del primer texto; verificar respuestas validas, candidates/parts ausentes o vacios y JSON interno de clasificacion con los tests existentes y red-green ante diferencias observables.
- [x] 4.2 Migrar GeminiEscaneoTicketService y sus fixtures/assertions a System.Text.Json; verificar requests estructurados, MIME/base64, monto decimal nullable, fechas invalidas, campos ausentes, acentos y errores con GeminiEscaneoTicketServiceTests en verde.
- [x] 4.3 Migrar GeminiAsistenteFinancieroService y sus tests preservando el schema de clasificacion y fallback General; verificar Ahorro/Inversiones, clasificacion desconocida/invalida y delegacion en verde. Actualizar comentarios JsonProperty y registrar decisiones finales de parsing en design.md.

## 5. Limpieza e integracion

- [x] 5.4 Corregir la duplicacion de assets JSON de ejecucion en ReferenceCopyLocalPaths, conservando las referencias directas compartidas; verificar que Rebuild y Build incremental Debug copien una sola fuente por DLL, que las identidades/hashes coincidan con MSBuild y que un smoke con TpIDS.exe.config inicialice IdiomaCatalogoService y haga round-trip JSON. Cerrar con suite unit/integracion y build productivo Debug. Evidencia: MSBuild TpIDS.sln /t:Rebuild y /t:Build /p:Configuration=Debug /m exit 0, guards OK; log json-debug-after.log confirma copia unica de Json desde MSBuild; hashes de las nueve DLL disponibles del grupo coinciden despues del build incremental; smoke de AppDomain con TpIDS.exe.config inicializa el servicio y completa round-trip; TpIDS.exe Debug abre FormLogin con titulo Menu de ingreso y se cierra normalmente. Runner tests/tools/run-tests.ps1 -IncludeIntegration exit 0; TRX unitarios 490/490 e integracion 161/161, sin fallos. Persisten warnings previos ValueTuple y redireccion Resources.Extensions; suite UI completa no ejecutada por falta de confirmacion especifica, laboratorio sin verificar.

- [x] 5.1 Eliminar PackageReference de Newtonsoft.Json en SERVICIOS y SERVICIOS.Tests; verificar mediante busqueda que no queden using, JsonConvert, atributos JsonProperty ni JObject/JArray de Newtonsoft en codigo/tests activos, conservando referencias historicas de archivos archivados.
- [ ] 5.2 Registrar en un cambio OpenSpec propio el trabajo restante de portabilidad de UI cuando se confirme la distribucion permitida; verificar que enumere GMap, Krypton, LiveCharts y dependencias nativas/transitivas, y que incluya compilacion limpia integral sin NuGet como criterio de cierre. Incluye el seguimiento transversal retirado de mis-notificaciones-usuario con confirmación del usuario al cerrar ese módulo; verificar la solución productiva completa, incluida la bandeja y el badge, sin restore ni caché NuGet en un entorno equivalente al laboratorio. Los builds locales del módulo no acreditan esa portabilidad.
- [x] 5.3 Ejecutar tests/tools/run-tests.ps1 -IncludeIntegration y reportar contadores reales del TRX; compilar despues TpIDS.sln con MSBuild y guards de arquitectura/composicion. Verificar aparte la referencia JSON sin restore/cache, informar las limitaciones de entorno y dependencias productivas restantes, y validar este cambio con openspec validate migrar-json-system-text-sin-nuget --strict. Evidencia: runner -IncludeIntegration -Configuration Release exit 0; TRX unitarios 480/480 e integracion 159/159, sin fallos; MSBuild TpIDS.sln /t:Build /p:Configuration=Release /m exit 0, guards OK; SERVICIOS /t:Rebuild sin restore y con cache vacio exit 0. Release evita DLL bloqueadas por la app abierta en Debug. Warnings MSB3243 de dependencias en tests y MSB3277 de ValueTuple en UI; laboratorio y suite UI no verificados.
