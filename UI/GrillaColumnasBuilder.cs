using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TpIDS
{
    // Construye las columnas de una grilla a partir de los atributos [GridColumn] del tipo de fila.
    // Cachea las definiciones por tipo para no reflexionar en cada recarga.
    internal static class GrillaColumnasBuilder
    {
        private static readonly Dictionary<Type, List<GridColumnDefinition>> DefinicionesPorTipo =
            new Dictionary<Type, List<GridColumnDefinition>>();

        // Por grilla: nombres de columnas cuyas celdas guardan una clave y se traducen al pintarse.
        private static readonly ConditionalWeakTable<DataGridView, HashSet<string>> ColumnasTraducibles =
            new ConditionalWeakTable<DataGridView, HashSet<string>>();

        // Por grilla: tipo de fila ya configurado. Las columnas se arman una sola vez.
        private static readonly ConditionalWeakTable<DataGridView, Type> TipoConfigurado =
            new ConditionalWeakTable<DataGridView, Type>();

        public static void Configurar<TFila>(DataGridView grilla)
            where TFila : class
        {
            if (grilla == null)
            {
                return;
            }

            // Las columnas se arman una sola vez por grilla: las recargas posteriores solo cambian
            // los datos. Asi los headers (que traduce el Observer segun el Tag) y los anchos no se pierden.
            if (TipoConfigurado.TryGetValue(grilla, out Type tipo) && tipo == typeof(TFila))
            {
                return;
            }

            grilla.AutoGenerateColumns = false;
            grilla.Columns.Clear();

            HashSet<string> traducibles = new HashSet<string>(StringComparer.Ordinal);

            foreach (GridColumnDefinition definicion in ObtenerDefiniciones(typeof(TFila)))
            {
                grilla.Columns.Add(CrearColumna(definicion));

                if (definicion.TraducirValor)
                {
                    traducibles.Add(definicion.ColumnName);
                }
            }

            ColumnasTraducibles.Remove(grilla);
            ColumnasTraducibles.Add(grilla, traducibles);

            // Suscribir una sola vez: el -= remueve la suscripcion previa del mismo metodo estatico.
            grilla.CellFormatting -= TraducirCelda;
            grilla.CellFormatting += TraducirCelda;

            TipoConfigurado.Remove(grilla);
            TipoConfigurado.Add(grilla, typeof(TFila));
        }

        // En cada pintado, las celdas de columnas marcadas traducen su clave al idioma vigente.
        private static void TraducirCelda(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.RowIndex < 0)
            {
                return;
            }

            DataGridView grilla = (DataGridView)sender;
            if (!ColumnasTraducibles.TryGetValue(grilla, out HashSet<string> traducibles) ||
                !traducibles.Contains(grilla.Columns[e.ColumnIndex].Name))
            {
                return;
            }

            e.Value = UiDialogHelper.TraducirSiExiste(e.Value.ToString());
            e.FormattingApplied = true;
        }

        private static List<GridColumnDefinition> ObtenerDefiniciones(Type tipoFila)
        {
            lock (DefinicionesPorTipo)
            {
                if (DefinicionesPorTipo.TryGetValue(tipoFila, out List<GridColumnDefinition> definiciones))
                {
                    return definiciones;
                }

                definiciones = tipoFila
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(propiedad => new
                    {
                        Propiedad = propiedad,
                        Atributo = propiedad.GetCustomAttribute<GridColumnAttribute>(true)
                    })
                    .Where(item => item.Atributo != null && item.Atributo.Visible)
                    .OrderBy(item => item.Atributo.Order)
                    .ThenBy(item => item.Propiedad.MetadataToken)
                    .Select(item => new GridColumnDefinition
                    {
                        ColumnName = string.IsNullOrWhiteSpace(item.Atributo.ColumnName)
                            ? item.Propiedad.Name
                            : item.Atributo.ColumnName.Trim(),
                        DataPropertyName = item.Propiedad.Name,
                        TranslationKey = item.Atributo.TranslationKey,
                        Width = item.Atributo.Width,
                        FillWeight = item.Atributo.FillWeight,
                        ReadOnly = item.Atributo.ReadOnly,
                        TraducirValor = item.Atributo.TraducirValor,
                        EsBooleano = item.Propiedad.PropertyType == typeof(bool) ||
                                     item.Propiedad.PropertyType == typeof(bool?)
                    })
                    .ToList();

                DefinicionesPorTipo[tipoFila] = definiciones;
                return definiciones;
            }
        }

        private static DataGridViewColumn CrearColumna(GridColumnDefinition definicion)
        {
            DataGridViewColumn columna = definicion.EsBooleano
                ? (DataGridViewColumn)new DataGridViewCheckBoxColumn()
                : new DataGridViewTextBoxColumn();

            columna.Name = definicion.ColumnName;
            columna.DataPropertyName = definicion.DataPropertyName;
            columna.Tag = definicion.TranslationKey;
            columna.HeaderText = definicion.TranslationKey;
            columna.ReadOnly = definicion.ReadOnly;

            if (definicion.Width > 0)
            {
                columna.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                columna.Width = definicion.Width;
            }
            else
            {
                columna.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                columna.FillWeight = definicion.FillWeight <= 0 ? 100F : definicion.FillWeight;
            }

            return columna;
        }

        private sealed class GridColumnDefinition
        {
            public string ColumnName { get; set; }
            public string DataPropertyName { get; set; }
            public string TranslationKey { get; set; }
            public int Width { get; set; }
            public float FillWeight { get; set; }
            public bool ReadOnly { get; set; }
            public bool TraducirValor { get; set; }
            public bool EsBooleano { get; set; }
        }
    }
}
