using System.Windows.Forms;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class AttackFormulaEditor : Form
    {
        public AttackFormulaEditor()
        {
            InitializeComponent();
        }

        public string Code { get { return codeEditor1.Code; } }
    }
}
