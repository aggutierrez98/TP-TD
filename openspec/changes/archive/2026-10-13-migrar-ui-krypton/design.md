## Context

UI WinForms sobre .NET Framework 4.7.2: 28 formularios (todos heredan de `FormTraducibleBase`), un unico contenedor MDI (`FormPrincipal`, con `MenuStrip` de 26 items), ~340 controles estandar (Label 111, Button 89, TextBox 37, GroupBox 30, DataGridView 22, ComboBox 20, DateTimePicker 7, CheckBox 7, otros). Casi no hay colores hardcodeados; `Font("Segoe UI")` aparece en unos 23 designers. Los tests FlaUI buscan por `AutomationId` (= `Name`) y usan patrones UIA especificos (ValuePattern, SelectionItem, Toggle, clicks de mouse reales).

Fuente de la identidad visual: presentacion "Finanzapp" (fondo `#FFFFFF`, verde `#5DC47D` / `#4CDB81`, texto `#3E3A4E`, gris-violaceo `#48465F`).

## Goals / Non-Goals

**Goals:**
- Aspecto coherente con la presentacion, en modo claro, aplicado desde un unico punto.
- Mantener verde la suite UI y la compilacion en cualquier maquina de laboratorio.

**Non-Goals:**
- Modo oscuro, acentos de graficos, fuentes Anton/Poppins, cambios de logica de negocio.

## Decisions

1. **Krypton con `PaletteMode.Custom`.** Ningun tema built-in es verde (las familias son Professional, Office 2007/2010/2013, Microsoft 365, Sparkle azul/naranja/violeta, Visual Studio 2010, Material). Alternativas descartadas: ReaLTaiizor (pide .NET Framework 4.8+), MaterialSkin2Framework (obliga a heredar de `MaterialForm`, poca actividad), AltUI (sin NuGet, proyecto chico).
2. **Version de `Krypton.Toolkit` fija** (linea 100.x/105.x). La API cambio entre mayores (`GlobalPaletteMode` → `GlobalPalette` en V90) y V110 mueve los temas custom a `ThemeManager`.
3. **Init del tema en `Program.cs`** justo despues de `SetCompatibleTextRenderingDefault(false)` (linea 21) y antes de `InicializadorAplicacionBLL.Inicializar()`.
4. **`FormTraducibleBase : KryptonForm`** como unico punto de herencia.
5. **Todo Krypton; nativo solo por excepcion documentada** (decision del usuario tras ver el primer resultado visual, que mezclaba controles nativos con marco Krypton y se veia peor que un formulario nativo). Se migran tambien ComboBox, CheckBox, DateTimePicker y DataGridView y se adaptan los helpers de `UiFind`. Antes de migrar, un spike prueba estrategias de interaccion (ValuePattern, clic real, teclado) para cada control; si alguno no se puede automatizar de forma fiable queda nativo y tematizado con un helper en `UI/Tema/`. El `MenuStrip` sigue siendo el nativo con el renderer de Krypton (`ClickMenu` depende de su arbol UIA).
8. **Marco de ventana.** El relleno del marco (`FormMain.StateActive.Back`) en verde se veia como un borde grueso; se usa blanco con borde verde de 1 px (`UseThemeFormChromeBorderWidth`). Menu con seleccion verde suave (`ToolMenuStatus`) y area del `MdiClient` en `#F3F4F8`. Verificado en una sonda, pendiente de verlo en la app real.
6. **Segoe UI como fuente base.** Una fuente no instalada cae en silencio a una por defecto en tiempo de ejecucion; Segoe UI viene con Windows. Embeber fuentes con `PrivateFontCollection` queda como prueba opcional (riesgo no verificado con el render GDI de `TextRenderer`).
7. **Migracion por puerta de decision**: la prueba de concepto sobre `FormGestionCategorias` define, control por control, si se migra o se deja nativo.

## Risks / Trade-offs

- [UIA no documentado en Krypton] → la Fase 0 verifica con FlaUInspect y el test existente antes de migrar nada; los controles incompatibles quedan nativos.
- [Cambios de API entre versiones mayores] → version fija en NuGet; la paleta vive en un solo archivo de `UI/Tema/`.
- [Formularios MDI: `MdiClient` tocado por `FormBalanceGeneral`/`FormDashboardAnaliticas`] → probar `FormPrincipal` como `KryptonForm` MDI en la Fase 1.
- [Texto dentado con `SetCompatibleTextRenderingDefault(true)` (issue #1109 de Krypton)] → el proyecto ya usa `false`.
- [Desviacion visual respecto de la presentacion] → revision manual de cada formulario contra la presentacion.
- [Helpers de `UiFind` no adaptables a algun control Krypton] → el spike lo detecta antes de migrar; ese control queda nativo y tematizado, documentado como excepcion.
- [`IdiomaFormHelper` usa `control is ComboBox`/`DataGridView`] → un `KryptonComboBox` no es un `ComboBox`: se extiende el helper y se revisan las demas comprobaciones de tipo.
- [Migrar ~340 controles de golpe] → migracion por lotes con la suite en verde al cierre de cada uno; el codigo de los forms (`SelectedItem`, `DataSource`, etc.) se revisa por lote.

## Migration Plan

Fase 0 (prueba de concepto) → Fase 1 (cimientos del tema, marco, menu, MDI) → **Spike de automatizacion y adaptacion de `UiFind`/`IdiomaFormHelper`** → tematizado de las excepciones nativas → migracion por lotes de todos los controles (dialogos pequeños, MDI de gestion, Dashboard/Balance/Mapa) → verificacion final → documentacion. Reversion: el cambio es por lotes y cada lote cierra con la suite en verde; se puede revertir un lote sin afectar a los demas.

## Resultado de la prueba de concepto UIA (tarea 1.3)

Sonda temporal (fuera del repo) que aloja cada control Krypton y su equivalente nativo con el mismo `Name` y les aplica los helpers reales de `tests/TpIDS.UiTests/Infrastructure/UiFind.cs` (FlaUI.UIA3 4.0.0, Krypton.Toolkit 105.26.7.201, net472). Resultado identico con host `Form` normal y con host `KryptonForm`. Los controles nativos pasaron todos los helpers (linea base valida).

| Control | AutomationId | ControlType / patrones UIA (Krypton) | Helpers de `UiFind` actuales | Decision inicial (superada por la decision 5 revisada) |
|---|---|---|---|---|
| Button | OK | Pane, sin Invoke | `Click` (mouse real) OK | Krypton |
| TextBox | OK | Edit + Value | `EscribirTexto`/`LeerTexto` OK | Krypton |
| GroupBox | OK (hijos en `Panel` se encuentran) | Pane | `Click` sobre hijo OK | Krypton |
| Label | OK | Pane (nativo: Text) | no se uso `UiFind` para leer; sin verificar lectura de texto | Krypton (a verificar al migrar) |
| ListBox | OK | List + Selection | `SeleccionarItemLista` OK | Krypton posible (el plan lo dejaba nativo) |
| ComboBox | OK | ComboBox + Value, sin ExpandCollapse | `SeleccionarItemCombo` y `...ConTeclado` fallan (NullReference) | **Nativo + paleta** |
| CheckBox | OK | Pane, sin Toggle | `MarcarCheckBox` falla (no hay Toggle) | **Nativo + paleta** |
| DateTimePicker | OK | Pane sin patrones | `EstablecerFecha`/`LeerFecha` fallan | **Nativo + paleta** |
| DataGridView | OK | Table + Grid | `ContarFilasGrilla` y `ClickFilaGrilla` OK; `SeleccionarFilaGrilla` (SelectionItem) no selecciona | **Nativo + paleta** |
| TreeView, CheckedListBox, MenuStrip | no probados | — | — | Nativo + paleta (sin cambios respecto del plan) |

Limitaciones de la prueba: es una sonda aislada, no la app real. No cubre `IdiomaFormHelper` (usa `control is ComboBox`/`DataGridView`, que no aplica a `KryptonComboBox`), ni lectura de `Name` de labels, ni `IsEnabled` de botones. Los helpers que fallan con ComboBox/CheckBox/DateTimePicker/DataGridView de Krypton se van a adaptar (decision 5 revisada), pero **no se verifico que sea viable**: el spike (tareas 3.x) lo determina.

## Resultado de la prueba de fuentes embebidas (tarea 1.5)

Fuentes OFL de Google Fonts (Anton-Regular y Poppins-Regular, licencia SIL OFL 1.1) cargadas desde bytes en memoria (emulando un recurso embebido) y aplicadas a controles nativos y Krypton, capturando el resultado a PNG.

- `PrivateFontCollection` + `AddMemoryFont`: la fuente **solo se aplica con render GDI+** (`UseCompatibleTextRendering = true`) en un `Label` nativo. Con el render GDI de la app (`SetCompatibleTextRenderingDefault(false)`) cae a la fuente por defecto en `Label`, `Button` y `TextBox` nativos, y tambien en `KryptonLabel`, `KryptonButton` y `KryptonTextBox`. En `KryptonLabel` ademas el texto se recorta (se mide con una fuente y se dibuja con otra).
- `AddFontMemResourceEx` (registro solo para el proceso) + `new Font("Anton", ...)` por nombre: la API devuelve handles validos (1 fuente cada una), pero ninguna fuente se aplica en ningun control. No se diagnostico la causa.
- `KryptonManager.BaseFont` con la fuente embebida: no concluyente.

**Decision:** se descartan Anton/Poppins embebidas; la fuente base sigue siendo Segoe UI. Reabrir solo si se encuentra un metodo que funcione con GDI y Krypton, o si se acepta instalar las fuentes en cada maquina (riesgo en el laboratorio).

## Resultado del spike de automatizacion (tarea 3.1)

Sonda temporal (fuera del repo) con FlaUI.UIA3 4.0.0 y Krypton.Toolkit 105.26.7.201, dos pasadas. Hallazgo estructural: `KryptonComboBox` envuelve un `ComboBox` nativo como hijo UIA (con `ExpandCollapse`) y, en modo editable, un `Edit`; `KryptonCheckBox` y `KryptonDateTimePicker` son un `Pane` sin hijos, solo con `LegacyIAccessible`; `KryptonDataGridView` expone `Table` + `Grid`, con filas sin `SelectionItem` y celdas `Edit` con `Value`.

| Control | Escribir / accionar | Leer estado | Veredicto |
|---|---|---|---|
| ComboBox | OK: logica nativa del helper sobre el ComboBox interno (`Expand` + `Items` + `Select`), o teclado (Focus + HOME + flecha abajo), que ademas dispara `SelectedIndexChanged`. `ValuePattern.SetValue` no funciona | Se lee `Text` del control en la app | **Migrable** adaptando el helper para usar el hijo ComboBox |
| CheckBox | OK: clic real (o Focus + barra espaciadora). Sin subclase no existen `Invoke`/`Toggle` | Sin subclase: `LegacyIAccessible.State` no incluye CHECKED | **Migrable con subclase accesible** (ver 3.3): el helper `MarcarCheckBox` funciona sin cambios |
| DateTimePicker | OK: clic en el segmento del dia + digitos con RIGHT entre segmentos ('15', '01', '2026' → 15/01/2026); flecha arriba tambien cambia el valor. `SetValue` y `AsDateTimePicker().SelectedDate` fallan | Sin subclase: `Legacy.Value` vacio | **Migrable con subclase accesible + escritura por teclado en el helper** (ver 3.3) |
| DataGridView | OK: clic sobre una **celda** de la fila (el clic sobre el elemento fila no selecciona) o clic en la primera fila + flecha abajo. Contar filas por `Grid` funciona | Fila actual leible en la app | **Migrable** adaptando `SeleccionarFilaGrilla`/`ClickFilaGrilla` |

Advertencia sobre la grilla: Krypton nombra las filas con base 0 (`'Fila 1'` es la segunda) y cada fila tiene un hijo `Header` con ese nombre. El helper actual compara `celda.Name == texto`, asi que puede elegir la fila equivocada; hay que comparar por el `Value` de las celdas `Edit`.

Hipotesis para los dos pendientes (no verificada): subclases propias en `UI/Tema/` (por ejemplo para `KryptonCheckBox` y `KryptonDateTimePicker`) que sobrescriban `CreateAccessibilityInstance()` con un `AccessibleObject` que exponga `Role`, `State` (Checked) y `Value`. Mejoraria la accesibilidad real y permitiria que los helpers lean el estado sin cambiar su logica. Si no funciona o no se acepta, esos dos controles quedan nativos y tematizados (excepcion documentada). Usos actuales en tests: `MarcarCheckBox` x5, `EstablecerFecha` x2, `LeerFecha` x2.

### Prototipo de subclases accesibles (tarea 3.3)

Prototipo en la sonda, validado con los helpers reales de `UiFind`:

- `CheckBoxAccesible : KryptonCheckBox` sobrescribe `CreateAccessibilityInstance()` con un `ControlAccessibleObject` que expone `Role = CheckButton`, `State` (Checked), `DefaultAction` y `DoDefaultAction`. UIA lo ve como `CheckBox` con `Invoke` + `Toggle`. `UiFind.MarcarCheckBox` funciona **sin modificar** y es idempotente (marcar dos veces deja marcado, desmarcar dos veces deja desmarcado).
- `DateTimePickerAccesible : KryptonDateTimePicker` expone `Role = DropList` y `Value` (get/set, formato corto de la cultura actual). `UiFind.LeerFecha` funciona **sin modificar** (lee `09/03/2024` tras un cambio hecho desde la app). `UiFind.EstablecerFecha` sigue fallando ("Unable to set the selected date"): se resuelve adaptando el helper para escribir por teclado (clic en el segmento del dia y digitos con RIGHT), que ya se verifico en el spike. Limite: el orden de los segmentos depende de la cultura de la maquina (dd/MM/yyyy en es-AR).
- Efecto colateral positivo: mejor accesibilidad real (lectores de pantalla) para esos dos controles.

**Conclusion:** los cuatro controles son migrables. ComboBox (hijo ComboBox interno) y DataGridView (clic en celda, busqueda por `Value`) solo requieren adaptar `UiFind`; CheckBox y DateTimePicker ademas requieren las subclases accesibles en `UI/Tema/`. Falta verificar la idea en la app real (no en una sonda aislada) al migrar el primer formulario.

### Conformidad de los helpers adaptados (tareas 3.2, 3.4 y 3.5)

Sonda de conformidad que ejecuta los helpers **reales** de `UiFind` (ya adaptados) y las subclases reales de `UI/Tema/` contra la version nativa y la Krypton de cada control: combo (por item y por teclado), checkbox (marcar, idempotencia, desmarcar), fecha (escribir y leer, incluso tras un cambio hecho desde la app) y grilla (esperar/contar filas, seleccionar, clickear y deseleccionar por texto, con el caso de filas con base 0). Resultado: **14/14 con controles nativos y 14/14 con Krypton**. Ejecutada en una maquina con cultura es-AR; el orden de los segmentos de la fecha se deriva de `ShortDatePattern`.

**Decision por control (3.5):** los cuatro se migran a Krypton (ComboBox, CheckBox, DateTimePicker, DataGridView). Quedan nativos y tematizados solo TreeView y CheckedListBox (no se probaron) y el `MenuStrip`. `IdiomaFormHelper` ya reconoce `KryptonComboBox`; no hay otras comprobaciones de tipo sobre controles nativos en el codigo de los formularios (solo el uso de `MdiClient` en Balance/Dashboard).

Pendiente: confirmarlo en la app real al migrar el primer formulario con combos/fechas/checks (los formularios que hoy usan Krypton no tienen ninguno de esos controles).

### Aprendizajes del Lote A (migracion de designers)

Herramienta: `.agents/scripts/migrar-designer-krypton.py` (dry-run por defecto; `--escribir`; `--solo-etiquetas`; `--solo-estilo-boton`). Migra tipos, `Values.Text`/`Values.Heading`, fuentes, grupos con `Panel` y quita propiedades que Krypton no tiene; conserva `Name`, `Tag` y handlers. Los 8 designers del lote compilaron sin errores tras la migracion; los ajustes visuales salieron de capturas reales de la app (test temporal `ZZ_TourVisualTests`, no forma parte de la suite).

- **Boton verde = `ButtonStyle.Custom1`, no `ButtonStandalone`.** Fijar texto blanco en `ButtonStandalone` hacia desaparecer la flecha de los `KryptonComboBox`/fechas (heredan de ese estilo y no basto sobrescribir `ButtonInputControl` ni `ButtonButtonSpec`). La herramienta asigna `ButtonStyle = Custom1` a cada `KryptonButton`; un boton sin ese estilo sale con el aspecto base de Microsoft 365.
- **`KryptonLabel` es mas alto que `Label`:** su fondo pisaba el borde superior del campo de abajo. La herramienta sube las etiquetas 4 px (salvo las de titulo).
- **`KryptonTextBox` necesita ~24 px de alto:** `PasswordTextBox` (UserControl de 20 px) recortaba el borde superior; se llevo a 24.
- **`KryptonGroupBox`:** los hijos van al `Panel` y su Y baja 16 px (el encabezado ya esta fuera del panel).
- **Grillas:** la paleta define `GridStyles.GridList` (encabezado verde con texto blanco, seleccion verde suave) para que `KryptonDataGridView` y la grilla nativa tematizada se vean igual.
- **Marco/area MDI/menu:** confirmados en la app real (marco blanco con borde de 1 px, area MDI `#F3F4F8`, menu con seleccion verde suave).

Cierre del lote: `TpIDS.sln` compila, unit 383/383, integracion 121/121 y suite UI completa 62/62 (61 de la suite mas el recorrido temporal). Sin verificar visualmente: `FormHistorialUsuario`, `FormContingenciaIntegridad` y `FormSeleccionarTarjetaDashboard` (migrados y cubiertos solo por compilacion y suite). Detalle menor visto en `FormDetalleComercio`: la grilla de promociones queda con un marco gris a los lados dentro de su grupo.

### Aprendizajes del Lote B

Los 12 designers del lote migraron con la herramienta; el usuario detecto en la app real un crash al abrir Bitacora y desalineaciones visibles en las capturas.

- **`FormattingEnabled` no se quita.** `KryptonComboBox`, `KryptonListBox` y `KryptonCheckedListBox` la tienen. Sin ella, `ComboBox.Text` hace `ToString()` sobre el valor de `DisplayMember` y tira `NullReferenceException` si es `null` (Bitacora: `OpcionTraducible.Texto` arranca vacio). Restaurada en 17 controles de los lotes A y B; la herramienta ya no la elimina.
- **Hijos nativos de un grupo tambien suben 16 px.** La herramienta solo corregia la Y de los controles que migraba; TreeView, CheckedListBox y NumericUpDown quedaban desplazados (el arbol de Usuarios tapaba su boton). Corregido en la herramienta y en 6 controles.
- **`KryptonListBox` no tiene `IntegralHeight`** (la herramienta la quita) y los metodos privados con parametros `ComboBox`/`ListBox` pasan a los tipos Krypton.
- **`KryptonButton` es `Pane` en UIA:** un test que inventariaba botones por `ControlType.Button` dejo de encontrarlos; ahora busca por `AutomationId`.
- **NumericUpDown queda nativo** (no estaba en el plan), tematizado por `TemaFinanzapp`.
- Defectos previos que el tema dejo a la vista y se corrigieron: layout de Idiomas, etiqueta de Monedas, botones de Backup y la traduccion `FormLanguageManagement.MakeDefault` ("Eliminar idioma" en el boton de marcar default).
- **Hallazgo (corregido):** el `MenuStrip` principal no se traducia en runtime (`IdiomaFormHelper` solo recorria `Controls`, no los `ToolStripItem`); al cambiar a ingles el menu seguia en espanol. `IdiomaFormHelper` ahora recorre los items de cualquier `ToolStrip` y sus `DropDownItems`.

Cierre: `TpIDS.sln` compila, unit 383/383, integracion 121/121, suite UI 61/62 y el test fallido (`FIN_CuentasFinancieras_Pantalla_SoloPermiteAltaYBaja`) en verde tras adaptarlo, corrido solo con `-UiFilter` a pedido del usuario. Los ajustes de Monedas/Backup posteriores a la suite solo tocaron posiciones y tamanos.

### Aprendizajes del Lote C

Movimientos, Lista de compras y Asistente migraron con la herramienta y compilaron tras cambiar dos helpers privados de `FormMovimientos` (`ComboBox` -> `KryptonComboBox`).

- **Filas de inputs apiladas:** `KryptonComboBox` mide ~25 px; con etiquetas de la fila siguiente a 22 px del input de arriba, el fondo de la `KryptonLabel` tapaba el borde inferior de los combos. Se bajo 8 px la segunda fila y los botones del grupo de edicion de Movimientos.
- **`RichTextBox` queda nativo** (historial del Asistente), tematizado por `TemaFinanzapp`. Ignora `BorderStyle.FixedSingle` con estilos visuales (sigue el borde 3D) y `KryptonForm` no dispara `Paint` en su area cliente, asi que el helper lo envuelve en un `Panel` verde con `Padding` de 1 px y le quita el borde nativo. `Tematizar` itera una copia de `Controls` porque ahora puede reubicar controles.

Cierre: `TpIDS.sln` compila, unit 383/383, integracion 121/121 y tour visual sin errores. Suite UI completa pendiente.

### Aprendizajes del Lote D

Balance, Dashboard y Seleccionar ubicacion migraron con la herramienta (Mapa = `FormSeleccionarUbicacionComercio`, unico form con GMap; el `GMapControl` queda nativo). Ningun test UI cubre estas pantallas: se verificaron con el tour (tarjetas agregadas desde el dialogo y movimientos sembrados por SQL).

- **Tarjetas del Dashboard armadas por codigo:** titulo `KryptonLabel` (tamaño fijo: con `AutoSize` se mide con la fuente por defecto y recorta el titulo en negrita), boton `KryptonButton` `Custom1`, panel y grafico con fondo de la paleta (se crean despues de `Tematizar`, asi que no lo heredan).
- **Colores de LiveCharts:** Ingresos `Verde`, Egresos `GrisVioleta`, tortas rotando tonos de la paleta (`SolidColorPaint`); los acentos naranja/azul siguen fuera de alcance.
- **`TextAlign` de `Label`** no existe en `KryptonLabel`: equivale a `StateCommon.ShortText.TextH` (la herramienta lo marca para revision manual).
- **Etiquetas en negrita** no se suben 4 px (la herramienta las trata como titulos): los montos del Balance quedaban desalineados con su rotulo.
- **Items de `KryptonListBox` seleccionados en amarillo** (base Microsoft 365): la paleta define `ButtonStyles.ButtonListItem` (verde suave con borde verde). Afectaba al dialogo de tarjetas y a las listas del Lote A/B.
- **`dgvCategorias` del piloto** seguia nativa (decision previa a la revision de la decision 5): migrada a `KryptonDataGridView`.
- **2.4:** Balance y Dashboard abren ocupando el `MdiClient` con su propia barra de titulo; sin cambios.

Cierre: `TpIDS.sln` compila, unit 383/383, integracion 121/121 y tour sin errores. Suite UI completa pendiente (6.1).

### Arranque: redireccion de `System.Resources.Extensions` (tarea 2.9)

Regresion detectada por el usuario al ejecutar desde Visual Studio: `TypeInitializationException` en `KryptonManager` con `FileLoadException` por `System.Resources.Extensions, Version=4.0.0.0` (el paquete trae la 9.0.0.10). La suite UI no lo veia porque el build de `tests\TpIDS.Tests.sln` genero la redireccion en su `TpIDS.exe.config` y el build del proyecto UI desde Visual Studio no. Causa: `AutoGenerateBindingRedirects` no es determinista entre formas de compilar para esta dependencia (Krypton la usa para leer recursos y no figura como referencia directa). Solucion: redireccion explicita en `UI/App.config`. Leccion: verificar siempre el arranque real de la app compilada como lo hace el usuario (Visual Studio), no solo a traves de la suite; y para la maquina del laboratorio no depender de redirecciones generadas.

## Open Questions

- Medicion precisa de los acentos naranja/azul de los graficos (fuera de alcance por ahora).
- (Resuelto) Anton/Poppins embebidas descartadas tras la prueba de la tarea 1.5.
