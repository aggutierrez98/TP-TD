using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class GrillaTraduccionHelper
    {
        public static void Recargar<TFuente, TFila>(
            DataGridView grilla,
            IEnumerable<TFuente> datos,
            Func<TFuente, Idioma, TFila> proyector,
            Idioma idioma,
            Action<DataGridView> posDataSource = null)
            where TFila : class
        {
            List<TFila> filas = datos == null
                ? new List<TFila>()
                : datos.Select(item => proyector(item, idioma)).ToList();

            grilla.DataSource = null;
            grilla.DataSource = filas;

            if (posDataSource != null)
            {
                posDataSource(grilla);
            }

            IdiomaFormHelper.AplicarDataGridView(grilla, idioma);
        }
    }
}
