# Proposal

## Why

Casi todos los formularios de FINAZAPP tienen tamaño fijo: 23 de 28 usan `FormBorderStyle.FixedSingle` o `FixedDialog` con `MaximizeBox = false`, y sus controles estan posicionados en coordenadas absolutas (no hay ningun `TableLayoutPanel` y solo Balance, Dashboard y Usuarios usan `Anchor`). Login, Registro, Balance y Dashboard ya tienen borde redimensionable, pero su contenido se adapta solo en parte (Balance y Dashboard) o nada (Login y Registro). Al agrandar la ventana principal, las pantallas no aprovechan el espacio: las grillas quedan chicas y sobra area vacia. Con la migracion a Krypton cerrada, la UI ya tiene una base visual estable sobre la que reacomodar el layout.

## What Changes

- Todo formulario salvo `FormPrincipal` pasa a ser redimensionable por el usuario (borde arrastrable) y su contenido se reacomoda para cubrir el area de la ventana.
- Los controles que muestran datos (grillas, arboles, listas, historial del asistente, mapa, area de tarjetas) se estiran para ocupar el espacio libre; los campos de texto y combos se ensanchan; botones y etiquetas mantienen su tamaño y se reposicionan.
- Cada formulario tiene un tamaño minimo igual a su tamaño de diseño actual: no se puede achicar por debajo de lo que hoy se ve bien.
- Todas las pantallas tienen boton de maximizar; en las abiertas dentro de la ventana principal, maximizar ocupa el area de trabajo sin fusionar su barra de titulo (como Balance y Dashboard), y restaurar vuelve al tamaño anterior.
- En Login y Registro el bloque de campos crece hasta el doble de su ancho y despues queda fijo y centrado.
- El tamaño con el que abre cada formulario no cambia (los tests UI y los usuarios ven la misma pantalla inicial).
- Se reutiliza el mecanismo nativo de WinForms (`Anchor`, `Dock`, `TableLayoutPanel`, `SplitContainer`); no se escalan fuentes ni se agrega una libreria nueva.

## Capabilities

### New Capabilities
- `ventanas-redimensionables`: comportamiento de los formularios ante el cambio de tamaño de su ventana (redimensionables, tamaño minimo, que controles se estiran y cuales se reposicionan, tamaño inicial).

### Modified Capabilities
- (ninguna: `tema-visual` y `compatibilidad-tests-ui` viven en el change `migrar-ui-krypton`, aun sin archivar; este change no altera sus requisitos)

## Impact

- **Codigo:** los 27 `UI/*.Designer.cs` (todos salvo `FormPrincipal`) y `UI/PasswordTextBox.Designer.cs`; ajustes menores en `FormBalanceGeneral.cs`/`FormDashboardAnaliticas.cs` si su logica de tamaño inicial lo requiere. Sin cambios en BLL, REPOSITORIO, DAL, SERVICIOS ni BE.
- **Tema:** `TemaFinanzapp` ya pinta `TableLayoutPanel`/`FlowLayoutPanel`/`Panel` con el fondo de la paleta; `SplitContainer` se agrega a ese helper si se usa.
- **Tests UI:** los helpers de `UiFind` buscan por `AutomationId` entre descendientes, asi que los contenedores de layout nuevos no rompen la busqueda; hay que correr la suite completa al cierre.
- **Dependencias:** ninguna nueva. Depende de `migrar-ui-krypton` (controles Krypton ya en los designers).
