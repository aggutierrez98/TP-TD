namespace TpIDS
{
    internal sealed class CambioEstadoGridItem
    {
        [GridColumn("FormUserHistory.DiffGrid.Field", Order = 0)]
        public string Campo { get; set; }

        [GridColumn("FormUserHistory.DiffGrid.PreviousValue", Order = 1)]
        public string ValorAnterior { get; set; }

        [GridColumn("FormUserHistory.DiffGrid.NewValue", Order = 2)]
        public string ValorNuevo { get; set; }
    }
}
