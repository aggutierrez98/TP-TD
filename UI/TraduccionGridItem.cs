namespace TpIDS
{
    internal sealed class TraduccionGridItem
    {
        public int PalabraId { get; set; }

        [GridColumn("FormLanguageManagement.Grid.Key", Order = 0)]
        public string Key { get; set; }

        [GridColumn("FormLanguageManagement.Grid.Value", Order = 1)]
        public string Valor { get; set; }

        public bool EsKeyBase { get; set; }
    }
}
