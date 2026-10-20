# Design

## Context

Hoy `FormPerfilInversor.cs:59-73` define el estilo de su `KryptonProgressBar` en el propio formulario. En Krypton, `StateCommon.Back` alimenta tambien el relleno y `StateNormal.Back` define la pista (hallazgo del change `perfil-inversor-usuario`). `FormMovimientos.Designer.cs:416-423` usa un `System.Windows.Forms.ProgressBar` nativo en `Marquee`. `TemaFinanzapp` ya concentra helpers de tematizacion (`TematizarGrilla`) y `FinanzappPaleta` tiene `Verde` y `VerdeSuave`.

## Goals / Non-Goals

**Goals:**
- Un solo punto de definicion del estilo de barra, en `UI/Tema/`.
- Ambas barras con el mismo aspecto y su comportamiento propio intacto.

**Non-Goals:**
- Cambiar la paleta global de Krypton para las barras: el helper por control basta y no afecta a otros controles.

## Decisions

- **Helper `TemaFinanzapp.TematizarBarraProgreso(KryptonProgressBar)`**, extraido tal cual del bloque actual del perfil inversor (pista `VerdeSuave`, relleno `Verde` solido, `Rounding` 3, `Text` vacio, `UseValueAsText = false`). Alternativa descartada: una subclase `BarraProgresoFinanzapp`; el helper sigue el patron existente y no agrega un tipo que los tests UI deban conocer.
- **Escaneo**: se reemplaza el control nativo por `KryptonProgressBar` en el designer, editado a mano (el disenador de VS rompe la grilla de `FormMovimientos`; respaldar antes y diffear despues). El helper se aplica en el constructor del formulario.
- **Animacion de espera**: spike del 2026-10-20 (control real de `Krypton.Toolkit` 105.26.7.201 con el estilo comun, capturas cada 400 ms): `Style = Marquee` con `MarqueeAnimationSpeed = 30` anima un segmento verde sobre la pista verde clara (5 frames distintos en 5 capturas). Se usa `Marquee` nativo de Krypton; el `Timer` se descarta.
- **Nombre del control**: se conserva `prgbEscaneoTicket` para respetar la nomenclatura y no romper referencias.

## Risks / Trade-offs

- [Editar el designer de `FormMovimientos`] -> backup previo, edicion manual y diff posterior.
- [Tests UI de Movimientos dependen del panel de escaneo] -> `prgbEscaneoTicket` conserva nombre y posicion; se verifica con compilacion y la suite UI previo aviso al usuario.
