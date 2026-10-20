# Tasks

## 1. Spike y helper del tema

- [x] 1.1 Verificar si `KryptonProgressBar` 105.26.7 anima `ProgressBarStyle.Marquee` con un programa descartable fuera del repo, y registrar el resultado en `design.md` (decision: Marquee nativo o `Timer`)
- [x] 1.2 Agregar `TemaFinanzapp.TematizarBarraProgreso(KryptonProgressBar)` con el estilo comun y verificar que compila `TpIDS.sln` (exit 0). Compilo sin errores.

## 2. Barra del perfil inversor

- [x] 2.1 Reemplazar el bloque de estilo local de `FormPerfilInversor` por el helper y verificar que la barra se dibuja igual (renderizado del control con valores 0/1/5/10, como en `perfil-inversor-usuario`). Control real con el helper compilado: pixeles de relleno 0/69/359/720 sobre 724 de ancho, pista clara en el resto.

## 3. Barra del escaneo de tickets

- [x] 3.1 Respaldar `FormMovimientos.Designer.cs`, cambiar `prgbEscaneoTicket` a `KryptonProgressBar` editando a mano y verificar con diff que no cambia ningun otro control. Diff contra el backup: solo la declaracion y el `new` de `prgbEscaneoTicket`.
- [x] 3.2 Aplicar el helper en el constructor de `FormMovimientos` y conservar `Style = Marquee` con `MarqueeAnimationSpeed = 30`, y verificar que la barra se anima al mostrar `pnlEscaneoTicket`. Control real con el helper: 5 frames distintos en 5 capturas.
- [x] 3.3 Verificar con una captura real de la barra en espera que coincide con la del perfil inversor. Verificado renderizando el control real con el helper (mismos colores de pista y relleno); no se tomo captura del formulario completo.

## 4. Cierre

- [x] 4.1 Compilar `TpIDS.sln` y correr `tests/tools/run-tests.ps1 -IncludeIntegration` en verde; informar contadores del TRX. Resultado: unitarios 1121/1121, integracion 252/252, `TpIDS.sln` Debug sin errores.
- [x] 4.2 Avisar al usuario que la suite UI toma el escritorio ~5 minutos y correrla solo con su confirmacion, para validar el flujo de escaneo en Movimientos Corrida filtrada a pedido del usuario (`-UiFilter FIN_MovimientosPdf,FIN_PerfilInversor`): UI 10/10, unitarios 1173/1173, integracion 262/262, exit 0. Ningun test UI ejercita el flujo de escaneo (requiere API key de Vision), asi que la animacion de la barra en ese flujo no esta cubierta por la suite.
