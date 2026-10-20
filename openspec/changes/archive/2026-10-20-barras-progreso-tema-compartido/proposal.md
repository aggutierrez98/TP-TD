# Proposal

## Why

La aplicacion tiene dos barras de progreso con aspecto distinto: la del perfil inversor (`KryptonProgressBar` con el estilo escrito a mano en el formulario) y la del escaneo de tickets en `FormMovimientos` (`ProgressBar` nativo en modo `Marquee`, sin tema). El estilo vive duplicado en codigo de formulario y cualquier ajuste obliga a tocar cada barra.

## What Changes

- Un helper unico en `UI/Tema/` aplica a cualquier `KryptonProgressBar` el estilo comun: pista verde clara, relleno verde FINAZAPP, bordes suavemente redondeados y sin texto superpuesto.
- La barra del perfil inversor deja de definir su estilo localmente y usa el helper. Su comportamiento (avance por pregunta, 0 a 10) no cambia.
- La barra del escaneo de tickets pasa a Krypton, usa el mismo helper y conserva su animacion de espera, porque alli no se conoce un porcentaje real.
- Las barras futuras se tematizan con el mismo helper.

Fuera de alcance: el comportamiento de avance del cuestionario, la logica del escaneo OCR, otras barras o indicadores de progreso nuevos.

## Capabilities

### New Capabilities

### Modified Capabilities
- `tema-visual`: agrega el requisito de estilo comun para las barras de progreso.

## Impact

- `UI/Tema/TemaFinanzapp.cs` (nuevo helper), `UI/FormPerfilInversor.cs`, `UI/FormMovimientos.Designer.cs` y `UI/FormMovimientos.cs`.
- Sin cambios en capas inferiores, referencias entre proyectos, traducciones ni NuGet.
