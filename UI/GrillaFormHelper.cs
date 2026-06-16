using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using SERVICIOS;

namespace TpIDS
{
    internal static class GrillaFormHelper
    {
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
            Retraductores.Add(grilla, idiomaVigente => Pintar(grilla, fuente, proyector, idiomaVigente, posDataSource));

            Pintar(grilla, fuente, proyector, idioma, posDataSource);
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

        private static void Pintar<TFuente, TFila>(
            DataGridView grilla,
            List<TFuente> fuente,
            Func<TFuente, Idioma, TFila> proyector,
            Idioma idioma,
            Action<DataGridView> posDataSource)
            where TFila : class
        {
            List<TFila> filas = fuente.Select(item => proyector(item, idioma)).ToList();

            GrillaColumnasBuilder.Configurar<TFila>(grilla);
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
