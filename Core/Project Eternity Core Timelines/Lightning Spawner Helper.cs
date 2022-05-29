using System;
using System.Windows.Forms;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class LightningSpawnerHelper : Form
    {
        public LightningSpawnerHelper()
        {
            InitializeComponent();
        }

        public LightningSpawnerHelper(string BitmapName)
        {
            if (SpawnViewer != null)
            {
                SpawnViewer.BitmapName = BitmapName;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void tvLightningStructure_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pgLightningNodeProperties.SelectedObject = tvLightningStructure.SelectedNode;
        }

        private void tsmNewChild_Click(object sender, EventArgs e)
        {
            tvLightningStructure.Nodes.Add("New node");
        }

        private void tsmNewRoot_Click(object sender, EventArgs e)
        {

        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {

        }
    }
}
