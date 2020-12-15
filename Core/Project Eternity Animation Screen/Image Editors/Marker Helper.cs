using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class MarkerHelper : Form
    {
        public MarkerHelper()
        {
            InitializeComponent();
        }

        public MarkerHelper(MarkerTimeline ActiveMarker)
        {
            InitializeComponent();
            MarkerViewer.ActiveMarker = (MarkerTimeline)ActiveMarker.Copy(null);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSetTexture_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem("Animations/Sprites", "Select a Sprite to use", false));
        }

        private void btnNoPlaceholder_Click(object sender, EventArgs e)
        {
            MarkerViewer.ActiveMarker.Sprite = null;
            MarkerViewer.ActiveMarker.BitmapName = string.Empty;
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null || Items.Count == 0)
            {
                MarkerViewer.ActiveMarker = new MarkerTimeline();
                return;
            }

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Sprites") + 8);
                if (Name != null)
                {
                    MarkerViewer.ActiveMarker = new MarkerTimeline("New Marker", Name, MarkerViewer.content.Load<Texture2D>("Animations/Sprites/" + Name));
                }
            }
        }

        private void AnimatedBitmapSpawnerHelper_Shown(object sender, EventArgs e)
        {
            //Show the user a list of sprite to pick.
            if (MarkerViewer.ActiveMarker == null)
                ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem("Animations/Sprites", "Select a Sprite to use", false));
        }
    }
}
