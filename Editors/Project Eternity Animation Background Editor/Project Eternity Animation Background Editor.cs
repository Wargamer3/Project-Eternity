using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    public partial class AnimationBackgroundEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Sprite };

        private ItemSelectionChoices ItemSelectionChoice;
        private MouseEventArgs MousePositionOld;
        private BackgroundProperties PropertiesDialog;
        private AnimationBackground3DBase SelectedBackgroundSystem;

        public AnimationBackgroundEditor()
        {
            InitializeComponent();
        }

        public AnimationBackgroundEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);
                
                BW.Write((byte)0);//World Type.
                BW.Write(200);//World Width.
                BW.Write(200);//World Depth.
                BW.Write(0f);//Save Background Default Camera Position X.
                BW.Write(-150f);//Save Background Default Camera Position Y.
                BW.Write(0);//Save Background Default Camera Position Z.
                BW.Write(0f);//Save Background Default Camera Yaw.
                BW.Write(2.9f);//Save Background Default Camera Position Pitch.
                BW.Write(0);//Save Background Default Camera Position Roll.

                BW.Write(0);//Backgrounds count.

                FS.Close();
                BW.Close();
            }

            LoadAnimationBackground(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsBackgrounds3D, EditorHelper.GUIRootPathAnimationsBackgroundsAll }, "Animations/Backgrounds 3D/", new string[] { ".peab" }, typeof(AnimationBackgroundEditor)),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsBackground3DModels, EditorHelper.GUIRootPathAnimationsBackground3DUsableItems }, "Animations/Models/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer)),
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

            AnimationBackgroundViewer.ActiveAnimationBackground = (AnimationBackground3D)AnimationBackground.LoadAnimationBackground(Name, AnimationBackgroundViewer.content,
                                                                                                                                        AnimationBackgroundViewer.GraphicsDevice);

            PropertiesDialog = new BackgroundProperties(AnimationBackgroundViewer.ActiveAnimationBackground);

            PropertiesDialog.txtBackgroundStartX.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraPosition.X;
            PropertiesDialog.txtBackgroundStartY.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraPosition.Y;
            PropertiesDialog.txtBackgroundStartZ.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraPosition.Z;

            PropertiesDialog.txtBackgroundYaw.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraRotation.X;
            PropertiesDialog.txtBackgroundPitch.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraRotation.Y;
            PropertiesDialog.txtBackgroundRoll.Value = (decimal)AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraRotation.Z;

            foreach (AnimationBackground3DBase ActiveBackgroundSystem in AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground)
            {
                TreeNode NewTreeNode = new TreeNode(ActiveBackgroundSystem.ToString());
                NewTreeNode.Tag = ActiveBackgroundSystem;
                lstItemChoices.Nodes.Add(NewTreeNode);
                foreach (string ActiveChild in ActiveBackgroundSystem.GetChild())
                {
                    NewTreeNode.Nodes.Add(ActiveChild);
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void tsmProperties_Click(object sender, EventArgs e)
        {
            PropertiesDialog.ShowDialog();

            AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraPosition = new Vector3(
                                                                            (float)PropertiesDialog.txtBackgroundStartX.Value,
                                                                            (float)PropertiesDialog.txtBackgroundStartY.Value,
                                                                            (float)PropertiesDialog.txtBackgroundStartZ.Value);

            AnimationBackgroundViewer.ActiveAnimationBackground.DefaultCameraRotation = new Vector3(
                                                                            (float)PropertiesDialog.txtBackgroundYaw.Value,
                                                                            (float)PropertiesDialog.txtBackgroundPitch.Value,
                                                                            (float)PropertiesDialog.txtBackgroundRoll.Value);
        }

        private void btnLoadNewBackgroundType_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Sprite;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsBackground3DUsableItems));
        }

        private void btnCreateNewProp_Click(object sender, EventArgs e)
        {
            CreatePropAtMousePosition(AnimationBackgroundViewer.Width / 2, AnimationBackgroundViewer.Height / 2);
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            AnimationBackground3DBase ActiveParticleSystem = lstItemChoices.SelectedNode.Tag as AnimationBackground3DBase;

            if (ActiveParticleSystem != null)
            {
                int SelectedNodeIndex = lstItemChoices.SelectedNode.Index;
                lstItemChoices.Nodes.RemoveAt(SelectedNodeIndex);
                AnimationBackgroundViewer.RemoveBackgroundSystem(SelectedNodeIndex);
            }
            else if (ActiveParticleSystem == null && lstItemChoices.SelectedNode.Parent != null)
            {
                TreeNode SelectedNode = lstItemChoices.SelectedNode.Parent;
                int SelectedNodeIndex = SelectedNode.Index;
                ActiveParticleSystem = SelectedNode.Tag as AnimationBackground3DBase;

                ActiveParticleSystem.RemoveItem(SelectedNodeIndex);
                lstItemChoices.Nodes.Remove(lstItemChoices.SelectedNode);
            }
        }

        private void lstItemChoices_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (lstItemChoices.SelectedNode != null)
            {
                if (lstItemChoices.SelectedNode.Parent == null)
                {
                    AnimationBackground3DBase ActiveParticleSystem = lstItemChoices.SelectedNode.Tag as AnimationBackground3DBase;

                    pgAnimationProperties.SelectedObject = ActiveParticleSystem.GetEditableObject(-1);
                }
                else
                {
                    AnimationBackground3DBase ActiveParticleSystem = lstItemChoices.SelectedNode.Parent.Tag as AnimationBackground3DBase;

                    pgAnimationProperties.SelectedObject = ActiveParticleSystem.GetEditableObject(lstItemChoices.SelectedNode.Index);
                }
            }
        }

        #region AnimationBackgroundViewer

        private void AnimationBackgroundViewer_MouseDown(object sender, MouseEventArgs e)
        {
            AnimationBackgroundViewer.Focus();
            float MouseX = e.X;
            float MouseY = e.Y;

            float MinDistanceToTriangle = float.MaxValue;
            SelectedBackgroundSystem = null;
            int SelectedBackgroundIndex = -1;

            for (int P = 0; P < AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground.Count; P++)
            {
                AnimationBackground3DBase ActiveBackgroundSystem = AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground[P];
                float Distance = ActiveBackgroundSystem.GetDistance(MouseX, MouseY,
                    AnimationBackgroundViewer.ActiveAnimationBackground.View,
                    AnimationBackgroundViewer.ActiveAnimationBackground.Projection,
                    AnimationBackgroundViewer.GraphicsDevice.Viewport);

                if (Distance < MinDistanceToTriangle)
                {
                    MinDistanceToTriangle = Distance;
                    SelectedBackgroundSystem = ActiveBackgroundSystem;
                    SelectedBackgroundIndex = P;
                }
            }

            pgAnimationProperties.SelectedObject = null;

            if (SelectedBackgroundSystem != null)
            {
                pgAnimationProperties.SelectedObject = SelectedBackgroundSystem.GetEditableObject(SelectedBackgroundIndex);
            }

            MousePositionOld = e;
        }

        private void AnimationBackgroundViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift)
                CreatePropAtMousePosition(e.X, e.Y);

            MousePositionOld = e;
        }

        private void AnimationBackgroundViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    CreatePropAtMousePosition(e.X, e.Y);
                }
                else if (pgAnimationProperties.SelectedObject != null)
                {
                    AnimationBackground3D.TemporaryBackground SelectedObject = (AnimationBackground3D.TemporaryBackground)pgAnimationProperties.SelectedObject;
                    Vector3 NewPos = (MousePositionOld.Y - e.Y) * AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Up;
                    NewPos -= (MousePositionOld.X - e.X) * AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Right;
                    SelectedObject.Move(NewPos);
                }
                else
                {
                    AnimationBackgroundViewer.ActiveAnimationBackground.Pitch += (MousePositionOld.Y - e.Y) / 100f;
                    AnimationBackgroundViewer.ActiveAnimationBackground.Yaw += (MousePositionOld.X - e.X) / 100f;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(MousePositionOld.Y - e.Y, -AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Up);
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(MousePositionOld.X - e.X, AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Right);
            }

            MousePositionOld = e;
        }

        private void AnimationBackgroundViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(e.Delta * 0.5f, AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Forward);
        }

        private void AnimationBackgroundViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.W)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(1, AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Forward);
            }
            if (e.KeyData == Keys.S)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(1, -AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Forward);
            }
            if (e.KeyData == Keys.A)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(1, -AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Right);
            }
            if (e.KeyData == Keys.D)
            {
            }
            if (e.KeyData == Keys.E)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(1, AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Up);
            }
            if (e.KeyData == Keys.Q)
            {
                AnimationBackgroundViewer.ActiveAnimationBackground.MoveCamera(1, -AnimationBackgroundViewer.ActiveAnimationBackground.CameraRotation.Up);
            }
        }

        private void AnimationBackgroundViewer_KeyUp(object sender, KeyEventArgs e)
        {
        }

        #endregion

        private void CreatePropAtMousePosition(int MouseX, int MouseY)
        {
            if (lstItemChoices.SelectedNode != null)
            {
                TreeNode SelectedNode = lstItemChoices.SelectedNode;
                AnimationBackground3DBase ActiveParticleSystem = lstItemChoices.SelectedNode.Tag as AnimationBackground3DBase;

                if (ActiveParticleSystem == null && lstItemChoices.SelectedNode.Parent != null)
                {
                    SelectedNode = lstItemChoices.SelectedNode.Parent;
                    ActiveParticleSystem = lstItemChoices.SelectedNode.Parent.Tag as AnimationBackground3DBase;
                }
                if (ActiveParticleSystem != null)
                {
                    Vector3 NearScreenPoint = new Vector3(MouseX, MouseY, 0);
                    Vector3 FarScreenPoint = new Vector3(MouseX, MouseY, 1);
                    Vector3 NearWorldPoint = AnimationBackgroundViewer.GraphicsDevice.Viewport.Unproject(NearScreenPoint,
                                                                AnimationBackgroundViewer.ActiveAnimationBackground.Projection,
                                                                AnimationBackgroundViewer.ActiveAnimationBackground.View, Matrix.Identity);
                    Vector3 FarWorldPoint = AnimationBackgroundViewer.GraphicsDevice.Viewport.Unproject(FarScreenPoint,
                                                                AnimationBackgroundViewer.ActiveAnimationBackground.Projection,
                                                                AnimationBackgroundViewer.ActiveAnimationBackground.View, Matrix.Identity);

                    Vector3 Direction = FarWorldPoint - NearWorldPoint;

                    float zFactor = -NearWorldPoint.Y / Direction.Y;
                    Vector3 ZeroWorldPoint = NearWorldPoint + Direction * zFactor;

                    ActiveParticleSystem.AddItem(ZeroWorldPoint);
                    SelectedNode.Nodes.Add("Prop " + (SelectedNode.Nodes.Count + 1));
                }
            }
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
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(19);
                        TreeNode NewTreeNode = new TreeNode(Name);
                        AnimationBackground3DBase NewBackgroundSystem = AnimationBackgroundViewer.AddBackgroundSystem(Name);
                        AnimationBackgroundViewer.ActiveAnimationBackground.ListBackground.Add(NewBackgroundSystem);
                        NewTreeNode.Tag = NewBackgroundSystem;
                        lstItemChoices.Nodes.Add(NewTreeNode);
                        lstItemChoices.SelectedNode = NewTreeNode;
                        btnCreateNewProp_Click(null, null);
                        break;
                }
            }
        }
    }
}
