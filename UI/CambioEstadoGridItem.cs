namespace TpIDS
{
    internal sealed class CambioEstadoGridItem
    {
        [GridColumn("FormUserHistory.DiffGrid.Field", Order = 0, TraducirValor = true)]
        public string Campo { get; set; }

        [GridColumn("FormUserHistory.DiffGrid.PreviousValue", Order = 1, TraducirValor = true)]
        public string ValorAnterior { get; set; }

        [GridColumn("FormUserHistory.DiffGrid.NewValue", Order = 2, TraducirValor = true)]
        public string ValorNuevo { get; set; }
    }
}
