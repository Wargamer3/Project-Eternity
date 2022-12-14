using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    public partial class BackgroundObject2DEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Background, NewLink };

        private ItemSelectionChoices ItemSelectionChoice;

        private AnimationBackgroundLink ActiveBackgroundLink;
        private AnimationBackgroundObject2D ActiveBackgroundObject;
        private Texture2D sprPixel;

        private MouseEventArgs LastMousePos;

        public BackgroundObject2DEditor()
        {
            InitializeComponent();
            BackgroundViewer.DrawOverlay = DrawOverlayBackground;
            BackgroundLinkViewer.DrawOverlay = DrawOverlayLink;
        }

        public BackgroundObject2DEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write("");//Image Path
                BW.Write(0f);//Position X
                BW.Write(0f);//Position Y
                BW.Write(0f);//Speed X
                BW.Write(0f);//Speed Y
                BW.Write(1f);//Scale X
                BW.Write(1f);//Scale Y
                BW.Write(1f);//Depth
                BW.Write(true);//UseParallaxScrolling

                BW.Write(false);//Repeat X
                BW.Write(false);//Repeat Y
                BW.Write(false);//FlipOnRepeat X
                BW.Write(false);//FlipOnRepeat Y
                BW.Write((byte)255);
                BW.Write((byte)255);
                BW.Write((byte)255);
                BW.Write((byte)255);

                BW.Write(0);//Background link count.

                FS.Close();
                BW.Close();
            }

            LoadBackgroundObject2D(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathAnimationsBackground2DObject, GUIRootPathAnimationsBackground2DUsableItems }, "Animations/Background Objects 2D/", new string[] { ".pebo" }, typeof(BackgroundObject2DEditor)),
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            ActiveBackgroundObject.Save(BW);

            FS.Close();
            BW.Close();
        }

        private void LoadBackgroundObject2D(string AnimationBackgroundPath)
        {
            string Name = AnimationBackgroundPath.Substring(0, AnimationBackgroundPath.Length - 5).Substring(41);
            this.Text = Name + " - Project Eternity Object 2D Editor";

            BackgroundViewer.Preload();
            BackgroundLinkViewer.Preload();

            sprPixel = BackgroundViewer.content.Load<Texture2D>("Pixel");

            FileStream FS = new FileStream("Content/Animations/Background Objects 2D/" + Name + ".pebo", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            ActiveBackgroundObject = new AnimationBackgroundObject2D(BackgroundViewer.content, BR);

            BackgroundViewer.ChangeTexture(ActiveBackgroundObject.sprBackground);

            foreach (AnimationBackgroundLink ActiveChain in ActiveBackgroundObject.ListBackgroundLink)
            {
                lstBackgroundChain.Items.Add(ActiveChain);
            }

            FS.Close();
            BR.Close();

            BackgroundPreviewViewer.Preload();
            BackgroundPreviewViewer.ActiveAnimationBackground = new AnimationBackground2D(BackgroundPreviewViewer.content, BackgroundPreviewViewer.GraphicsDevice);

            BackgroundPreviewViewer.ActiveAnimationBackground.ListBackground.Add(new AnimationBackground2DImageComplex(ActiveBackgroundObject));
        }

        private void tsmSetBackgroundImage_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Background;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundSprites, "Select a Background to use", false));
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void lstBackgroundChain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstBackgroundChain.SelectedIndex < 0)
                return;

            ActiveBackgroundLink = ActiveBackgroundObject.ListBackgroundLink[lstBackgroundChain.SelectedIndex];
            BackgroundLinkViewer.ChangeTexture(ActiveBackgroundLink.sprBackground);
            pgBackgroundLink.SelectedObject = ActiveBackgroundLink;
        }

        private void btnAddBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.NewLink;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundSprites, "Select a Background to use", false));
        }

        private void btnRemoveBackground_Click(object sender, EventArgs e)
        {
            if (lstBackgroundChain.SelectedIndex < 0)
                return;

            ActiveBackgroundObject.ListBackgroundLink.RemoveAt(lstBackgroundChain.SelectedIndex);
            lstBackgroundChain.Items.RemoveAt(lstBackgroundChain.SelectedIndex);
        }

        private void BackgroundViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveBackgroundLink == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                ActiveBackgroundLink.AnchorPointParent = new Vector2(e.X, e.Y);
            }
        }

        private void BackgroundLinkViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveBackgroundLink == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                ActiveBackgroundLink.AnchorPointSelf = new Vector2(e.X, e.Y);
            }
        }

        private void BackgroundPreviewViewer_MouseDown(object sender, MouseEventArgs e)
        {
            LastMousePos = e;
        }

        private void BackgroundPreviewViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (LastMousePos != null && e.Button == MouseButtons.Left)
            {
                BackgroundPreviewViewer.ActiveAnimationBackground.MoveCamera(new Vector3(LastMousePos.X - e.X, LastMousePos.Y - e.Y, 0f));
                LastMousePos = e;
            }
        }

        public void DrawOverlayBackground(CustomSpriteBatch g)
        {
            if (ActiveBackgroundLink == null)
                return;

            g.Begin();

            Vector2 Center = ActiveBackgroundLink.AnchorPointParent;

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

        public void DrawOverlayLink(CustomSpriteBatch g)
        {
            if (ActiveBackgroundLink == null)
                return;

            g.Begin();

            Vector2 Center = ActiveBackgroundLink.AnchorPointSelf;

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
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Background:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(38);

                        Texture2D NewBackground = BackgroundViewer.content.Load<Texture2D>("Animations/Background Sprites/" + Name);
                        ActiveBackgroundObject.sprBackground = NewBackground;
                        ActiveBackgroundObject.BackgroundPath = Name;
                        BackgroundViewer.ChangeTexture(NewBackground);
                        break;

                    case ItemSelectionChoices.NewLink:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(38);
                        AnimationBackgroundLink NewChain = new AnimationBackgroundLink(Name, BackgroundLinkViewer.content);
                        ActiveBackgroundObject.ListBackgroundLink.Add(NewChain);
                        lstBackgroundChain.Items.Add(NewChain);
                        BackgroundLinkViewer.ChangeTexture(NewChain.sprBackground);
                        break;
                }
            }
        }
    }
}
