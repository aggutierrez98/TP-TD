# Spec Delta

## Purpose

Garantiza que la solucion productiva se pueda compilar y ejecutar en el laboratorio de la facultad a partir del codigo fuente, sin instalar ni restaurar paquetes NuGet.

## ADDED Requirements

### Requirement: Compilacion sin NuGet
La solucion productiva `TpIDS.sln` MUST compilar desde el codigo fuente en un entorno con .NET Framework 4.7.2 sin ejecutar restore y sin depender del cache NuGet.

#### Scenario: Compilacion en clon limpio
- **WHEN** se clona el repositorio en una carpeta nueva, con el cache NuGet vacio y sin red, y se ejecuta MSBuild sobre `TpIDS.sln`
- **THEN** la compilacion termina con exit code 0 y sin pasos de restore

#### Scenario: Sin referencias a NuGet en proyectos productivos
- **WHEN** se inspeccionan los proyectos de `TpIDS.sln`
- **THEN** ninguno contiene `PackageReference`, `packages.config` ni rutas a `NuGetPackageRoot`

### Requirement: Dependencias externas versionadas con el codigo
Toda biblioteca externa de los proyectos productivos MUST estar incluida en el repositorio con su version, origen y licencia documentados.

#### Scenario: Inventario completo
- **WHEN** se revisa `lib/`
- **THEN** cada DLL referenciada por UI figura en un README con paquete, version y licencia

### Requirement: Ejecucion con dependencias nativas
La aplicacion compilada MUST arrancar y mostrar el mapa de comercios y los graficos sin que existan paquetes NuGet en la maquina.

#### Scenario: Arranque y pantallas con graficos
- **WHEN** se ejecuta `TpIDS.exe` generado en el clon limpio y se abren el Dashboard y la seleccion de ubicacion de comercio
- **THEN** ambas pantallas cargan y renderizan sin excepciones de carga de ensamblados ni de bibliotecas nativas

### Requirement: Tests de desarrollo exceptuados
Los proyectos de tests y `tests/TpIDS.Tests.sln` MAY usar NuGet y MUST NOT incorporarse a `TpIDS.sln`.

#### Scenario: Suite de tests sigue funcionando
- **WHEN** se ejecuta `tests/tools/run-tests.ps1 -IncludeIntegration` en la maquina de desarrollo
- **THEN** termina en verde con los mismos contadores que antes del cambio
