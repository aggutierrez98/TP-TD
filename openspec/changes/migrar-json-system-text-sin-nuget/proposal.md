# Proposal

## Why

FINAZAPP debe compilar desde el codigo fuente en el laboratorio de la facultad con .NET Framework 4.7.2, sin instalar ni restaurar paquetes NuGet. El usuario solicita reemplazar Newtonsoft.Json por System.Text.Json, que ya probo en ese entorno; los tests estan exceptuados porque solo se ejecutan en su maquina.

## What Changes

- Migrar todos los usos de Newtonsoft.Json en los cuatro servicios JSON y sus cuatro archivos de tests a System.Text.Json, conservando contratos JSON, errores de dominio y comportamiento de Gemini e idiomas.
- Quitar las referencias a Newtonsoft.Json de SERVICIOS y SERVICIOS.Tests. La referencia productiva a System.Text.Json debe resolverse sin agregar otro PackageReference ni depender de un cache NuGet.
- Escribir en AGENTS.md la restriccion inviolable: TpIDS.sln debe compilar y ejecutarse en el laboratorio sin instalacion/restauracion NuGet ni cache previo; los proyectos de tests quedan exceptuados y no deben incorporarse a la solucion productiva.
- Corregir tests/README.md: actualmente declara ausencia de NuGet productivo, pero UI referencia GMap.NET.Core, GMap.NET.WinForms, Krypton.Toolkit y LiveChartsCore.SkiaSharpView.WinForms; ademas copia el nativo de SkiaSharp desde NuGetPackageRoot.
- Registrar las dependencias encontradas y la resolucion de portabilidad restante en los artefactos de este cambio. La autorizacion para distribuir DLL externas no esta confirmada: no se interpreta el reemplazo de JSON como permiso para distribuirlas o redisenar toda la UI.

## Capabilities

### New Capabilities

Ninguna. Es un refactor de serializacion y documentacion de restricciones de compilacion; se declara `skip_specs: true`.

### Modified Capabilities

Ninguna. No se modifica el comportamiento funcional aceptado. La resolucion completa de dependencias visuales requiere una decision de distribucion adicional antes de planificar cambios sobre la UI.

## Impact

- SERVICIOS/{IdiomaCatalogoService,GeminiChatClient,GeminiEscaneoTicketService,GeminiAsistenteFinancieroService}.cs y SERVICIOS.csproj.
- Sus cuatro archivos de tests en tests/SERVICIOS.Tests y SERVICIOS.Tests.csproj; MSTest y FlaUI pueden seguir usando NuGet en desarrollo.
- AGENTS.md y tests/README.md. No se eleva el framework ni se modifica la matriz de capas.
- El usuario indica que no tiene acceso al proyecto del laboratorio y solicita avanzar con la migracion de codigo. Se usaran referencias directas a los ensamblados netfx disponibles en MSBuild, con directorio configurable, sin agregar paquetes productivos ni distribuir DLL en el repositorio. La compatibilidad concreta del laboratorio se verificara despues.
- Cambios abiertos como chat-asistente-sin-bloqueo-ui comparten archivos de servicios: preservar el trabajo existente y coordinar la migracion.
