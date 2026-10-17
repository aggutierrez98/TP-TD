## 1. Prueba de concepto (puerta de decision)

- [x] 1.1 Agregar `Krypton.Toolkit` con version fija a `UI/UI.csproj`
- [x] 1.2 Migrar `FormGestionCategorias` a controles Krypton como piloto
- [x] 1.3 Verificar con FlaUInspect y el test FlaUI existente `AutomationId`, `ControlType` y patrones UIA de KryptonButton, KryptonTextBox, KryptonComboBox, KryptonCheckBox, KryptonDateTimePicker y KryptonDataGridView (hecho con una sonda temporal que aplica los helpers de `UiFind`; no existe test FlaUI de Categorias)
- [x] 1.4 Registrar la tabla "control → Krypton / nativo con paleta" en `design.md`
- [x] 1.6 Agregar tests UI FlaUI de `FormGestionCategorias` (no existian) para cubrir el piloto: alta, nombre vacio, nombre duplicado, actualizacion, baja y categoria global no editable
- [x] 1.5 (Opcional) Probar fuentes embebidas con `PrivateFontCollection` y decidir si se descartan (descartadas: ver `design.md`)

## 2. Cimientos del tema

- [x] 2.1 Crear `UI/Tema/` con la paleta Finanzapp (`KryptonCustomPaletteBase`) y opcionalmente `finanzapp.palette.xml`
- [x] 2.2 Inicializar `KryptonManager` y la paleta global en `UI/Program.cs` tras `SetCompatibleTextRenderingDefault(false)`
- [x] 2.3 Cambiar `FormTraducibleBase` para heredar de `KryptonForm`
- [x] 2.4 Verificar `FormPrincipal` como contenedor MDI con `KryptonForm` y el `MdiClient` de Balance/Dashboard
- [x] 2.5 Configurar el renderer de Krypton para `menuStripPrincipal` (menu desplegado visto en captura real del tour, Lote B)
- [x] 2.6 Fijar Segoe UI como fuente base
- [x] 2.7 Tematizar los controles nativos hijos (Label, CheckBox, RadioButton, GroupBox, paneles) con un helper invocado desde `FormTraducibleBase`: `KryptonForm` ignora `BackColor`/`ForeColor`, asi que los nativos heredan el gris del sistema sobre el form blanco
- [x] 2.8 Pulido visual tras la primera revision del usuario (el resultado inicial se veia "horrible"): marco de ventana blanco con borde verde fino (sin el marco verde grueso), menu y fondo del area MDI tematizados. Verificado solo en una sonda; falta verlo en la app real
- [x] 2.9 Declarar en `UI/App.config` la redireccion de `System.Resources.Extensions` (0.0.0.0-9.0.0.10 → 9.0.0.10): la generacion automatica la omite en algunos builds (por ejemplo desde Visual Studio) y la app no arrancaba (`FileLoadException` al crear el `KryptonManager`). Verificado: compilando con MSBuild queda una sola redireccion en `bin` y la app arranca; el usuario confirmo el arranque desde Visual Studio

## 3. Spike de automatizacion y adaptacion de tests UI

- [x] 3.1 Spike: sonda temporal que prueba estrategias de interaccion (ValuePattern, clic real, teclado) para KryptonComboBox, KryptonCheckBox, KryptonDateTimePicker y KryptonDataGridView, y registra el resultado en `design.md`
- [x] 3.2 Adaptar `UiFind` (`SeleccionarItemCombo`, `SeleccionarItemComboConTeclado`, `MarcarCheckBox`, `EstablecerFecha`/`LeerFecha`, `SeleccionarFilaGrilla`) para operar tambien los controles Krypton sin romper los nativos, y crear en `UI/Tema/` las subclases `CheckBoxAccesible` y `DateTimePickerAccesible` validadas en 3.3
- [x] 3.3 Prototipar en la sonda subclases con `AccessibleObject` (CheckBox y DateTimePicker) que expongan estado y valor por UIA y validarlas con los helpers de `UiFind`; si funcionan, llevarlas a `UI/Tema/`; si no, esos dos controles quedan nativos y tematizados (excepcion documentada)
- [x] 3.4 Extender `IdiomaFormHelper` para `KryptonComboBox` (opciones traducibles) y revisar las demas comprobaciones `is ComboBox`/`is DataGridView`
- [x] 3.5 Decidir por control: migrar o dejar nativo tematizado (excepcion documentada) y registrarlo en `design.md`

## 4. Controles nativos tematizados (solo excepciones)

- [x] 4.1 Helper en `UI/Tema/` para colores de los controles que queden nativos (TreeView, CheckedListBox y los que el spike declare no automatizables): `TemaFinanzapp.TematizarControl` cubre TreeView, ListBox/CheckedListBox, NumericUpDown y RichTextBox (Lote C)
- [x] 4.2 Aplicar el helper en los formularios que usan esos controles (automatico: lo invoca `FormTraducibleBase`)

## 5. Migracion por lotes (todos los controles)

- [x] 5.1 Lote A: dialogos pequeños (Login, Registro, detalle de Comercio/Entidad, etc.)
- [x] 5.2 Lote B: MDI de gestion (Categorias, Monedas, Comercios, Medios de Pago, Entidades, Cuentas, Usuarios, Permisos, Idiomas, etc.)
- [x] 5.3 Lote C: Movimientos, Lista de Compras, Asistente Personal
- [x] 5.4 Lote D: Dashboard y Balance (LiveCharts) y Mapa (GMap), con colores de la paleta
- [x] 5.5 Tras cada lote: compilar `TpIDS.sln` y correr `tests/tools/run-tests.ps1 -IncludeIntegration`; revisar el codigo del form por el cambio de tipo de los controles

## 6. Verificacion final

- [x] 6.1 Avisar al usuario, esperar su confirmacion y correr la suite UI FlaUI completa (sin tocar mouse ni teclado durante la corrida)
- [x] 6.3 Verificar que la app arranca compilada desde Visual Studio (F5) y desde MSBuild, y en una maquina limpia (la del laboratorio) sin depender de redirecciones generadas por el build

## 7. Documentacion

- [x] 7.1 Actualizar `.agents/skills/winforms-ui-control-naming/references/control-naming.md` con controles Krypton
- [x] 7.2 Actualizar `AGENTS.md`/`.agents/` con la regla del tema
- [x] 7.4 Eliminar los artefactos temporales: `tests/TpIDS.UiTests/ZZ_TourVisualTests.cs` y la sonda fuera del repo (`C:\Users\agust\AppData\Local\Temp\krypton-probe`)
