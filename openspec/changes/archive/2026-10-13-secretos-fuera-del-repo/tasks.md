# Tasks

## 1. Archivo local de secretos

- [x] 1.1 Crear `UI/secrets.config` (raíz `<appSettings>`) con la clave `GeminiApiKey` actual y agregar `UI/secrets.config` a `.gitignore`; verificar con `git check-ignore UI/secrets.config` que queda ignorado
- [x] 1.2 Crear `UI/secrets.config.example` con las mismas claves y valores vacíos, más un comentario que indique copiarlo como `secrets.config`; verificar que `git status` lo muestra como archivo nuevo
- [x] 1.3 En `UI/App.config`, quitar `GeminiApiKey` y agregar `file="secrets.config"` a `<appSettings>` (`GeminiModel` queda); verificar con `rg GeminiApiKey UI/App.config` que no queda la clave
- [x] 1.4 En `UI/UI.csproj`, copiar `secrets.config` al output con `PreserveNewest` solo si existe; compilar `TpIDS.sln` y verificar que `UI/bin/Debug/secrets.config` existe
- [x] 1.5 Renombrar temporalmente `secrets.config`, compilar y verificar que el build pasa y que la app inicia y muestra el mensaje de clave faltante al escanear un ticket; restaurar el archivo

## 2. Verificación y documentación

- [x] 2.1 Correr `tests/tools/run-tests.ps1 -IncludeIntegration` en verde y probar con el usuario el escaneo de tickets o el asistente con la clave cargada desde `secrets.config`
- [x] 2.2 Documentar en `.agents/01-stack-dotnet-winforms.md` la regla: todo secreto nuevo va a `secrets.config` y a la plantilla, nunca a `App.config`

## 3. Historial

- [x] 3.1 Con confirmación del usuario, hacer amend del commit local `792acdc` stageando solo `UI/App.config` sin la clave; verificar con `git log -S <prefijo de la clave> --all` que ningún commit la contiene antes del primer push
