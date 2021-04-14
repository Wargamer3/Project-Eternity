using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class SpriteSheetHelper : Form
    {
        public bool Replace;

        public SpriteSheetHelper()
        {
            InitializeComponent();

            Replace = false;
            SpriteSheetViewer.DicActiveSpriteSheetBitmap = new Dictionary<Tuple<int, int>, SpriteSheetTimeline>();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem("Animations/Sprite Sheets", "Select a sprite sheet to use", false));
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvSpriteSheets.SelectedIndices.Count == 0)
                return;

            SpriteSheetViewer.DicSpriteSheet.Remove(lvSpriteSheets.SelectedItems[0].Text);
            lvSpriteSheets.Items.RemoveAt(lvSpriteSheets.SelectedIndices[0]);
        }

        private void SpriteSheetViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvSpriteSheets.SelectedIndices.Count == 0)
                return;

            foreach (Shape CurrentShape in SpriteSheetViewer.HashShape)
            {
                if (e.X >= CurrentShape.X1 && e.X <= CurrentShape.X2 && e.Y >= CurrentShape.Y1 && e.Y <= CurrentShape.Y2)
                {
                    if (!SpriteSheetViewer.DicActiveSpriteSheetBitmap.ContainsKey(new Tuple<int, int>(CurrentShape.X1, CurrentShape.Y1)))
                    {
                        if (Replace)
                        {
                            SpriteSheetViewer.DicActiveSpriteSheetBitmap.Clear();
                        }
                        SpriteSheetViewer.DicActiveSpriteSheetBitmap.Add(new Tuple<int, int>(CurrentShape.X1, CurrentShape.Y1), new SpriteSheetTimeline(lvSpriteSheets.SelectedItems[0].Text, (Texture2D)lvSpriteSheets.SelectedItems[0].Tag,
                            new Microsoft.Xna.Framework.Rectangle(CurrentShape.X1, CurrentShape.Y1, CurrentShape.Width, CurrentShape.Height)));
                    }
                }
            }
        }

        private void lvSpriteSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSpriteSheets.SelectedIndices.Count == 0)
                return;

            SpriteSheetViewer.SpriteSheet = SpriteSheetViewer.DicSpriteSheet[lvSpriteSheets.SelectedItems[0].Text];

            short ContourCount = 0;
            ContourTracingTechnique.CCL_Map2D FirstImage = null;

            ContourTracingTechnique Newstuff = new ContourTracingTechnique();

            Newstuff.load_bmp(SpriteSheetViewer.SpriteSheet, ref FirstImage);
            SpriteSheetViewer.HashShape = ContourTracingTechnique.GetContour(Newstuff.Ras2Vec2D(FirstImage, (ushort)0, ref ContourCount));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(Items[0].LastIndexOf("Animations") + 25);
                if (Name != null)
                {
                    if (Replace)
                    {
                        SpriteSheetViewer.DicSpriteSheet.Clear();
                        lvSpriteSheets.Items.Clear();
                    }
                    ListViewItem NewListViewItem = new ListViewItem(Name);
                    Texture2D NewTexture = SpriteSheetViewer.content.Load<Texture2D>("Animations/Sprite Sheets/" + Name);
                    NewListViewItem.Tag = NewTexture;
                    SpriteSheetViewer.DicSpriteSheet.Add(Name, NewTexture);
                    lvSpriteSheets.Items.Add(NewListViewItem);

                    NewListViewItem.Selected = true;
                    NewListViewItem.Focused = true;
                }
            }
        }
    }
}
