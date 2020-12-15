using System.Windows.Forms;

namespace ProjectEternity.Core.AI
{
    public partial class FollowingScriptOrderEditor : Form
    {
        public FollowingScripts[] ArrayFollowingScript;

        public FollowingScriptOrderEditor(FollowingScripts[] ArrayFollowingScript)
        {
            InitializeComponent();

            this.ArrayFollowingScript = new FollowingScripts[ArrayFollowingScript.Length];
            ArrayFollowingScript.CopyTo(this.ArrayFollowingScript, 0);

            lstFollowingScripts.Items.Clear();
            lstFollowingScripts.Items.AddRange(this.ArrayFollowingScript);

            if (this.ArrayFollowingScript.Length > 0)
                lstFollowingScripts.SelectedIndex = 0;
        }

        private void lstFollowingScripts_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            FollowingScripts SelectedItem = (FollowingScripts)lstFollowingScripts.SelectedItem;
            lstScriptsOrder.Items.Clear();
            lstScriptsOrder.Items.AddRange(SelectedItem.ListScript.ToArray());
        }

        private void btnMoveUp_Click(object sender, System.EventArgs e)
        {
            if (lstScriptsOrder.SelectedIndex > 0)
            {
                FollowingScripts SelectedItem = (FollowingScripts)lstFollowingScripts.SelectedItem;
                int SelectedIndex = lstScriptsOrder.SelectedIndex;

                ScriptEvaluator SelectedScript = SelectedItem.ListScript[SelectedIndex];

                SelectedItem.ListScript[SelectedIndex] = SelectedItem.ListScript[SelectedIndex - 1];
                SelectedItem.ListScript[SelectedIndex - 1] = SelectedScript;

                lstScriptsOrder.Items[SelectedIndex] = lstScriptsOrder.Items[SelectedIndex - 1];
                lstScriptsOrder.Items[SelectedIndex - 1] = SelectedScript;

                lstScriptsOrder.SelectedIndex = SelectedIndex - 1;
            }
        }

        private void btnMoveDown_Click(object sender, System.EventArgs e)
        {
            if (lstScriptsOrder.SelectedIndex < lstScriptsOrder.Items.Count - 1)
            {
                FollowingScripts SelectedItem = (FollowingScripts)lstFollowingScripts.SelectedItem;
                int SelectedIndex = lstScriptsOrder.SelectedIndex;

                ScriptEvaluator SelectedScript = SelectedItem.ListScript[SelectedIndex];

                SelectedItem.ListScript[SelectedIndex] = SelectedItem.ListScript[SelectedIndex + 1];
                SelectedItem.ListScript[SelectedIndex + 1] = SelectedScript;

                lstScriptsOrder.Items[SelectedIndex] = lstScriptsOrder.Items[SelectedIndex + 1];
                lstScriptsOrder.Items[SelectedIndex + 1] = SelectedScript;

                lstScriptsOrder.SelectedIndex = SelectedIndex + 1;
            }
        }
    }
}
