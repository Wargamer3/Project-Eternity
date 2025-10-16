using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.Editors.BitmapAnimationEditor;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationEditor
{
    public partial class ProjectEternityAnimationEditor : BaseEditor
    {//Can you do that thing where you press left click and then drag it to select multiple things
        //A special layer just for SFX, quotes, background, etc
        //http://imgur.com/a/rbxH3
        // When you click on a bitmap
        //[20:39:02] <Finlander> You could also have stuff like the drawing depth and alpha available in the menu
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        public CheckBox cbShowBorderBoxes;
        public CheckBox cbShowNextPositions;
        public AnimationClassEditor ActiveAnimation;

        private AnimationEditorController Controller;
        public AnimationProperties PropertiesDialog;

        public ProjectEternityAnimationEditor()
        {
            Controller = new AnimationEditorController(this);
            InitializeComponent();

            if (!DesignMode)
            {
                PropertiesDialog = new AnimationProperties();
                Controller.Init();
            }
        }

        public ProjectEternityAnimationEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                //Create the Part file.
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);
                
                BW.Write(0);//Save Start loop frame.
                BW.Write(0);//Save End loop frame.
                BW.Write(PropertiesDialog.ScreenWidth);
                BW.Write(PropertiesDialog.ScreenHeight);

                BW.Write(1);//Default animation layer.
                BW.Write("Default Layer");
                BW.Write((byte)AnimationClass.AnimationLayer.LayerBlendStates.Add);
                BW.Write((byte)AnimationClass.AnimationLayer.LayerSamplerStates.LinearClamp);
                BW.Write(0);//Empty Layer Group list.
                BW.Write(0);//Empty Layer Event list.
                BW.Write(0);//Empty Layer Children list.

                AnimationClass.GameEngineLayer DefaultGameEngineLayer = AnimationClass.GameEngineLayer.EmptyGameEngineLayer(null);
                DefaultGameEngineLayer.SaveLayer(BW);

                FS.Close();
                BW.Close();
            }
            LoadAnimation(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimations, EditorHelper.GUIRootPathVisualNovelCharacters }, "Animations/", new string[] { ".pea" }, typeof(ProjectEternityAnimationEditor)),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsSprites }, "Animations/Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsSpriteSheets }, "Animations/Sprite Sheets/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathAnimationsBitmapAnimations }, "Animations/Bitmap Animations/", new string[] { ".png" }, typeof(ProjectEternityBitmapAnimationEditor), false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);
            
            BW.Write((int)PropertiesDialog.txtLoopStart.Value);
            BW.Write((int)PropertiesDialog.txtLoopEnd.Value);

            BW.Write(PropertiesDialog.ScreenWidth);
            BW.Write(PropertiesDialog.ScreenHeight);

            BW.Write(AnimationViewer.ActiveAnimation.ListAnimationLayer.BasicLayerCount);
            for (int L = 0; L < AnimationViewer.ActiveAnimation.ListAnimationLayer.BasicLayerCount; L++)
            {
                AnimationViewer.ActiveAnimation.ListAnimationLayer[L].SaveLayer(BW);
            }
            AnimationViewer.ActiveAnimation.ListAnimationLayer.EngineLayer.SaveLayer(BW);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load an Animation at selected path.
        /// </summary>
        /// <param name="AnimationPath">Path from which to open the Animation.</param>
        private void LoadAnimation(string AnimationPath)
        {
            string Name = AnimationPath.Substring(0, AnimationPath.Length - 4).Substring(19);
            this.Text = Name + " - Project Eternity Animation Editor";

            AnimationViewer.Preload();

            ActiveAnimation = new AnimationClassEditor(Name);
            ActiveAnimation.Content = new ContentManager(AnimationViewer.Services);
            ActiveAnimation.Content.RootDirectory = "Content";
            AnimationViewer.ActiveAnimation = ActiveAnimation;

            AnimationViewer.Services.AddService<GraphicsDevice>(AnimationViewer.GraphicsDevice);
            ActiveAnimation.Load();

            PropertiesDialog.txtScreenWidth.Value = ActiveAnimation.ScreenWidth;
            PropertiesDialog.txtScreenHeight.Value = ActiveAnimation.ScreenHeight;

            PropertiesDialog.txtLoopStart.Value = ActiveAnimation.LoopStart;
            PropertiesDialog.txtLoopEnd.Value = ActiveAnimation.LoopEnd;

            foreach (KeyValuePair<string, Timeline> ActiveTimeline in ActiveAnimation.DicTimeline)
            {
                if (ActiveTimeline.Value is AnimationOriginTimeline)
                {
                    continue;
                }

                if (ActiveTimeline.Value is VisibleTimeline)
                {
                    ActiveTimeline.Value.OnAnimationEditorLoad(ActiveAnimation);
                    ToolStripMenuItem tsmNewItem = new ToolStripMenuItem(ActiveTimeline.Value.TimelineEventType);
                    tsmNewItem.Tag = ActiveTimeline.Value;
                    tsmNewItem.Click += AnimationViewer.tsmNewItem_Click;
                    AnimationViewer.spawnNewToolStripMenuItem.DropDownItems.Add(tsmNewItem);
                }
            }

            for (int L = 0; L < AnimationViewer.ActiveAnimation.ListAnimationLayer.BasicLayerCount; L++)
            {
                foreach (var ActiveTimelineFrame in AnimationViewer.ActiveAnimation.ListAnimationLayer[L].DicTimelineEvent.Values)
                {
                    foreach (var ActiveTimeline in ActiveTimelineFrame)
                    {
                        ActiveTimeline.OnAnimationEditorLoad(ActiveAnimation);
                    }
                }
            }

            panTimelineViewer.SetActiveAnimation(ActiveAnimation);
        }

        public void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        public void tsmProperties_Click(object sender, EventArgs e)
        {
            PropertiesDialog.ShowDialog();
            if (!string.IsNullOrEmpty(PropertiesDialog.BackgroundPreview))
            {
                AnimationViewer.LoadBackgroundPreview(PropertiesDialog.BackgroundPreview);

                ActiveAnimation.ActiveAnimationBackground.ResetCamera();
            }
        }

        private void tsmUndo_Click(object sender, EventArgs e)
        {
            Controller.Undo();
        }

        //Used so the alt key won't cause the Form to unfocus.
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
                return true;
            else
                return base.ProcessDialogKey(keyData);
        }

        public void ProjectEternityAnimationEditor_Shown(object sender, EventArgs e)
        {
            Controller.ProjectEternityAnimationEditor_Shown(sender, e);

            Controller.SaveCurrentAnimation();
        }

        private void pgAnimationProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Controller.SaveCurrentAnimation();
        }

        public void ProjectEternityAnimationEditor_Resize(object sender, EventArgs e)
        {
            Controller.ProjectEternityAnimationEditor_Resize(sender, e);
        }

        #region Animation Viewer

        private void AnimationViewer_TimelineChanged()
        {
            panTimelineViewer.OnTimelineChanged();
            Controller.SaveCurrentAnimation();
            ActiveAnimation.CreateMultipleSelectionRectangle();
        }

        private void AnimationViewer_TimelineSelected(Timeline SelectedTimeline)
        {
            pgAnimationProperties.SelectedObject = SelectedTimeline;
        }

        private void AnimationViewer_TimelineSelectionChanged()
        {
            ActiveAnimation.CreateMultipleSelectionRectangle();
            panTimelineViewer.UpdateTimelineVisibleItems();
            panTimelineViewer.OnTimelineChanged();
        }

        private void AnimationViewer_LayersChanged()
        {
            Controller.UpdateAnimationLayerVisibleItems();
            Controller.SaveCurrentAnimation();
            Controller.DrawAnimationLayers();
        }

        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            Controller.tmrAnimation_Tick(sender, e);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Controller.btnPlay_Click(sender, e);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Controller.btnPause_Click(sender, e);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Controller.btnStop_Click(sender, e);
        }

        #endregion

        #region Timeline

        public void OnTimelineSelectionChange()
        {
            Controller.SaveCurrentAnimation();
        }

        protected void OnTimelineSelection(Timeline ActiveTimelineEvent)
        {
            pgAnimationProperties.SelectedObject = ActiveTimelineEvent;
            ActiveAnimation.CreateMultipleSelectionRectangle();
        }

        protected void OnKeyFrameSelected(AnimationObjectKeyFrame ActiveKeyFrame)
        {
            pgAnimationProperties.SelectedObject = ActiveKeyFrame;
        }

        #endregion

        #region Animation Layers

        public void btnAddLayer_Click(object sender, EventArgs e)
        {
            Controller.btnAddLayer_Click(sender, e);
        }

        public void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            Controller.btnRemoveLayer_Click(sender, e);
        }

        public void panAnimationLayers_Paint(object sender, PaintEventArgs e)
        {
            Controller.panAnimationLayers_Paint(sender, e);
        }

        public void vsbAnimationLayer_Scroll(object sender, ScrollEventArgs e)
        {
            Controller.vsbAnimationLayer_Scroll(sender, e);
        }

        public void panAnimationLayers_MouseUp(object sender, MouseEventArgs e)
        {
            Controller.panAnimationLayers_MouseUp(sender, e);
        }

        public void panAnimationLayers_MouseMove(object sender, MouseEventArgs e)
        {
            Controller.panAnimationLayers_MouseMove(sender, e);
        }

        public void panAnimationLayers_DragEnter(object sender, DragEventArgs e)
        {
            Controller.panAnimationLayers_DragEnter(sender, e);
        }

        public void panAnimationLayers_DragDrop(object sender, DragEventArgs e)
        {
            Controller.panAnimationLayers_DragDrop(sender, e);
        }

        public void panAnimationLayers_DragOver(object sender, DragEventArgs e)
        {
            Controller.panAnimationLayers_DragOver(sender, e);
        }

        #endregion
    }
}
