## 1. Fundaciones

- [x] 1.1 `FormTraducibleBase.OnLoad`: asignar `MinimumSize = Size` a los formularios redimensionables sin minimo propio, antes de `base.OnLoad` (excluye `FormPrincipal`)
- [x] 1.2 Spike: determinar como llega el pedido de maximizar en un hijo MDI `KryptonForm` (`WM_SYSCOMMAND/SC_MAXIMIZE` en `WndProc` o cambio de `WindowState`) y como se comporta el icono maximizar/restaurar; registrar el resultado en `design.md`
- [x] 1.3 `FormTraducibleBase`: maximizar de hijos MDI dentro del area de trabajo (guardar/restaurar limites, seguir `MdiClient.Resize`); migrar Balance y Dashboard a este mecanismo y quitar su codigo de tamaño en `Load`
- [x] 1.4 Helper de UI para bloque con ancho maximo centrado (`LayoutHelper.CentrarConAnchoMaximo`)
- [x] 1.5 Crear el test temporal de capturas `tests/TpIDS.UiTests/ZZ_TourRedimensionTests.cs`: cada formulario a tamaño de diseño y agrandado ~50% (`TransformPattern.Resize`), y captura al intentar achicarlo por debajo del minimo
- [x] 1.6 Correr el tour sobre el codigo actual y guardar las capturas a tamaño de diseño como linea base (comparar despues que el tamaño inicial no cambio)

## 2. Lote A: formularios simples (solo anclajes)

- [x] 2.1 Login, Registro y `PasswordTextBox`: borde `Sizable`, contenido en un panel que crece hasta el doble de ancho y despues queda centrado (helper 1.4); campos `Top, Left, Right` dentro del panel
- [x] 2.2 ConfiguracionUsuario, ContingenciaIntegridad y SeleccionarTarjetaDashboard: borde `Sizable`, lista/contenido estirable, Aceptar/Cancelar `Bottom, Right`
- [x] 2.3 SeleccionarUbicacionComercio (mapa `Top, Bottom, Left, Right`) y AsistentePersonal (historial estirable, mensaje y Enviar `Bottom`)
- [x] 2.4 Backup y DashboardAnaliticas (completar los anclajes existentes)
- [x] 2.5 Compilar `TpIDS.sln`, unit + integracion, tour y revision de capturas

## 3. Lote B: una grilla y un grupo

- [x] 3.1 GestionCategorias, ListaDeCompras, GestionEntidadesFinancieras y GestionMediosPago
- [x] 3.2 GestionNotificaciones, GestionCuentasFinancieras y ReglasCategorizacion
- [x] 3.3 Bitacora y BalanceGeneral (filtros con varios campos por fila en `TableLayoutPanel`; paginacion `Bottom, Left`)
- [x] 3.4 DetalleEntidadFinanciera
- [x] 3.5 Revisar `TabIndex` de los formularios del lote; compilar, unit + integracion, tour y revision de capturas

## 4. Lote C: varias zonas de datos o muchos campos

- [x] 4.1 GestionPermisos y GestionUsuarios (dos zonas lado a lado en `TableLayoutPanel` 50/50)
- [x] 4.2 GestionComercios, GestionMonedas y DetalleComercio (dos o tres grillas que crecen en proporcion)
- [x] 4.3 HistorialUsuario y GestionIdiomas
- [x] 4.4 Movimientos (filtros arriba, grilla al medio, grupo de edicion `Bottom, Left, Right` con filas de campos en `TableLayoutPanel`)
- [x] 4.5 Revisar `TabIndex`; abrir un formulario del lote en el designer de Visual Studio y confirmar que no se reescribe mal; compilar, unit + integracion, tour y revision de capturas. Movimientos abre y se ve bien en el designer; al editarlo con el mouse la grilla quedo dentro de `pnlEscaneoTicket` (que la tapa en diseño) y se perdieron sus columnas, asi que se restauro el designer: en ese form conviene editar a mano o cuidando no soltar controles sobre el panel

## 5. Tests y verificacion final

- [x] 5.1 Tests UI FlaUI de redimension: GestionComercios (la grilla crece al agrandar, el minimo se respeta y maximizar ocupa el area de trabajo con barra propia; restaurar vuelve al tamaño previo) y un dialogo (botones quedan abajo a la derecha)
- [x] 5.2 Avisar al usuario, esperar su confirmacion y correr la suite UI completa (`run-tests.ps1 -IncludeIntegration -IncludeUi`)
- [x] 5.3 Revision visual con el usuario (capturas a tamaño de diseño, agrandadas y maximizadas)

## 6. Documentacion y limpieza

- [x] 6.1 Agregar las reglas de anclaje (tabla de `design.md`) a `.agents/01-stack-dotnet-winforms.md` como convencion para formularios nuevos
- [x] 6.2 Actualizar `ESTADO-ACTUAL-Y-PROXIMOS-PASOS.md`
- [x] 6.3 Borrar `tests/TpIDS.UiTests/ZZ_TourRedimensionTests.cs`
