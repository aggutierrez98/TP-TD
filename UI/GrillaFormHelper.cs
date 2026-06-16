using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class GrillaFormHelper
    {
        private static readonly Dictionary<Type, List<GridColumnDefinition>> ColumnasPorTipo =
            new Dictionary<Type, List<GridColumnDefinition>>();

        // Cada grilla recuerda como re-proyectar los datos que ya tiene cargados con el idioma
        // vigente. Asi la traduccion de celdas la dispara el observador generico (IdiomaFormHelper)
        // al notificar el cambio de idioma, sin que el formulario tenga que escucharlo ni reconsultar.
        private static readonly ConditionalWeakTable<DataGridView, Action<Idioma>> Retraductores =
            new ConditionalWeakTable<DataGridView, Action<Idioma>>();

        public static void Recargar<TFuente, TFila>(
            DataGridView grilla,
            IEnumerable<TFuente> datos,
            Func<TFuente, Idioma, TFila> proyector,
            Idioma idioma,
            Action<DataGridView> posDataSource = null)
            where TFila : class
        {
            if (grilla == null)
            {
                return;
            }

            List<TFuente> fuente = datos == null ? new List<TFuente>() : datos.ToList();

            Retraductores.Remove(grilla);
            Retraductores.Add(grilla, idiomaVigente => Proyectar(grilla, fuente, proyector, idiomaVigente, posDataSource));

            Proyectar(grilla, fuente, proyector, idioma, posDataSource);
        }

        // Re-proyecta una grilla ya cargada con el idioma indicado. Devuelve false si la grilla
        // no fue cargada via Recargar (no hay datos cacheados para re-proyectar).
        public static bool Retraducir(DataGridView grilla, Idioma idioma)
        {
            if (grilla != null && Retraductores.TryGetValue(grilla, out Action<Idioma> retraducir))
            {
                retraducir(idioma);
                return true;
            }

            return false;
        }

        private static void Proyectar<TFuente, TFila>(
            DataGridView grilla,
            List<TFuente> fuente,
            Func<TFuente, Idioma, TFila> proyector,
            Idioma idioma,
            Action<DataGridView> posDataSource)
            where TFila : class
        {
            List<TFila> filas = fuente.Select(item => proyector(item, idioma)).ToList();

            ConfigurarColumnas<TFila>(grilla);
            grilla.DataSource = null;
            grilla.DataSource = filas;

            if (posDataSource != null)
            {
                posDataSource(grilla);
            }

            IdiomaFormHelper.AplicarDataGridView(grilla, idioma);
        }

        private static void ConfigurarColumnas<TFila>(DataGridView grilla)
            where TFila : class
        {
            if (grilla == null)
            {
                return;
            }

            List<GridColumnDefinition> definiciones = ObtenerDefiniciones(typeof(TFila));
            grilla.AutoGenerateColumns = false;
            grilla.Columns.Clear();

            foreach (GridColumnDefinition definicion in definiciones)
            {
                grilla.Columns.Add(CrearColumna(definicion));
            }
        }

        private static List<GridColumnDefinition> ObtenerDefiniciones(Type tipoFila)
        {
            lock (ColumnasPorTipo)
            {
                if (ColumnasPorTipo.TryGetValue(tipoFila, out List<GridColumnDefinition> definiciones))
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
                        EsBooleano = item.Propiedad.PropertyType == typeof(bool) ||
                                     item.Propiedad.PropertyType == typeof(bool?)
                    })
                    .ToList();

                ColumnasPorTipo[tipoFila] = definiciones;
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

            public bool EsBooleano { get; set; }
        }
    }
}
