## Context

Estado medido en los designers (12/10/2026), sin contar `FormPrincipal`:

- 23 formularios con `FormBorderStyle.FixedSingle`/`FixedDialog` y `MaximizeBox = false`; Login, Registro, Balance y Dashboard ya tienen borde redimensionable.
- Posicionamiento absoluto en todos: 0 `TableLayoutPanel`, `Anchor` solo en Balance (4), Dashboard (3) y Usuarios (2). Ningun `MinimumSize`.
- Todas las grillas ya usan `AutoSizeColumnsMode = Fill` (las columnas se reparten el ancho solas).
- Los helpers de `UiFind` ubican controles con `FindFirstDescendant` por `AutomationId`: un contenedor de layout intermedio no rompe la busqueda.
- Solo las tarjetas del Dashboard se posicionan por codigo, dentro de un `FlowLayoutPanel` que ya reacomoda.
- Balance y Dashboard abren ocupando el `MdiClient` con su propia barra de titulo (`WindowState = Maximized` fusionaba la barra con la ventana principal y el usuario no lo quiso).

Controles Krypton relevantes: `KryptonGroupBox` aloja sus hijos en `.Panel` (los anclajes de los hijos son relativos a ese panel); `KryptonForm` admite `FormBorderStyle.Sizable`.

## Goals / Non-Goals

**Goals:**
- Que cada formulario (salvo `FormPrincipal`) se pueda redimensionar y su contenido cubra el area nueva.
- Que el tamaño inicial y la disposicion al abrir no cambien (suite UI y capturas actuales siguen validas).
- Mantener los designers editables desde Visual Studio (todo el layout expresado en propiedades del designer).

**Non-Goals:**
- Escalar fuentes o controles en proporcion al tamaño de la ventana.
- Achicar formularios por debajo de su tamaño de diseño (no hay layout "compacto").
- Cambiar el comportamiento de `FormPrincipal` o del area MDI.
- Rediseñar pantallas (orden de campos, textos, agrupaciones).

## Decisions

### 1. Tecnica: `Anchor`/`Dock` primero, `TableLayoutPanel` solo donde hay reparto proporcional

Alternativas evaluadas:

| Tecnica | Que logra | Costo / riesgo | Veredicto |
|---|---|---|---|
| **`Anchor`/`Dock` por control** | El control se estira o se mueve con el borde de su contenedor | Bajo: una propiedad por control. No resuelve dos controles lado a lado que deban crecer (se superponen) | Base del cambio |
| **`TableLayoutPanel`** con columnas en `Percent` | Reparte el ancho entre varios campos o zonas en proporcion | Medio: hay que reubicar controles en celdas; anida un nivel mas | Solo filas con varios campos y pantallas con dos zonas de datos |
| `SplitContainer` | Dos zonas con divisor arrastrable | Agrega un control nativo a tematizar y un comportamiento nuevo (divisor) que nadie pidio | Descartado (se puede sumar despues) |
| Escalado proporcional (`Control.Scale` o un helper en `Resize` que multiplique posiciones, tamaños y fuentes) | Todo "se agranda" | Fuentes y alturas de `KryptonTextBox`/combos escalados se ven mal, acumula errores de redondeo y pelea con el escalado por DPI; Krypton toma la fuente de la paleta | Descartado |

Fuente: guia de Microsoft "Best Practices for the TableLayoutPanel Control": usar `TableLayoutPanel` con moderacion, solo para partes que crecen en proporcion entre si; para una zona grande que debe llenar el espacio, alcanza con `Anchor`.

### 2. Reglas de anclaje por tipo de control (convencion del cambio)

| Elemento | Anclaje |
|---|---|
| Titulo de la pantalla | `Top, Left` (sin cambios) |
| Grupo de filtros o de edicion arriba de la grilla | `Top, Left, Right` |
| Grilla/arbol/lista principal | `Top, Bottom, Left, Right` |
| Grupo de edicion debajo de la grilla (por ejemplo, Movimientos) | `Bottom, Left, Right` |
| Campo unico en su fila dentro de un grupo | `Top, Left, Right` |
| Varios campos en una fila | `TableLayoutPanel` con columnas `Percent`, campos `Dock = Fill` o `Left, Right` en su celda; etiquetas en la fila de arriba |
| Botones de accion de un grupo | se mantienen en su posicion relativa al grupo (`Top, Left`); los de un grupo anclado abajo, `Bottom, Left` |
| Botones Aceptar/Cancelar de dialogos | `Bottom, Right` |
| Paginacion (Balance) | `Bottom, Left` |
| Mapa, historial del asistente, area de tarjetas | `Top, Bottom, Left, Right` |

Dos zonas de datos (Permisos, Usuarios, Comercios, Monedas, Detalle de comercio, Historial): un `TableLayoutPanel` de 2 columnas (o filas) al 50% que aloje cada zona con `Dock = Fill`.

### 3. Borde y maximizar

`FormBorderStyle = Sizable` y `MaximizeBox = true` en el designer de cada formulario (visible y editable en Visual Studio).

Decision del usuario: maximizar una pantalla MDI debe dejarla "grande pero adentro de la ventana principal", con su barra de titulo propia, como abren Balance y Dashboard. El maximizado nativo de un hijo MDI fusiona la barra con la ventana principal, asi que se reemplaza en `FormTraducibleBase` (solo si `IsMdiChild`):

- Al pedir maximizar: guardar los limites actuales y asignar `Location = Point.Empty` y `Size = MdiClient.ClientSize`, dejando `WindowState = Normal`.
- Al volver a pedirlo: restaurar los limites guardados.
- Mientras este "maximizada": suscribirse a `MdiClient.Resize` para seguir el area de trabajo.
- Balance y Dashboard pasan a usar este mismo mecanismo al abrir (reemplaza su codigo actual en `Load`).

Punto a verificar (spike, tarea 1.2): por donde llega el pedido de maximizar. `KryptonForm` dibuja sus propios botones de ventana; puede llegar como `WM_SYSCOMMAND/SC_MAXIMIZE` (interceptable en `WndProc`) o como cambio directo de `WindowState` (interceptable en `OnResize`/`OnSizeChanged`, con riesgo de parpadeo). El spike decide y verifica tambien el icono del boton (maximizar/restaurar). Los dialogos modales maximizan de forma nativa (no son hijos MDI).

### 3b. Login y Registro: bloque con ancho maximo y centrado

Decision del usuario: el bloque de campos se ensancha hasta el doble de su ancho de diseño y despues queda fijo y centrado. `Anchor` no puede limitar un ancho y a la vez centrar, asi que el contenido de cada uno se agrupa en un `Panel` contenedor y un helper de UI (por ejemplo, `LayoutHelper.CentrarConAnchoMaximo(panel, anchoMaximo)`, en `UI/`) recalcula en `Resize`: `ancho = min(ClientSize.Width - 2 * margen, 2 * anchoDiseño)`, `Left = (ClientSize.Width - ancho) / 2` y `Top = (ClientSize.Height - alto) / 2`. Dentro del panel, campos `Top, Left, Right`. `PasswordTextBox` ya ancla su caja de texto. El panel nuevo no cambia nombres ni `Tag` de los controles que contiene.

### 4. Tamaño minimo: centralizado en `FormTraducibleBase`

`FormTraducibleBase.OnLoad` asigna `MinimumSize = Size` si el formulario es redimensionable y no tiene minimo propio. Se hace en runtime y no en cada designer porque el tamaño exterior depende del marco que dibuja Krypton (el designer solo conoce `ClientSize`); asi el minimo es exactamente el tamaño con el que abre. Se ejecuta antes de `base.OnLoad`, o sea antes de que Balance/Dashboard se agranden al area MDI. Excluye `FormPrincipal`.

### 5. Ejecucion por lotes con verificacion visual

Igual que en `migrar-ui-krypton`: lotes de formularios, compilar y correr unit + integracion por lote, y un test temporal de capturas (formulario a tamaño de diseño y agrandado ~50%) que se borra al final. Los anclajes se escriben a mano en los designers (son decisiones por pantalla); no se automatiza con el script de migracion.

### 6. Tests UI de redimension

Se agregan tests FlaUI para dos pantallas representativas (una de gestion con grilla y un dialogo): agrandar la ventana por `TransformPattern` y comprobar que la grilla crece y que el minimo se respeta. Se escriben despues del desarrollo (regla de `strict-tdd` para tests UI).

## Dimensionamiento

| Categoria | Formularios | Trabajo por formulario | Esfuerzo |
|---|---|---|---|
| **A. Simples** (anclajes, sin `TableLayoutPanel`; Login/Registro con panel centrado) | Login, Registro, ConfiguracionUsuario, ContingenciaIntegridad, SeleccionarTarjetaDashboard, SeleccionarUbicacionComercio, AsistentePersonal, Backup, DashboardAnaliticas (9) | Borde + 3 a 8 anclajes | Bajo (~10 min c/u) |
| **B. Una grilla + un grupo** | GestionCategorias, ListaDeCompras, GestionEntidadesFinancieras, GestionMediosPago, GestionNotificaciones, GestionCuentasFinancieras, ReglasCategorizacion, Bitacora, BalanceGeneral, DetalleEntidadFinanciera (10) | Anclajes + `TableLayoutPanel` en filas con 2 o mas campos | Medio (~20-30 min c/u) |
| **C. Varias zonas o muchos campos** | GestionComercios, GestionMonedas, DetalleComercio, HistorialUsuario, GestionUsuarios, GestionPermisos, GestionIdiomas, Movimientos (8) | `TableLayoutPanel` de zonas + filas de campos; revisar TabIndex | Alto (~45-60 min c/u) |

Total: 27 formularios mas `PasswordTextBox` (control compuesto: anclar su `TextBox` y el icono). Fundaciones: `MinimumSize` centralizado, maximizar dentro del area MDI (spike + implementacion, ~1-2 h) y el helper de bloque centrado (~30 min). Estimacion total: 3 a 4 sesiones de trabajo (fundaciones + A, B, C) mas la suite UI completa al cierre.

## Risks / Trade-offs

- **Maximizar simulado en hijos MDI**: si `KryptonForm` no expone un punto limpio para interceptarlo, puede haber parpadeo o el icono del boton puede no alternar entre maximizar y restaurar. Mitigacion: spike antes de tocar formularios; si no hay forma limpia, volver a consultar al usuario.
- **Reubicar controles en un `TableLayoutPanel` cambia el padre del control** → `IdiomaFormHelper` y `TemaFinanzapp` recorren el arbol recursivamente, asi que siguen alcanzandolos; los `AutomationId` no cambian. Mitigacion: suite UI completa al cierre.
- **Pequeños corrimientos al abrir** (el `TableLayoutPanel` impone su propio espaciado) → contradice "tamaño inicial sin cambios". Mitigacion: celdas en `Absolute`/`Percent` calculadas desde las posiciones actuales y comparacion de capturas a tamaño de diseño antes/despues.
- **Orden de tabulacion** (`TabIndex`) se rompe al mover controles a contenedores nuevos → revisarlo en cada formulario de las categorias B y C.
- **Hijo MDI agrandado mas alla del area visible** → el `MdiClient` muestra barras de desplazamiento; es el comportamiento estandar de WinForms y se acepta.
- **El designer de Visual Studio puede reescribir el archivo al abrirlo** → los cambios se hacen en `.Designer.cs` con la misma forma que genera el designer; abrir uno por categoria en Visual Studio para confirmar que no se rompe.

## Migration Plan

Cambio solo de UI, sin datos ni configuracion. Se aplica por lotes; cada lote compila y deja los tests en verde. Rollback: revertir los designers del lote.

## Open Questions

- (Resuelta 12/10/2026) Maximizar: boton en todas las pantallas; en las MDI ocupa el area de trabajo sin fusionar la barra de titulo (decision 3).
- (Resuelta 12/10/2026) Login/Registro: el bloque crece hasta el doble de su ancho y despues queda fijo y centrado (decision 3b). Supuesto: verticalmente el bloque conserva su alto y queda centrado.
- (Resuelta 12/10/2026, spike 1.2) Mecanismo para interceptar el maximizar de `KryptonForm`: ver "Resultado del spike".

## Resultado del spike (tarea 1.2)

Leido el codigo de Krypton.Toolkit 105.26.7.201 (descompilado): el boton maximizar (`ButtonSpecFormWindowMax.OnClick`) llama a `SendSysCommand(WindowState == Maximized ? SC_RESTORE : SC_MAXIMIZE)`, que es un `SendMessage(WM_SYSCOMMAND)` sincronico. El doble clic en la barra y el menu de sistema llegan por el mismo mensaje (via `DefWndProc`). Por eso `FormTraducibleBase.WndProc` intercepta `WM_SYSCOMMAND/SC_MAXIMIZE` en hijos MDI y alterna el maximizado propio, sin parpadeo (nunca pasa por `WindowState = Maximized`). Mientras esta maximizado se ignoran `SC_MOVE` y `SC_SIZE` (como una ventana maximizada nativa).

Icono: `KryptonForm.CheckViewLayout` elige FormMax/FormRestore segun `WindowState`, que queda en `Normal`. Se fuerza el icono de restaurar cargando `ButtonSpecMax.ImageStates` con las imagenes `FormRestore` de la paleta mientras dura el maximizado, y se vacian al restaurar. Verificado con el tour de capturas (Asistente, Backup, Balance, Dashboard).

Limite conocido: un `WindowState = Maximized` asignado por codigo, o `ShowWindow(SW_MAXIMIZE)` (por ejemplo, el `WindowPattern` de UI Automation), no pasa por `WM_SYSCOMMAND` y maximiza en forma nativa. Ningun codigo de la app lo hace; los tests deben usar el boton o el doble clic en la barra.
