using ProjectEternity.Core.Editor;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                BackgroundPreview = Items[I].Substring(0, Items[0].Length - 5).Substring(19);
            }
        }
    }
}
