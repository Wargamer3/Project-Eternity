using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class AnimatedChainSpawnerHelper : Form
    {
        private enum ItemSelectionChoices { ChainLink, ChainEnd, ChainStart };

        private ItemSelectionChoices ItemSelectionChoice;

        private Texture2D sprPixel;

        public string ChainLinkPath;
        public string ChainEndPath;
        public string ChainStartPath;

        public AnimatedChainSpawnerHelper()
        {
            InitializeComponent();

            ChainLinkPath = string.Empty;
            ChainEndPath = string.Empty;
            ChainStartPath = string.Empty;

            ChainLinkViewer.DrawOverlay = DrawOverlayChainLink;
            ChainEndViewer.DrawOverlay = DrawOverlayChainEnd;
            ChainStartViewer.DrawOverlay = DrawOverlayChainStart;
        }

        public AnimatedChainSpawnerHelper(string BitmapName)
            : this()
        {
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSetChainLinkSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ChainLink;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsSprites, "Select an image to use", false));
        }

        private void btnSetChainEndSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ChainEnd;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsSprites, "Select an image to use", false));
        }

        private void btnSetChainStartSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ChainStart;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsSprites, "Select an image to use", false));
        }

        private void ChainLinkViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                txtChainLinkOriginX.Text = e.X.ToString();
                txtChainLinkOriginY.Text = e.Y.ToString();
            }
        }

        private void ChainEndViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                txtChainEndOriginX.Text = e.X.ToString();
                txtChainEndOriginY.Text = e.Y.ToString();
            }
        }

        private void ChainStartViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                txtChainStartOriginX.Text = e.X.ToString();
                txtChainStartOriginY.Text = e.Y.ToString();
            }
        }

        private void AnimatedChainSpawnerHelper_Shown(object sender, EventArgs e)
        {
            sprPixel = ChainLinkViewer.content.Load<Texture2D>("Pixel");

            btnSetChainLinkSprite_Click(sender, e);
        }

        public void DrawOverlayChainLink(CustomSpriteBatch g)
        {
            g.Begin();

            Vector2 Center = new Vector2((float)txtChainLinkOriginX.Value, (float)txtChainLinkOriginY.Value);

            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y + 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 1, 3), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 1, (int)Center.Y - 1, 1, 3), null, Color.Red);

            g.Draw(sprPixel, new Rectangle(0, (int)Center.Y, (int)Center.X - 1, 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 2, (int)Center.Y, Width - (int)Center.X - 5, 1), null, Color.Black);

            g.Draw(sprPixel, new Rectangle((int)Center.X, 0, 1, (int)Center.Y - 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X, (int)Center.Y + 2, 1, Height - (int)Center.Y - 5), null, Color.Black);

            g.End();
        }

        public void DrawOverlayChainEnd(CustomSpriteBatch g)
        {
            g.Begin();

            Vector2 Center = new Vector2((float)txtChainEndOriginX.Value, (float)txtChainEndOriginY.Value);

            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y + 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 1, 3), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 1, (int)Center.Y - 1, 1, 3), null, Color.Red);

            g.Draw(sprPixel, new Rectangle(0, (int)Center.Y, (int)Center.X - 1, 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 2, (int)Center.Y, Width - (int)Center.X - 5, 1), null, Color.Black);

            g.Draw(sprPixel, new Rectangle((int)Center.X, 0, 1, (int)Center.Y - 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X, (int)Center.Y + 2, 1, Height - (int)Center.Y - 5), null, Color.Black);

            g.End();
        }

        public void DrawOverlayChainStart(CustomSpriteBatch g)
        {
            g.Begin();

            Vector2 Center = new Vector2((float)txtChainStartOriginX.Value, (float)txtChainStartOriginY.Value);

            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y + 1, 3, 1), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X - 1, (int)Center.Y - 1, 1, 3), null, Color.Red);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 1, (int)Center.Y - 1, 1, 3), null, Color.Red);

            g.Draw(sprPixel, new Rectangle(0, (int)Center.Y, (int)Center.X - 1, 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X + 2, (int)Center.Y, Width - (int)Center.X - 5, 1), null, Color.Black);

            g.Draw(sprPixel, new Rectangle((int)Center.X, 0, 1, (int)Center.Y - 1), null, Color.Black);
            g.Draw(sprPixel, new Rectangle((int)Center.X, (int)Center.Y + 2, 1, Height - (int)Center.Y - 5), null, Color.Black);

            g.End();
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.ChainLink:
                        ChainLinkViewer.ChangeTexture("Animations/Sprites/" + Name);
                        ChainLinkPath = Name;
                        break;

                    case ItemSelectionChoices.ChainEnd:
                        ChainEndViewer.ChangeTexture("Animations/Sprites/" + Name);
                        ChainEndPath = Name;
                        break;

                    case ItemSelectionChoices.ChainStart:
                        ChainStartViewer.ChangeTexture("Animations/Sprites/" + Name);
                        ChainStartPath = Name;
                        break;
                }
            }
        }
    }
}
