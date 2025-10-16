using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.Editors.ImageViewer;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    public partial class AnimationBackgroundEditor2D : BaseEditor
    {
        private enum SelectionChoices { None, Camera, Move }
        private enum ItemSelectionChoices { Sprite };

        private ItemSelectionChoices ItemSelectionChoice;
        private SelectionChoices SelectionChoice;

        private MouseEventArgs MouseEventOld;
        private int SelectedBackgroundIndex;

        public AnimationBackgroundEditor2D()
        {
            InitializeComponent();
        }

        public AnimationBackgroundEditor2D(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Background count.

                FS.Close();
                BW.Close();
            }

            LoadAnimationBackground(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsBackgrounds2D, EditorHelper.GUIRootPathAnimationsBackgroundsAll }, "Animations/Backgrounds 2D/", new string[] { ".peab" }, typeof(AnimationBackgroundEditor2D)),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsBackgroundSprites, EditorHelper.GUIRootPathAnimationsBackground2DUsableItems, EditorHelper.GUIRootPathAnimationsBackground3DUsableItems }, "Animations/Background Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer)),
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            AnimationBackgroundViewer.ActiveAnimationBackground.Save(BW);

            FS.Close();
            BW.Close();
        }

        private void LoadAnimationBackground(string AnimationBackgroundPath)
        {
            string Name = AnimationBackgroundPath.Substring(0, AnimationBackgroundPath.Length - 5).Substring(19);
            this.Text = Name + " - Project Eternity Animation Background Editor";

            AnimationBackgroundViewer.Preload();

            AnimationBackgroundViewer.ActiveAnimationBackground = (AnimationBackground2D)AnimationBackground.LoadAnimationBackground(Name,
                                                                        AnimationBackgroundViewer.content, AnimationBackgroundViewer.GraphicsDevice);

            foreach (AnimationBackground2DBase ActiveBackgroundSystem in AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground)
            {
                TreeNode NewTreeNode = new TreeNode(ActiveBackgroundSystem.ToString());
                NewTreeNode.Tag = ActiveBackgroundSystem;
                lstItemChoices.Nodes.Add(NewTreeNode);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void btnAddBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Sprite;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsBackground2DUsableItems));
        }

        private void btnRemoveSprite_Click(object sender, EventArgs e)
        {
            if (lstItemChoices.SelectedNode != null)
            {
                int SelectedNodeIndex = lstItemChoices.SelectedNode.Index;
                lstItemChoices.Nodes.RemoveAt(SelectedNodeIndex);
                AnimationBackgroundViewer.RemoveBackground(SelectedNodeIndex);
            }
        }

        private void lstItemChoices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (lstItemChoices.SelectedNode != null)
            {
                pgAnimationProperties.SelectedObject = lstItemChoices.SelectedNode.Tag;
            }
        }

        private void AnimationBackgroundViewer_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventOld = e;

            if (Control.ModifierKeys == Keys.Alt)
            {
                SelectionChoice = SelectionChoices.Camera;
            }
            else
            {
                for (int B = 0; B < AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground.Count; B++)
                {
                    AnimationBackground2DBase ActiveBackgroundSystem = AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground[B];
                    if (ActiveBackgroundSystem.CollideWith(new Microsoft.Xna.Framework.Vector2(e.X, e.Y), 0f, 0f, AnimationBackgroundViewer.Width, AnimationBackgroundViewer.Height))
                    {
                        SelectionChoice = SelectionChoices.Move;
                        SelectedBackgroundIndex = B;
                        break;
                    }
                }
            }
        }

        private void AnimationBackgroundViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectionChoice == SelectionChoices.Camera)
            {
                if (e.Button == MouseButtons.Left)
                {
                    AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(new Microsoft.Xna.Framework.Vector3(MouseEventOld.X - e.X, MouseEventOld.Y - e.Y, 0f));
                }
            }
            else if (SelectionChoice == SelectionChoices.Move)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground[SelectedBackgroundIndex].CurrentPosition += new Microsoft.Xna.Framework.Vector2(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground[SelectedBackgroundIndex].StartPosition = AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground[SelectedBackgroundIndex].CurrentPosition;
            }

            MouseEventOld = e;
        }

        private void AnimationBackgroundViewer_MouseUp(object sender, MouseEventArgs e)
        {
            SelectionChoice = SelectionChoices.None;
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
                    case ItemSelectionChoices.Sprite:
                        if (Items[I].StartsWith("Content/Animations/Background Objects 2D/"))
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(41);
                            TreeNode NewTreeNode = new TreeNode(Name);
                            NewTreeNode.Tag = AnimationBackgroundViewer.AddBackgroundObject(Name);
                            lstItemChoices.Nodes.Add(NewTreeNode);
                            lstItemChoices.SelectedNode = NewTreeNode;
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 4).Substring(38);
                            TreeNode NewTreeNode = new TreeNode(Name);
                            NewTreeNode.Tag = AnimationBackgroundViewer.AddBackground(Name);
                            lstItemChoices.Nodes.Add(NewTreeNode);
                            lstItemChoices.SelectedNode = NewTreeNode;
                        }
                        break;
                }
            }
        }
    }
}
