using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    public sealed class RadioButtonAccesible : KryptonRadioButton
    {
        protected override AccessibleObject CreateAccessibilityInstance() => new ObjetoAccesible(this);

        // Las flechas son teclas de dialogo y sin esto el control no recibe KeyDown por ellas.
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        private sealed class ObjetoAccesible : Control.ControlAccessibleObject
        {
            private readonly RadioButtonAccesible _dueno;
            internal ObjetoAccesible(RadioButtonAccesible dueno) : base(dueno) { _dueno = dueno; }
            public override AccessibleRole Role => AccessibleRole.RadioButton;
            public override AccessibleStates State => base.State | (_dueno.Checked ? AccessibleStates.Checked : AccessibleStates.None);
            public override string DefaultAction => "Select";
            public override void DoDefaultAction() { if (_dueno.Enabled) _dueno.Checked = true; }
        }
    }
}
