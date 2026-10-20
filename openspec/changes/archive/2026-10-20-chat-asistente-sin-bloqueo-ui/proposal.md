# Proposal

## Why

El usuario reporta que el chat del asistente personal congela la interfaz mientras responde. Aunque el formulario y Gemini ya usan async/await, el envio consulta SQL sincronicamente antes de ceder el hilo, a diferencia del escaneo de tickets; es necesario garantizar que el procesamiento del envio no bloquee la UI.

## What Changes

- Ejecutar el trabajo del envio fuera del hilo de UI, incluyendo la lectura del historial y la persistencia sincrona existente, conservando la llamada HTTP asincrona.
- Mantener la interfaz receptiva durante el envio y evitar envios duplicados desde boton o Enter.
- Restaurar los controles ante exito o error y proteger las continuaciones cuando el formulario se haya cerrado.
- Verificar la regresion con dependencias lentas controladas y comprobar el comportamiento real del shell durante la espera.

## Capabilities

### New Capabilities

- `chat-asistente-personal`: envio de preguntas y recepcion de respuestas sin bloquear la interfaz, con persistencia y recuperacion del estado visual.

### Modified Capabilities

Ninguna.

## Impact

- `BLL/AsistenteFinancieroBLL.cs`, `UI/FormAsistentePersonal.cs` y tests relacionados.
- Se conservan las firmas de repositorios, los contratos del servicio, CompositionRoot y la transaccion de consulta/bitacora.
- Compatible con .NET Framework 4.7.2 y C# 7.3; sin dependencias nuevas.
- Fuera de alcance: streaming, cancelacion de llamadas a Gemini, rediseño visual y conversion general de DAL a SQL asincrono.
