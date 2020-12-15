using System;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class AnimatedBitmapSpawnerHelper : Form
    {
        public AnimatedBitmapSpawnerHelper()
        {
            InitializeComponent();
        }

        public AnimatedBitmapSpawnerHelper(string BitmapName)
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

        private void btnSetSprite_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem("Animations/Sprites", "Select an image to use", false));
        }

        private void SpawnViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                txtOriginX.Text = e.X.ToString();
                txtOriginY.Text = e.Y.ToString();
            }
        }

        private void AnimatedBitmapSpawnerHelper_Shown(object sender, EventArgs e)
        {
            //Show the user a list of sprite to pick.
            if (SpawnViewer.BitmapName == null)
                btnSetSprite_Click(sender, e);
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                if (Name != null)
                {
                    SpawnViewer.Bitmap = SpawnViewer.content.Load<Texture2D>("Animations/Sprites/" + Name);
                    SpawnViewer.BitmapName = Name;
                }
            }
        }
    }
}
