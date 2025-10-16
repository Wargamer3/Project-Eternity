using ProjectEternity.Core.Editor;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ProjectEternity.Editors.AnimationEditor
{
    public partial class AnimationProperties : Form
    {
        public int ScreenWidth = 640;
        public int ScreenHeight = 480;
        public string BackgroundPreview = "";

        public AnimationProperties()
        {
            InitializeComponent();
        }

        private void btnSelectBackgroundPreview_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsBackgroundsAll));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                BackgroundPreview = Items[I].Substring(0, Items[I].Length - 5).Substring(19);
            }
        }
    }
}
