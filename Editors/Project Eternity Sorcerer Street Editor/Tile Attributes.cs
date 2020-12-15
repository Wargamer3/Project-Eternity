using ProjectEternity.GameScreens.SorcererStreetScreen;
using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class TileAttributes : Form
    {
        public int TerrainTypeIndex;//What kind of terrain it is.

        public TileAttributes(int TerrainTypeIndex)
        {
            InitializeComponent();

            cboTerrainType.SelectedIndex = TerrainTypeIndex;
        }

        public TileAttributes(TerrainSorcererStreet ActiveTerrain)
        {
            InitializeComponent();

            cboTerrainType.SelectedIndex = ActiveTerrain.TerrainTypeIndex;
        }

        private void txtMVEnterCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMVMoveCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBonusValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TerrainTypeIndex = cboTerrainType.SelectedIndex;
        }


        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
