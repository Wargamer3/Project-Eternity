using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class MapStatistics : Form
    {
        public Microsoft.Xna.Framework.Point MapSize;
        public Microsoft.Xna.Framework.Point TileSize;

        public MapStatistics(string MapName, Microsoft.Xna.Framework.Point MapSize, Microsoft.Xna.Framework.Point TileSize)
        {
            InitializeComponent();
            txtMapName.Text = MapName;
            txtMapWidth.Text = MapSize.X.ToString();
            txtMapHeight.Text = MapSize.Y.ToString();
            txtTileWidth.Text = TileSize.X.ToString();
            txtTileHeight.Text = TileSize.Y.ToString();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            MapSize.X = Convert.ToInt32(txtMapWidth.Text);
            MapSize.Y = Convert.ToInt32(txtMapHeight.Text);
            TileSize.X = Convert.ToInt32(txtTileWidth.Text);
            TileSize.Y = Convert.ToInt32(txtTileHeight.Text);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void txtGridWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtGridHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCaseWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCaseHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Ignore;
        }
    }
}
