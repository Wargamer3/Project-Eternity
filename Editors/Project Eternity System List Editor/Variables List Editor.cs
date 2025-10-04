using System;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.SystemListEditor
{
    public partial class VariablesListEditor : BaseEditor
    {
        public VariablesListEditor()
        {
            InitializeComponent();
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[0], "Variables List", new string[0], typeof(VariablesListEditor)),
            };

            return Info;
        }

        private void lstVariables_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddVariable_Click(object sender, EventArgs e)
        {

        }

        private void btnRemoveVariable_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveUpVariable_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveDownVariable_Click(object sender, EventArgs e)
        {

        }

        private void txtVariableDefaultValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void tsmSave_Click(object sender, EventArgs e)
        {

        }
    }
}
