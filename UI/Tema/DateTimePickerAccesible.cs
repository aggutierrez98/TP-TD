using System;
using System.Globalization;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    /// <summary>
    /// KryptonDateTimePicker que expone su fecha a las tecnologias de accesibilidad (MSAA/UIA).
    /// El KryptonDateTimePicker original aparece como un Pane sin valor. Con este objeto accesible
    /// el valor es legible (fecha corta de la cultura actual) y el rol es DropList, como el
    /// DateTimePicker nativo de Windows.
    /// </summary>
    public class DateTimePickerAccesible : KryptonDateTimePicker
    {
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ObjetoAccesible(this);
        }

        private sealed class ObjetoAccesible : Control.ControlAccessibleObject
        {
            private readonly DateTimePickerAccesible dueno;

            public ObjetoAccesible(DateTimePickerAccesible dueno)
                : base(dueno)
            {
                this.dueno = dueno;
            }

            public override AccessibleRole Role
            {
                get { return AccessibleRole.DropList; }
            }

            public override string Value
            {
                get { return dueno.Value.ToString("d", CultureInfo.CurrentCulture); }
                set
                {
                    DateTime fecha;
                    if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out fecha))
                    {
                        dueno.Value = fecha;
                    }
                }
            }
        }
    }
}
