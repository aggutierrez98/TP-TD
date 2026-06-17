using System;

namespace TpIDS
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class GridColumnAttribute : Attribute
    {
        public GridColumnAttribute(string translationKey)
        {
            TranslationKey = translationKey;
            Visible = true;
            ReadOnly = true;
            FillWeight = 100F;
        }

        public string TranslationKey { get; private set; }

        public bool Visible { get; set; }

        public int Order { get; set; }

        public int Width { get; set; }

        public float FillWeight { get; set; }

        public bool ReadOnly { get; set; }

        public string ColumnName { get; set; }

        // Si es true, el valor de las celdas de esta columna se traduce en tiempo de render
        // (la celda guarda la clave y el helper la traduce al pintarse).
        public bool TraducirValor { get; set; }
    }
}
