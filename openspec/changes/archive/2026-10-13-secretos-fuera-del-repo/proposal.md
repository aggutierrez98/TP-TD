# Proposal

## Why

La clave de API de Gemini está en texto plano en `UI/App.config`, que está versionado. Ya entró en el commit local `792acdc` ("add parts of forms"), que todavía no se publicó: `main` va 3 commits adelante de `origin/main`. Si se hace push, la clave queda expuesta en GitHub y hay que rotarla.

## What Changes

- Los valores sensibles de configuración pasan a un archivo local `UI/secrets.config`, excluido del control de versiones. `UI/App.config` lo referencia con `<appSettings file="secrets.config">`.
- Se versiona una plantilla `UI/secrets.config.example` con las mismas claves y los valores vacíos.
- El build copia `secrets.config` junto al ejecutable cuando existe. Si no existe, compila igual.
- Se quita la clave del commit local `792acdc` antes del primer push.
- Relevamiento de valores sensibles: el único es `GeminiApiKey`. `GeminiModel` no es secreto y queda en `App.config`. Las cadenas de conexión (`UI/App.config` y `tests/TpIDS.IntegrationTests/App.config`) usan `Integrated Security=True` y no llevan credenciales.

## Capabilities

### New Capabilities
- `configuracion-secretos`: dónde viven los valores sensibles de configuración, qué queda versionado y cómo se comporta la app cuando falta un secreto.

### Modified Capabilities

## Impact

- `UI/App.config`, `UI/UI.csproj`, `.gitignore`, nuevo `UI/secrets.config.example`.
- Sin cambios de código en SERVICIOS: `ConfigurationManager.AppSettings` lee los valores combinados de forma transparente.
- Máquina del laboratorio y otros clones: hay que crear `secrets.config` a mano a partir de la plantilla para usar las funciones de IA.
- Historial de git: se reescribe un commit que no está publicado.
