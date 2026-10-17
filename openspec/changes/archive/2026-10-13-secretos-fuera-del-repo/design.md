# Design

## Context

En .NET Framework, `ConfigurationManager` lee solamente el config del ejecutable (`TpIDS.exe.config`, generado desde `UI/App.config`); los `App.config` de las bibliotecas no se usan en ejecución. Por eso la clave de Gemini está en UI aunque la consumen tres servicios de SERVICIOS (`GeminiEscaneoTicketService`, `GeminiAsistenteFinancieroService` y `AgenteIA`). Los tres ya lanzan `ExcepcionTecnica` con una key traducible cuando la clave está vacía.

La suite UI (`AppUnderTestStager`) copia la carpeta de salida de UI a un directorio de staging y reescribe solo las cadenas de conexión del `.exe.config`.

## Goals / Non-Goals

**Goals:**
- Sacar `GeminiApiKey` de todo archivo versionado y del commit local que la contiene.
- Dejar un mecanismo reutilizable para secretos futuros.

**Non-Goals:**
- Cifrar el archivo de secretos (DPAPI o `aspnet_regiis`): la app corre en máquinas de desarrollo y del laboratorio.
- Un guard de build que detecte secretos en `App.config`.
- Mover las cadenas de conexión: no contienen credenciales.

## Decisions

**`<appSettings file="secrets.config">` en lugar de variables de entorno.** El atributo `file` combina las claves del archivo externo con las de `appSettings`, y si el archivo no existe .NET lo ignora sin error. No requiere cambios de código: los servicios siguen leyendo `ConfigurationManager.AppSettings["GeminiApiKey"]`. Las variables de entorno obligarían a cambiar los tres servicios y a configurar cada máquina del laboratorio fuera de la carpeta de la app.

**`configSource` descartado.** Reemplaza la sección entera y falla si el archivo no existe. Eso rompe el requisito de iniciar sin secretos.

**Copia al output condicionada a que el archivo exista.** Un ítem `None` con `CopyToOutputDirectory=PreserveNewest` y `Condition="Exists('secrets.config')"` en `UI/UI.csproj`. Sin la condición, el build falla en un clon nuevo. Como el stager copia la carpeta de salida completa, el archivo llega también a la suite UI.

**Reescribir el commit local en lugar de un commit que borre la clave.** Un commit nuevo deja la clave en el historial. El commit `792acdc` es `HEAD` y no está publicado, así que alcanza con un `git commit --amend` que incluya solo `UI/App.config` sin la clave. El resto del working tree, con mucho trabajo sin commitear, queda intacto. Se hace con confirmación explícita del usuario y nunca con `git checkout` ni `git restore`.

## Risks / Trade-offs

- [Un clon nuevo o el laboratorio no tienen `secrets.config`, así que el escaneo de tickets y el asistente fallan] → Esas pantallas muestran el mensaje de clave faltante. La plantilla documenta qué completar.
- [El amend se hace con el working tree lleno de cambios sin commitear] → Se stagea solo `UI/App.config` y se revisa `git show --stat HEAD` antes y después.
- [La clave sigue en objetos sueltos del repo local después del amend] → No se publican. Rotarla es opcional, porque nunca salió de la máquina.

## Migration Plan

1. Crear `UI/secrets.config` con la clave actual y la plantilla `secrets.config.example`, y agregar `UI/secrets.config` a `.gitignore`.
2. Quitar la clave de `UI/App.config` y agregar el atributo `file`.
3. Compilar, verificar que `secrets.config` llega a `bin/` y que el escaneo de tickets funciona.
4. Amend del commit `792acdc` con el usuario.

Rollback: volver a poner la clave en `App.config`. No hay datos que migrar.
