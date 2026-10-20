# Design

## Context

Ver proposal.md para la motivacion. Los proyectos productivos son clasicos, apuntan a .NET Framework 4.7.2 y usan C# 7.3. Newtonsoft.Json 13.0.3 esta referenciado en SERVICIOS y SERVICIOS.Tests. Se usa JsonConvert, JsonProperty y JsonException; los tests tambien usan JObject/JArray.

IdiomaCatalogoService importa diccionarios y exporta UTF-8 sin BOM. Su test de exportacion exige acentos literales. Los servicios Gemini serializan objetos anonimos y deserializan DTO privados con nombres PascalCase enlazados a claves camelCase. Los campos opcionales del ticket y el fallback de clasificacion a General ya tienen tests.

## Goals / Non-Goals

**Goals:**
- Cambiar el motor JSON manteniendo contratos, claves de error y formato exportado.
- Mantener .NET Framework 4.7.2 y obtener una referencia productiva reproducible a System.Text.Json sin restauracion NuGet.
- Documentar la restriccion de laboratorio y el inventario real de paquetes.

**Non-Goals:**
- Investigar ahora la diferencia de disponibilidad entre las dos bibliotecas en el laboratorio: el usuario pidio hacerlo despues.
- Autorizar implicitamente DLL redistribuidas o reemplazar tema, mapas y graficos dentro de este refactor.
- Afirmar portabilidad integral mientras sigan existiendo las referencias NuGet de UI.

## Decisions

### 1. System.Text.Json es el motor elegido

Usar JsonSerializer.Serialize/Deserialize, JsonPropertyName y System.Text.Json.JsonException. No sustituirlo por DataContractJsonSerializer: el usuario eligio explicitamente System.Text.Json. Mantener los nombres de las propiedades de los requests y su estructura; no introducir una politica global camelCase para diccionarios de traduccion.

Para respuestas de Gemini se usa JsonDocument/JsonElement con recorrido explicito y comprobacion de objetos, arrays, nulls y propiedades ausentes. Escaneo reutiliza la extraccion del texto del cliente compartido. El ticket interno conserva un DTO privado con constructor publico explicito y atributos JsonPropertyName, validado por los tests de campos completos, opcionales y acentos. La clasificacion se lee con JsonDocument y conserva el fallback General.

### 2. Referencia directa, sin PackageReference productivo nuevo

El usuario confirmo que no tiene acceso al proyecto del laboratorio y ordeno avanzar con el codigo. Se usaran referencias directas compartidas por SERVICIOS y SERVICIOS.Tests, con `SystemTextJsonAssemblyDirectory` configurable y valor predeterminado `$(MSBuildToolsPath)`. No se agregan PackageReference productivos, rutas al cache ni DLL al repositorio.

En esta maquina, MSBuild de Visual Studio 18 Community incluye System.Text.Json 10.0.0.10 para .NET Framework (referencia mscorlib 4.0), junto a sus dependencias. Se verificara con build y tests; esa evidencia corresponde a esta maquina, no al laboratorio. Una instalacion MSBuild que no incluya esos ensamblados necesita configurar el directorio de referencia compatible; se informa esa limitacion, sin frenar la migracion ni pedir el proyecto inaccesible.

`SERVICIOS/SystemTextJson.references.props` centraliza las referencias a Json, Encodings.Web, Memory, Buffers, Unsafe, Tasks.Extensions, Bcl.AsyncInterfaces y Numerics.Vectors; agrega Pipelines y ValueTuple si estan disponibles en esa instalacion. `Directory.Build.targets` aplica las mismas referencias a UI y los demas proyectos de tests. El target UnifySystemTextJsonReferences conserva las referencias explicitas despues de que el SDK agrega dependencias de paquetes, evitando el error CS1705 por System.Memory antiguo. El build de tests todavia informa warnings MSB3243 por identidades antiguas de dependencias NuGet; la ejecucion funcional verifica la seleccion de ensamblados en este entorno.

Correccion del arranque Debug: se encontro `SERVICIOS.dll` referenciando Json 10.0.0.10 y `TpIDS.exe.config` redirigiendo a esa version, pero la DLL de salida era 8.0.0.5. El log de Rebuild confirma que `ReferenceCopyLocalPaths` contiene simultaneamente la copia NuGet 8.0.5 y la referencia directa de MSBuild 10.0.0.10: unificar `Reference` no unifica los assets de ejecucion que NuGet agrega por separado. Antes de `_CopyFilesMarkedCopyLocal`, normalizar `ReferenceCopyLocalPaths` por nombre y extension para los ensamblados del grupo compartido y conservar exclusivamente sus HintPath directos. Verificar la lista de copia, las identidades y hashes de salida en Rebuild y Build incremental Debug, y ejecutar un smoke de carga con el config del ejecutable para inicializar IdiomaCatalogoService y comprobar el round-trip JSON. No fijar versiones nuevas en App.config ni agregar paquetes.

Verificacion de referencia JSON sin cache: se eliminaron exclusivamente los archivos generados de restore de Newtonsoft en SERVICIOS/obj y se ejecuto MSBuild sobre SERVICIOS.csproj con `/t:Rebuild /p:Configuration=Release /p:NuGetPackageRoot=C:\dev\TpIDS-TD\TestResults\empty-nuget-cache\`, sin `/restore`; exit code 0.

### 3. Compatibilidad de catalogos y respuestas

- Exportar catalogos compactos, UTF-8 sin BOM, conservando acentos literales. Configurar el encoder de exportacion para ese contrato; el archivo no se inserta como HTML. No imponer esa configuracion a requests HTTP sin necesidad.
- Mantener Error.Language.JsonFileNotFound, JsonInvalidFormat y JsonEmpty para catalogos de strings validos y errores cubiertos. Se usa parsing estandar de System.Text.Json: las coerciones de valores no string y sintaxis permisiva propia de Newtonsoft no constituyen un contrato de catalogo valido; no se agrega un emulador de Newtonsoft.
- Mantener el texto del primer candidate/part, respuestas ausentes, monto nullable, fechas invalidas, textoCrudo vacio y fallback General.
- Migrar los fixtures y assertions de los cuatro archivos de tests: JsonDocument para inspeccion de requests y JsonSerializer para construir respuestas. Comparar estructuras JSON, salvo el contrato de bytes de exportacion que ya esta probado.

### 4. Restriccion permanente e inventario

Agregar a AGENTS.md una regla inviolable que exija compilacion y ejecucion de TpIDS.sln en laboratorio sin instalar/restaurar NuGet ni depender de su cache. Los tests de desarrollo quedan expresamente exceptuados. No convertir el funcionamiento con binarios ya compilados en evidencia de compilacion portable.

Inventario directo observado:

| Ambito | Paquetes |
| --- | --- |
| SERVICIOS | Sin paquetes NuGet; System.Text.Json por referencia directa |
| UI | GMap.NET.Core 2.1.7; GMap.NET.WinForms 2.1.7; Krypton.Toolkit 105.26.7.201; LiveChartsCore.SkiaSharpView.WinForms 2.0.5 |
| Tests | MSTest.TestFramework 4.2.3; MSTest.TestAdapter 4.2.3; Microsoft.NET.Test.Sdk 18.7.0; FlaUI.Core 4.0.0; FlaUI.UIA3 4.0.0 |

UI tambien copia libSkiaSharp.dll desde NuGetPackageRoot (SkiaSharp.NativeAssets.Win32 3.119.0). El inventario es de referencias directas, no de todo el grafo transitorio. tests/README.md documenta las referencias JSON y la restriccion, sin afirmar que UI ya la cumple. La portabilidad de UI debe llevarse a un cambio propio cuando se defina la distribucion permitida.

## Risks / Trade-offs

- [Referencia JSON del laboratorio sin identificar] → Usar los ensamblados netfx de MSBuild local y directorio configurable; informar que el laboratorio sigue sin verificarse.
- [Diferencias de serializacion, parsing y coercion] → Caracterizar los contratos antes de migrar; ciclo TDD cuando haya diferencias observables y mantener los tests de errores existentes.
- [UI conserva NuGet y una DLL nativa] → Informar el incumplimiento restante, registrar su resolucion como tarea y verificarla antes de afirmar portabilidad completa.
- [Trabajo concurrente sobre servicios] → Revisar los cambios locales y los cambios OpenSpec activos al aplicar; no sobrescribir nuevas implementaciones del chat.

## Migration Plan

1. Documentar la regla e inventario; configurar referencias directas a los ensamblados netfx de MSBuild local.
2. Obtener baseline de tests, caracterizar diferencias y migrar servicios y fixtures con verificacion por componente.
3. Eliminar referencias y usos activos de Newtonsoft; conservar menciones historicas en artefactos archivados.
4. Ejecutar la suite con integracion y compilar TpIDS.sln con MSBuild. Verificar el componente JSON sin restauracion/cache, y reportar separadamente el incumplimiento productivo de UI.
5. La comprobacion final de toda TpIDS.sln en una copia limpia sin NuGet pertenece al trabajo de portabilidad integral; no puede acreditarse con una compilacion que use el cache del desarrollador.

Para revertir el refactor, restaurar solo los cambios de esta migracion; conservar la regla de laboratorio confirmada por el usuario y el trabajo local ajeno.
