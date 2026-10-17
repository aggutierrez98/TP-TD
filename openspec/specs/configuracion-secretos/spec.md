# configuracion-secretos Specification

## Purpose
Define dónde viven los valores sensibles de configuración de FINAZAPP (claves de API y credenciales), qué parte de la configuración queda versionada y cómo se comporta la aplicación cuando un secreto no está configurado.

## Requirements

### Requirement: Secretos fuera del control de versiones
Los valores sensibles de configuración (claves de API, tokens, contraseñas) SHALL guardarse en un archivo local excluido del control de versiones, nunca en un archivo versionado ni en su historial publicado.

#### Scenario: Clonar el repositorio
- **WHEN** alguien clona el repositorio
- **THEN** no obtiene ningún valor de clave de API ni credencial

#### Scenario: Agregar un secreto nuevo
- **WHEN** una funcionalidad necesita un valor sensible nuevo
- **THEN** el valor va al archivo local de secretos y su clave, sin valor, a la plantilla versionada

### Requirement: Plantilla versionada de secretos
El repositorio SHALL incluir una plantilla con todas las claves de secretos que usa la aplicación y sus valores vacíos, para crear el archivo local en una máquina nueva.

#### Scenario: Preparar una máquina nueva
- **WHEN** un desarrollador copia la plantilla con el nombre del archivo local de secretos y completa los valores
- **THEN** la aplicación lee esos valores sin otros cambios de configuración

### Requirement: Funcionamiento sin archivo de secretos
La aplicación SHALL compilar e iniciar aunque el archivo local de secretos no exista. Las funcionalidades que no dependen de un secreto SHALL funcionar normalmente.

#### Scenario: Compilar y ejecutar sin secretos
- **WHEN** se compila y ejecuta la aplicación en una máquina sin el archivo de secretos
- **THEN** la compilación termina sin errores, la aplicación inicia y el login y los ABM funcionan

#### Scenario: Usar una funcionalidad de IA sin la clave
- **WHEN** el usuario escanea un ticket o usa el asistente personal y la clave de Gemini no está configurada
- **THEN** la aplicación muestra el mensaje traducido de clave faltante y no se cierra

### Requirement: Configuración no sensible versionada
Los valores de configuración que no son secretos (modelo de IA, cadenas de conexión con seguridad integrada) SHALL seguir versionados en la configuración de la aplicación.

#### Scenario: Cambiar el modelo de Gemini
- **WHEN** se cambia el modelo de Gemini en la configuración versionada
- **THEN** el cambio llega a todos los clones sin tocar el archivo de secretos
