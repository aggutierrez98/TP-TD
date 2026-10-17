using System.Windows.Forms;
using Krypton.Toolkit;

namespace TpIDS
{
    /// <summary>
    /// KryptonCheckBox que expone su estado a las tecnologias de accesibilidad (MSAA/UIA).
    /// El KryptonCheckBox original aparece como un Pane sin estado: un lector de pantalla o un
    /// test de UI no puede saber si esta marcado ni accionarlo. Con este objeto accesible se ve
    /// como CheckBox con TogglePattern (rol CheckButton, estado Checked y accion por defecto).
    /// </summary>
    public class CheckBoxAccesible : KryptonCheckBox
    {
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ObjetoAccesible(this);
        }

        private sealed class ObjetoAccesible : Control.ControlAccessibleObject
        {
            private readonly CheckBoxAccesible dueno;

            public ObjetoAccesible(CheckBoxAccesible dueno)
                : base(dueno)
            {
                this.dueno = dueno;
            }

            public override AccessibleRole Role
            {
                get { return AccessibleRole.CheckButton; }
            }

            public override AccessibleStates State
            {
                get { return base.State | (dueno.Checked ? AccessibleStates.Checked : AccessibleStates.None); }
            }

            public override string DefaultAction
            {
                get { return dueno.Checked ? "Uncheck" : "Check"; }
            }

            public override void DoDefaultAction()
            {
                dueno.Checked = !dueno.Checked;
            }
        }
    }
}
