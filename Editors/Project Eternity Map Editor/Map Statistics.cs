using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class MapStatistics : Form
    {
        public List<string> ListBackgroundsPath;
        public List<string> ListForegroundsPath;

        private enum ItemSelectionChoices { Backgrounds, Foregrounds };
        ItemSelectionChoices ItemSelectionChoice;

        public MapStatistics(string MapName, Microsoft.Xna.Framework.Point MapSize, Microsoft.Xna.Framework.Point TileSize, string CameraType,
            Microsoft.Xna.Framework.Vector3 CameraStartPosition, byte PlayersMin, byte PlayersMax, string Description)
        {
            InitializeComponent();
            txtMapName.Text = MapName;
            txtMapWidth.Text = MapSize.X.ToString();
            txtMapHeight.Text = MapSize.Y.ToString();
            txtTileWidth.Text = TileSize.X.ToString();
            txtTileHeight.Text = TileSize.Y.ToString();
            cbCameraType.SelectedText = CameraType;
            txtCameraStartPositionX.Value = (int)Math.Max(0,CameraStartPosition.X);
            txtCameraStartPositionY.Value = (int)Math.Max(0, CameraStartPosition.Y);
            txtPlayersMin.Value = PlayersMin;
            txtPlayersMax.Value = PlayersMax;
            txtDescription.Text = Description;
            ListBackgroundsPath = new List<string>();
            ListForegroundsPath = new List<string>();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
            DialogResult = DialogResult.Ignore;
        }

        private void btnSetBackgrounds_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Backgrounds;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll, "Select which backgrounds to use.", true));
        }

        private void btnSetForegrounds_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Foregrounds;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll, "Select which foregrounds to use.", true));
        }
        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                string BackgroundName = Items[I].Substring(0, Items[0].Length - 5).Substring(19);

                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Backgrounds:
                        ListBackgroundsPath.Clear();
                        ListBackgroundsPath.Add(BackgroundName);
                        break;

                    case ItemSelectionChoices.Foregrounds:
                        ListForegroundsPath.Clear();
                        ListForegroundsPath.Add(BackgroundName);
                        break;
                }
            }
        }
    }
}
