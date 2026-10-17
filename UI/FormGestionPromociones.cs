using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ABSTRACCIONES;
using BE;
using BLL;
using Krypton.Toolkit;

namespace TpIDS
{
    // Pantalla de operador (back-office): ABM de promociones. Arriba se busca por texto y por
    // comercio, en el medio va el listado y abajo el editor con todos los filtros de alcance.
    // Todos los filtros son opcionales: una lista sin marcar no restringe (aplica a todos). En los
    // productos y categorias excluidos la exclusion gana (la regla vive en el BLL).
    public partial class FormGestionPromociones : FormTraducibleBase
    {
        private readonly PromocionBLL _promocionBLL = CompositionRoot.Promocion;
        private readonly ComercioBLL _comercioBLL = CompositionRoot.Comercio;
        private readonly RubroComercioBLL _rubroComercioBLL = CompositionRoot.RubroComercio;
        private readonly EntidadFinancieraBLL _entidadFinancieraBLL = CompositionRoot.EntidadFinanciera;
        private readonly ProductoBLL _productoBLL = CompositionRoot.Producto;
        private readonly CategoriaProductoBLL _categoriaProductoBLL = CompositionRoot.CategoriaProducto;

        private readonly Dictionary<DiaSemana, CheckBoxAccesible> _checksDias;

        // Productos del catalogo: para avisar si un producto incluido cae en una categoria excluida.
        private List<Producto> _productos = new List<Producto>();

        // Evita recalcular la advertencia por cada item que se marca al cargar el editor.
        private bool _cargandoEditor;

        public FormGestionPromociones()
        {
            InitializeComponent();
            _checksDias = new Dictionary<DiaSemana, CheckBoxAccesible>
            {
                { DiaSemana.Lunes, chkbDiaLunes },
                { DiaSemana.Martes, chkbDiaMartes },
                { DiaSemana.Miercoles, chkbDiaMiercoles },
                { DiaSemana.Jueves, chkbDiaJueves },
                { DiaSemana.Viernes, chkbDiaViernes },
                { DiaSemana.Sabado, chkbDiaSabado },
                { DiaSemana.Domingo, chkbDiaDomingo }
            };
            Load += FormGestionPromociones_Load;
        }

        // View-model plano para bindear la grilla. Descuento, tope, dias y fechas se formatean en
        // CellFormatting para que los dias se re-traduzcan solos al cambiar el idioma.
        private sealed class FilaPromocion
        {
            public int Id { get; set; }
            public string Descripcion { get; set; }
            public decimal Descuento { get; set; }
            public decimal? TopeDescuento { get; set; }
            public List<DiaSemana> DiasSemana { get; set; }
            public DateTime VigenciaDesde { get; set; }
            public DateTime VigenciaHasta { get; set; }
            public Promocion Promocion { get; set; }
        }

        // Item de las listas de seleccion: el texto se arma al mostrarse, asi un cambio de idioma
        // en caliente solo requiere volver a agregar los items.
        private sealed class OpcionFiltro
        {
            private readonly Func<string> _texto;

            public OpcionFiltro(int id, string valor, Func<string> texto)
            {
                Id = id;
                Valor = valor;
                _texto = texto;
            }

            public int Id { get; }

            // Solo para segmentos (texto libre sin id).
            public string Valor { get; }

            public override string ToString()
            {
                return _texto();
            }
        }

        private void FormGestionPromociones_Load(object sender, EventArgs e)
        {
            // Tematizar (en OnLoad, antes de este handler) deja los paneles en blanco: el marco de
            // cada lista se colorea recien ahora.
            ColorearMarcosYAvisos();
            CargarCatalogos();
            LimpiarEditor();
            CargarPromociones(null);
        }

        // --- Catalogos ---

        private void CargarCatalogos()
        {
            try
            {
                CargarComboComercios(null);

                CargarLista(clbComercios, _comercioBLL.ObtenerTodos()
                    .OrderBy(item => item.Nombre)
                    .Select(item => new OpcionFiltro(item.Id, null, () => item.Nombre)));

                CargarLista(clbRubrosComercio, _rubroComercioBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .Select(item => new OpcionFiltro(item.Id, null, () => item.Nombre)));

                CargarLista(clbEntidadesFinancieras, _entidadFinancieraBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .Select(item => new OpcionFiltro(item.Id, null, () => item.Nombre)));

                CargarLista(clbMediosPago, _promocionBLL.ObtenerMediosPago()
                    .OrderBy(item => item.Tipo)
                    .ThenBy(item => item.Marca)
                    .Select(item => new OpcionFiltro(item.Id, null, () => DescribirMedioPago(item))));

                CargarLista(clbSegmentos, _promocionBLL.ObtenerSegmentosDisponibles()
                    .OrderBy(item => item, StringComparer.CurrentCultureIgnoreCase)
                    .Select(item => new OpcionFiltro(0, item, () => item)));

                _productos = _productoBLL.ObtenerTodos() ?? new List<Producto>();
                List<Producto> productosOrdenados = _productos.OrderBy(item => item.Nombre).ToList();
                CargarLista(clbProductosIncluidos, productosOrdenados.Select(CrearOpcionProducto));
                CargarLista(clbProductosExcluidos, productosOrdenados.Select(CrearOpcionProducto));

                List<CategoriaProducto> categorias = _categoriaProductoBLL.ObtenerTodas()
                    .OrderBy(item => item.Nombre)
                    .ToList();
                CargarLista(clbCategoriasIncluidas, categorias.Select(item => new OpcionFiltro(item.Id, null, () => item.Nombre)));
                CargarLista(clbCategoriasExcluidas, categorias.Select(item => new OpcionFiltro(item.Id, null, () => item.Nombre)));
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Carga de catalogos de promociones");
            }

            ActualizarAyudasListasVacias();
        }

        private void CargarComboComercios(int? comercioSeleccionadoId)
        {
            List<Comercio> opciones = new List<Comercio>
            {
                new Comercio { Id = 0, Nombre = UiDialogHelper.Traducir("FormPromotionManagement.Value.AllMerchants") }
            };
            opciones.AddRange(_comercioBLL.ObtenerTodos().OrderBy(item => item.Nombre));

            cmbComercioBusquedaPromocion.DataSource = null;
            cmbComercioBusquedaPromocion.DisplayMember = "Nombre";
            cmbComercioBusquedaPromocion.ValueMember = "Id";
            cmbComercioBusquedaPromocion.DataSource = opciones;

            int indice = comercioSeleccionadoId.HasValue
                ? opciones.FindIndex(item => item.Id == comercioSeleccionadoId.Value)
                : 0;
            cmbComercioBusquedaPromocion.SelectedIndex = indice >= 0 ? indice : 0;
        }

        private static OpcionFiltro CrearOpcionProducto(Producto producto)
        {
            return new OpcionFiltro(producto.Id, null, () => producto.CategoriaProducto == null
                ? producto.Nombre
                : string.Format("{0} ({1})", producto.Nombre, producto.CategoriaProducto.Nombre));
        }

        private static string DescribirMedioPago(MedioPago medioPago)
        {
            string texto = UiDialogHelper.Traducir(EtiquetasFinanzas.ClaveTipoMedioPago(medioPago.Tipo));
            return string.IsNullOrWhiteSpace(medioPago.Marca)
                ? texto
                : string.Format("{0} {1}", texto, medioPago.Marca);
        }

        private void ColorearMarcosYAvisos()
        {
            Panel[] marcosNeutros =
            {
                pnlMarcoComercios, pnlMarcoRubrosComercio, pnlMarcoEntidadesFinancieras, pnlMarcoMediosPago,
                pnlMarcoSegmentos, pnlMarcoProductosIncluidos, pnlMarcoCategoriasIncluidas
            };
            foreach (Panel marco in marcosNeutros)
            {
                marco.BackColor = FinanzappPaleta.Verde;
            }

            // Excluidos: marco y rotulo en un rojo apagado para distinguirlos de los incluidos.
            pnlMarcoProductosExcluidos.BackColor = FinanzappPaleta.AlertaSuave;
            pnlMarcoCategoriasExcluidas.BackColor = FinanzappPaleta.AlertaSuave;
            lblFiltroProductosExcluidos.StateCommon.ShortText.Color1 = FinanzappPaleta.Alerta;
            lblFiltroCategoriasExcluidas.StateCommon.ShortText.Color1 = FinanzappPaleta.Alerta;
            lblAdvertenciaCategoriaExcluida.StateCommon.ShortText.Color1 = FinanzappPaleta.Alerta;

            // Ayuda de las listas sin opciones: gris, nunca con el color de alerta.
            foreach (KryptonWrapLabel ayuda in ObtenerAyudasListasVacias().Values)
            {
                ayuda.StateCommon.TextColor = FinanzappPaleta.TextoSecundario;
            }
        }

        // --- Ayuda de listas sin opciones ---

        // Cada lista de filtros con el aviso que se muestra dentro del recuadro mientras no tiene items.
        private Dictionary<CheckedListBox, KryptonWrapLabel> ObtenerAyudasListasVacias()
        {
            return new Dictionary<CheckedListBox, KryptonWrapLabel>
            {
                { clbComercios, lblVacioComercios },
                { clbRubrosComercio, lblVacioRubrosComercio },
                { clbEntidadesFinancieras, lblVacioEntidadesFinancieras },
                { clbMediosPago, lblVacioMediosPago },
                { clbSegmentos, lblVacioSegmentos },
                { clbProductosIncluidos, lblVacioProductosIncluidos },
                { clbProductosExcluidos, lblVacioProductosExcluidos },
                { clbCategoriasIncluidas, lblVacioCategoriasIncluidas },
                { clbCategoriasExcluidas, lblVacioCategoriasExcluidas }
            };
        }

        // Regla unica: la ayuda se ve solo si la lista no tiene ninguna opcion.
        private void ActualizarAyudasListasVacias()
        {
            foreach (KeyValuePair<CheckedListBox, KryptonWrapLabel> par in ObtenerAyudasListasVacias())
            {
                par.Value.Visible = par.Key.Items.Count == 0;
            }
        }

        // --- Listas de seleccion ---

        private static void CargarLista(CheckedListBox lista, IEnumerable<OpcionFiltro> opciones)
        {
            lista.BeginUpdate();
            lista.Items.Clear();
            foreach (OpcionFiltro opcion in opciones)
            {
                lista.Items.Add(opcion, false);
            }

            lista.EndUpdate();
        }

        private static List<int> ObtenerIdsMarcados(CheckedListBox lista)
        {
            return lista.CheckedItems.OfType<OpcionFiltro>().Select(item => item.Id).ToList();
        }

        private static void MarcarIds(CheckedListBox lista, IEnumerable<int> ids)
        {
            HashSet<int> marcados = new HashSet<int>(ids ?? Enumerable.Empty<int>());
            for (int i = 0; i < lista.Items.Count; i++)
            {
                lista.SetItemChecked(i, lista.Items[i] is OpcionFiltro opcion && marcados.Contains(opcion.Id));
            }
        }

        private static void DesmarcarTodo(CheckedListBox lista)
        {
            for (int i = 0; i < lista.Items.Count; i++)
            {
                lista.SetItemChecked(i, false);
            }
        }

        private List<string> ObtenerSegmentosMarcados()
        {
            return clbSegmentos.CheckedItems.OfType<OpcionFiltro>().Select(item => item.Valor).ToList();
        }

        // Un segmento de la promocion que ya no figura en el catalogo se agrega a la lista para
        // no perderlo al guardar de nuevo.
        private void MarcarSegmentos(IEnumerable<string> segmentos)
        {
            List<string> pedidos = (segmentos ?? Enumerable.Empty<string>()).ToList();
            foreach (string segmento in pedidos)
            {
                bool existe = clbSegmentos.Items.OfType<OpcionFiltro>()
                    .Any(item => string.Equals(item.Valor, segmento, StringComparison.OrdinalIgnoreCase));
                if (!existe)
                {
                    string valor = segmento;
                    clbSegmentos.Items.Add(new OpcionFiltro(0, valor, () => valor), false);
                    ActualizarAyudasListasVacias();
                }
            }

            for (int i = 0; i < clbSegmentos.Items.Count; i++)
            {
                OpcionFiltro opcion = (OpcionFiltro)clbSegmentos.Items[i];
                clbSegmentos.SetItemChecked(i, pedidos.Any(item => string.Equals(item, opcion.Valor, StringComparison.OrdinalIgnoreCase)));
            }
        }

        // --- Busqueda y grilla ---

        private int? ObtenerComercioBusqueda()
        {
            return cmbComercioBusquedaPromocion.SelectedItem is Comercio comercio && comercio.Id > 0
                ? (int?)comercio.Id
                : null;
        }

        private void CargarPromociones(int? promocionSeleccionadaId)
        {
            try
            {
                List<Promocion> promociones = _promocionBLL.Buscar(txtbBuscarTextoPromocion.Text, ObtenerComercioBusqueda());

                dgvPromociones.DataSource = null;
                dgvPromociones.DataSource = promociones.Select(ProyectarFilaPromocion).ToList();

                if (promocionSeleccionadaId.HasValue)
                {
                    SeleccionarFila(fila => fila.Id == promocionSeleccionadaId.Value);
                }
                else
                {
                    dgvPromociones.ClearSelection();
                    dgvPromociones.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Busqueda de promociones");
            }
        }

        private static FilaPromocion ProyectarFilaPromocion(Promocion promocion)
        {
            return new FilaPromocion
            {
                Id = promocion.Id,
                Descripcion = promocion.Descripcion,
                Descuento = promocion.Descuento,
                TopeDescuento = promocion.TopeDescuento,
                DiasSemana = promocion.DiasSemana ?? new List<DiaSemana>(),
                VigenciaDesde = promocion.VigenciaDesde,
                VigenciaHasta = promocion.VigenciaHasta,
                Promocion = promocion
            };
        }

        private void btnBuscarPromociones_Click(object sender, EventArgs e)
        {
            CargarPromociones(null);
        }

        private void dgvPromociones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (ObtenerFilaSeleccionada(out FilaPromocion fila))
            {
                CargarEditor(fila.Promocion);
            }
        }

        private void dgvPromociones_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            switch (dgvPromociones.Columns[e.ColumnIndex].Name)
            {
                case "Descuento":
                    if (e.Value is decimal descuento)
                    {
                        e.Value = descuento.ToString("0.##", CultureInfo.CurrentCulture) + " %";
                        e.FormattingApplied = true;
                    }

                    break;
                case "TopeDescuento":
                    e.Value = e.Value is decimal tope ? tope.ToString("0.00", CultureInfo.CurrentCulture) : "-";
                    e.FormattingApplied = true;
                    break;
                case "DiasSemana":
                    if (e.Value is List<DiaSemana> dias)
                    {
                        e.Value = dias.Count == 0
                            ? UiDialogHelper.Traducir(EtiquetasFinanzas.ClaveTodosLosDias)
                            : string.Join(", ", dias.OrderBy(dia => dia).Select(dia => UiDialogHelper.Traducir(EtiquetasFinanzas.ClaveDiaSemana(dia))));
                        e.FormattingApplied = true;
                    }

                    break;
                case "VigenciaDesde":
                case "VigenciaHasta":
                    if (e.Value is DateTime fecha)
                    {
                        e.Value = fecha.ToShortDateString();
                        e.FormattingApplied = true;
                    }

                    break;
            }
        }

        // --- Editor ---

        private void CargarEditor(Promocion promocion)
        {
            _cargandoEditor = true;
            try
            {
                txtbDescripcionPromocion.Text = promocion.Descripcion;
                txtbDescuentoPromocion.Text = promocion.Descuento.ToString("0.##", CultureInfo.InvariantCulture);
                txtbTopePromocion.Text = promocion.TopeDescuento.HasValue
                    ? promocion.TopeDescuento.Value.ToString("0.##", CultureInfo.InvariantCulture)
                    : string.Empty;
                dtpVigenciaDesdePromocion.Value = promocion.VigenciaDesde;
                dtpVigenciaHastaPromocion.Value = promocion.VigenciaHasta;

                foreach (KeyValuePair<DiaSemana, CheckBoxAccesible> par in _checksDias)
                {
                    par.Value.Checked = promocion.DiasSemana != null && promocion.DiasSemana.Contains(par.Key);
                }

                MarcarIds(clbComercios, promocion.ComercioIds);
                MarcarIds(clbRubrosComercio, promocion.RubroComercioIds);
                MarcarIds(clbEntidadesFinancieras, promocion.EntidadFinancieraIds);
                MarcarIds(clbMediosPago, promocion.MedioPagoIds);
                MarcarSegmentos(promocion.Segmentos);
                MarcarIds(clbProductosIncluidos, promocion.ProductoIncluidoIds);
                MarcarIds(clbProductosExcluidos, promocion.ProductoExcluidoIds);
                MarcarIds(clbCategoriasIncluidas, promocion.CategoriaIncluidaIds);
                MarcarIds(clbCategoriasExcluidas, promocion.CategoriaExcluidaIds);
            }
            finally
            {
                _cargandoEditor = false;
            }

            ActualizarAdvertencia();
        }

        private void LimpiarEditor()
        {
            _cargandoEditor = true;
            try
            {
                txtbDescripcionPromocion.Clear();
                txtbDescuentoPromocion.Clear();
                txtbTopePromocion.Clear();
                dtpVigenciaDesdePromocion.Value = DateTime.Today;
                dtpVigenciaHastaPromocion.Value = DateTime.Today.AddMonths(1);

                foreach (CheckBoxAccesible check in _checksDias.Values)
                {
                    check.Checked = false;
                }

                foreach (CheckedListBox lista in ListasDeFiltros())
                {
                    DesmarcarTodo(lista);
                }
            }
            finally
            {
                _cargandoEditor = false;
            }

            ActualizarAdvertencia();
            txtbDescripcionPromocion.Focus();
        }

        private IEnumerable<CheckedListBox> ListasDeFiltros()
        {
            return new[]
            {
                clbComercios, clbRubrosComercio, clbEntidadesFinancieras, clbMediosPago, clbSegmentos,
                clbProductosIncluidos, clbProductosExcluidos, clbCategoriasIncluidas, clbCategoriasExcluidas
            };
        }

        // Arma la promocion a partir del editor. Solo valida el formato de los numeros: las reglas
        // de negocio (rangos, vigencia, ids) las valida el BLL.
        private bool TryLeerEditor(out Promocion datos)
        {
            datos = null;

            if (!TryLeerDecimal(txtbDescuentoPromocion.Text, out decimal descuento))
            {
                UiDialogHelper.MostrarWarning("FormPromotionManagement.Warning.InvalidDiscount");
                return false;
            }

            decimal? tope = null;
            if (!string.IsNullOrWhiteSpace(txtbTopePromocion.Text))
            {
                if (!TryLeerDecimal(txtbTopePromocion.Text, out decimal topeLeido))
                {
                    UiDialogHelper.MostrarWarning("FormPromotionManagement.Warning.InvalidCap");
                    return false;
                }

                tope = topeLeido;
            }

            datos = new Promocion
            {
                Descripcion = txtbDescripcionPromocion.Text,
                Descuento = descuento,
                TopeDescuento = tope,
                VigenciaDesde = dtpVigenciaDesdePromocion.Value.Date,
                VigenciaHasta = dtpVigenciaHastaPromocion.Value.Date,
                DiasSemana = _checksDias.Where(par => par.Value.Checked).Select(par => par.Key).OrderBy(dia => dia).ToList(),
                ComercioIds = ObtenerIdsMarcados(clbComercios),
                RubroComercioIds = ObtenerIdsMarcados(clbRubrosComercio),
                EntidadFinancieraIds = ObtenerIdsMarcados(clbEntidadesFinancieras),
                MedioPagoIds = ObtenerIdsMarcados(clbMediosPago),
                Segmentos = ObtenerSegmentosMarcados(),
                ProductoIncluidoIds = ObtenerIdsMarcados(clbProductosIncluidos),
                ProductoExcluidoIds = ObtenerIdsMarcados(clbProductosExcluidos),
                CategoriaIncluidaIds = ObtenerIdsMarcados(clbCategoriasIncluidas),
                CategoriaExcluidaIds = ObtenerIdsMarcados(clbCategoriasExcluidas)
            };
            return true;
        }

        private static bool TryLeerDecimal(string texto, out decimal valor)
        {
            return decimal.TryParse(
                (texto ?? string.Empty).Trim().Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out valor);
        }

        // Aviso no bloqueante: un producto incluido que pertenece a una categoria excluida queda
        // fuera igual (la exclusion gana), y probablemente no es lo que se quiso cargar.
        private void ActualizarAdvertencia()
        {
            HashSet<int> categoriasExcluidas = new HashSet<int>(ObtenerIdsMarcados(clbCategoriasExcluidas));
            bool hayConflicto = ObtenerIdsMarcados(clbProductosIncluidos).Any(productoId =>
            {
                Producto producto = _productos.FirstOrDefault(item => item.Id == productoId);
                return producto?.CategoriaProducto != null && categoriasExcluidas.Contains(producto.CategoriaProducto.Id);
            });

            lblAdvertenciaCategoriaExcluida.Visible = hayConflicto;
        }

        // ItemCheck se dispara antes de que cambie la marca: se recalcula una vez aplicada.
        private void clbProductosIncluidos_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            RecalcularAdvertenciaDiferida();
        }

        private void clbCategoriasExcluidas_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            RecalcularAdvertenciaDiferida();
        }

        private void RecalcularAdvertenciaDiferida()
        {
            if (_cargandoEditor || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new Action(ActualizarAdvertencia));
        }

        // --- Acciones ---

        private void btnAgregarPromocion_Click(object sender, EventArgs e)
        {
            if (!TryLeerEditor(out Promocion datos))
            {
                return;
            }

            try
            {
                Promocion creada = _promocionBLL.Crear(datos);
                LimpiarEditor();
                CargarPromociones(creada.Id);
                UiDialogHelper.MostrarInfo("FormPromotionManagement.Success.PromocionCreated");
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Alta de promocion");
            }
        }

        private void btnActualizarPromocion_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaPromocion fila))
            {
                UiDialogHelper.MostrarWarning("FormPromotionManagement.Warning.SelectPromocion");
                return;
            }

            if (!TryLeerEditor(out Promocion datos))
            {
                return;
            }

            datos.Id = fila.Id;

            try
            {
                if (_promocionBLL.Actualizar(datos))
                {
                    LimpiarEditor();
                    CargarPromociones(fila.Id);
                    UiDialogHelper.MostrarInfo("FormPromotionManagement.Success.PromocionUpdated");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Edicion de promocion");
            }
        }

        private void btnEliminarPromocion_Click(object sender, EventArgs e)
        {
            if (!ObtenerFilaSeleccionada(out FilaPromocion fila))
            {
                UiDialogHelper.MostrarWarning("FormPromotionManagement.Warning.SelectPromocion");
                return;
            }

            DialogResult confirmacion = UiDialogHelper.Confirmar(
                "FormPromotionManagement.Confirm.DeletePromocionMessage",
                "FormPromotionManagement.Confirm.DeletePromocionTitle",
                MessageBoxIcon.Warning,
                fila.Descripcion);

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (_promocionBLL.Eliminar(fila.Id))
                {
                    LimpiarEditor();
                    CargarPromociones(null);
                    UiDialogHelper.MostrarInfo("FormPromotionManagement.Success.PromocionDeleted");
                }
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Baja de promocion");
            }
        }

        private void btnLimpiarPromocion_Click(object sender, EventArgs e)
        {
            LimpiarEditor();
            dgvPromociones.ClearSelection();
            dgvPromociones.CurrentCell = null;
        }

        // --- Idioma ---

        public override void Actualizar(IObservado observado)
        {
            base.Actualizar(observado);

            // El item "Todos" y los items de las listas arman su texto al mostrarse: se recargan
            // conservando la seleccion para que se recalculen ante un cambio de idioma en caliente.
            try
            {
                CargarComboComercios(ObtenerComercioBusqueda());
            }
            catch (Exception ex)
            {
                UiExceptionHandler.Mostrar(ex, "Actualizacion de idioma de promociones");
            }

            foreach (CheckedListBox lista in ListasDeFiltros())
            {
                List<object> items = lista.Items.Cast<object>().ToList();
                HashSet<object> marcados = new HashSet<object>(lista.CheckedItems.Cast<object>());
                lista.BeginUpdate();
                lista.Items.Clear();
                foreach (object item in items)
                {
                    lista.Items.Add(item, marcados.Contains(item));
                }

                lista.EndUpdate();
            }
        }

        // --- Helpers ---

        private bool ObtenerFilaSeleccionada(out FilaPromocion fila)
        {
            fila = dgvPromociones.CurrentRow?.DataBoundItem as FilaPromocion;
            return fila != null;
        }

        private void SeleccionarFila(Func<FilaPromocion, bool> predicado)
        {
            DataGridViewRow fila = dgvPromociones.Rows
                .Cast<DataGridViewRow>()
                .FirstOrDefault(item => item.DataBoundItem is FilaPromocion dato && predicado(dato));

            if (fila == null)
            {
                return;
            }

            dgvPromociones.ClearSelection();
            fila.Selected = true;
            if (fila.Cells.Count > 0)
            {
                dgvPromociones.CurrentCell = fila.Cells[0];
            }
        }
    }
}
