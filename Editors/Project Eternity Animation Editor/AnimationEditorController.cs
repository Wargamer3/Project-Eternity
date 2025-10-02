using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationEditor
{
    internal partial class AnimationEditorController
    {//Can you do that thing where you press left click and then drag it to select multiple things
        //A special layer just for SFX, quotes, background, etc
        //http://imgur.com/a/rbxH3
        // When you click on a bitmap
        //[20:39:02] <Finlander> You could also have stuff like the drawing depth and alpha available in the menu

        private enum TimelineResizeSides { None, All, Left, Right, MoveKeyFrame };

        private enum BitmapActions { None, Move, Rotate, ScaleX, ScaleY, ScaleAll, Origin, Node };

        private enum AxisLocks { None, X, Y };

        #region Variables

        //Buffer used to draw in the Owner.panAnimationLayers.
        private BufferedGraphicsContext panAnimationLayersContext;

        private BufferedGraphics panAnimationLayersGraphicDevice;
        private Graphics panAnimationLayersGraphics;

        private Font fntTimeline;
        private Font fntListView;
        private AnimationClass.AnimationLayer ActiveAnimationLayersEvent;

        private int ActiveKeyFrame { get { return Owner.ActiveAnimation.ActiveKeyFrame; } set { Owner.ActiveAnimation.ActiveKeyFrame = value; } }

        private AnimationClass.AnimationLayer ActiveLayer;

        private MouseEventArgs MouseEventOriginal = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        private MouseEventArgs MouseEventOld = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        private int AnimationLayersDragDropInsertIndex;
        private int AnimationLayersDragDropSelectedItemIndex;
        private int AnimationLayersDragDropSelectedItemIndent;//Used only to know where the Group arrow is located.
        private int AnimationLayersVisibleItemCount;
        private int AnimationLayerItemHeight;
        private Image imgEyeOpen;
        private Image imgEyeClosed;
        private Image imgUnlocked;
        private Image imgLocked;

        #endregion

        public ProjectEternityAnimationEditor Owner;

        public AnimationEditorController(ProjectEternityAnimationEditor Owner)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            #region Owner.cbShowBorderBoxes

            //Init the ShowGrid button (as it can't be done with the tool box)
            Owner.AnimationViewer.ShowBorderBoxes = true;
            Owner.cbShowBorderBoxes = new CheckBox();
            Owner.cbShowBorderBoxes.Text = "Show border boxes";
            //Link a CheckedChanged event to a method.
            Owner.cbShowBorderBoxes.CheckedChanged += new EventHandler(cbShowBorderBoxes_CheckedChanged);
            Owner.cbShowBorderBoxes.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            Owner.cbShowBorderBoxes.Padding = new Padding(10, 0, 0, 0);
            Owner.mnuToolBar.Items.Add(new ToolStripControlHost(Owner.cbShowBorderBoxes));

            #endregion

            #region Owner.cbShowNextPositions

            //Init the ShowGrid button (as it can't be done with the tool box)
            Owner.AnimationViewer.ShowNextPositions = true;
            Owner.cbShowNextPositions = new CheckBox();
            Owner.cbShowNextPositions.Text = "Show Next Positions";
            Owner.cbShowNextPositions.AutoSize = false;
            //Link a CheckedChanged event to a method.
            Owner.cbShowNextPositions.CheckedChanged += new EventHandler(cbShowNextPositions_CheckedChanged);
            Owner.cbShowNextPositions.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            Owner.cbShowNextPositions.Padding = new Padding(10, 0, 0, 0);
            ToolStripControlHost s = new ToolStripControlHost(Owner.cbShowNextPositions);
            s.AutoSize = false;
            s.Width = 150;
            Owner.mnuToolBar.Items.Add(s);

            #endregion

            if (GameScreens.GameScreen.FMODSystem == null)
            {
                try
                {
                    GameScreens.GameScreen.FMODSystem = new FMOD.SoundSystem();
                }
                catch (Exception)
                {
                }
            }

            AnimationLayersDragDropInsertIndex = -1;
            AnimationLayersDragDropSelectedItemIndex = -1;
            AnimationLayerItemHeight = 21;

            imgEyeOpen = Bitmap.FromFile("Content/Editors/Eye Open.png");
            imgEyeClosed = Bitmap.FromFile("Content/Editors/Eye Closed.png");
            imgUnlocked = Bitmap.FromFile("Content/Editors/Unlocked.png");
            imgLocked = Bitmap.FromFile("Content/Editors/Locked.png");

            //Create a new buffer based on the pannel.
            panAnimationLayersGraphics = Owner.panAnimationLayers.CreateGraphics();
            panAnimationLayersContext = BufferedGraphicsManager.Current;
            panAnimationLayersContext.MaximumBuffer = new Size(Owner.panAnimationLayers.Width, Owner.panAnimationLayers.Height);
            panAnimationLayersGraphicDevice = panAnimationLayersContext.Allocate(panAnimationLayersGraphics, new Rectangle(0, 0, Owner.panAnimationLayers.Width, Owner.panAnimationLayers.Height));

            fntTimeline = new Font("Arial", 6);
            fntListView = new Font("Arial", 8);
        }

        public void Undo()
        {
            if (Owner.AnimationViewer.ListOldAnimation.Count > 1)
            {
                Owner.AnimationViewer.Undo();
                Owner.ActiveAnimation = Owner.AnimationViewer.ActiveAnimation;
                Owner.panTimelineViewer.SetActiveAnimation(Owner.AnimationViewer.ActiveAnimation);
                FinalizeLayerSelection(Owner.ActiveAnimation.ListAnimationLayer[0]);
                Owner.panTimelineViewer.UpdateTimelineVisibleItems();
                UpdateAnimationLayerVisibleItems();

                Owner.panTimelineViewer.Refresh();
                DrawAnimationLayers();
                int CurrentKeyFrame = ActiveKeyFrame;
                ActiveKeyFrame = 0;
                Owner.panTimelineViewer.OnKeyFrameChange(CurrentKeyFrame);

                if (Owner.AnimationViewer.ListOldAnimation.Count <= 1)
                    Owner.tsmUndo.Enabled = false;
            }
        }

        public void SaveCurrentAnimation()
        {
            Owner.AnimationViewer.SaveCurrentAnimation();
            if (Owner.AnimationViewer.ListOldAnimation.Count > 1)
                Owner.tsmUndo.Enabled = true;
        }

        #region Animation Layers

        public void DrawAnimationLayers()
        {
            panAnimationLayersGraphicDevice.Graphics.Clear(Color.White);

            int Index = 0;
            int VisibleIndex = 0;

            for (int L = 0; L < Owner.ActiveAnimation.ListAnimationLayer.Count; L++)
            {
                DrawLayer(ref Index, 0, ref VisibleIndex, Owner.ActiveAnimation.ListAnimationLayer[L]);

                if (Index < Owner.vsbAnimationLayer.Value)
                {
                    Index++;
                    continue;
                }

                if (AnimationLayersDragDropInsertIndex >= 0)
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, 5, (AnimationLayersDragDropInsertIndex - Owner.vsbAnimationLayer.Value) * AnimationLayerItemHeight, 200, (AnimationLayersDragDropInsertIndex - Owner.vsbAnimationLayer.Value) * AnimationLayerItemHeight);
            }

            panAnimationLayersGraphicDevice.Render();
        }

        public void DrawLayer(ref int Index, int Indent, ref int VisibleIndex, AnimationClass.AnimationLayer ActiveEvent)
        {
            if (Index >= Owner.vsbAnimationLayer.Value)
            {
                int PosX = 1 + Indent * 5;
                int PosY = VisibleIndex * AnimationLayerItemHeight;

                if (ActiveEvent.IsSelected)
                    panAnimationLayersGraphicDevice.Graphics.FillRectangle(Brushes.CornflowerBlue, new Rectangle(0, PosY, Owner.panAnimationLayers.Width - 10, 20));

                if (ActiveEvent.IsVisible)
                    panAnimationLayersGraphicDevice.Graphics.DrawImage(imgEyeOpen, PosX, PosY - 1);
                else
                    panAnimationLayersGraphicDevice.Graphics.DrawImage(imgEyeClosed, PosX, PosY - 1);

                PosX += 22;

                if (ActiveEvent.ShowChildren)
                {
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, PosX, 10 + PosY, 5 + PosX, 10 + PosY);
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, PosX, 10 + PosY, 5 + PosX, 5 + PosY);
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, 5 + PosX, 5 + PosY, 5 + PosX, 10 + PosY);
                }
                else
                {
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, 1 + PosX, 2 + PosY, 5 + PosX, 6 + PosY);
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, 1 + PosX, 10 + PosY, 5 + PosX, 6 + PosY);
                    panAnimationLayersGraphicDevice.Graphics.DrawLine(Pens.Black, 1 + PosX, 2 + PosY, 1 + PosX, 10 + PosY);
                }

                PosX += 10;

                if (ActiveEvent.IsLocked)
                    panAnimationLayersGraphicDevice.Graphics.DrawImage(imgLocked, PosX, PosY - 1);
                else
                    panAnimationLayersGraphicDevice.Graphics.DrawImage(imgUnlocked, PosX, PosY - 1);

                PosX += 22;

                panAnimationLayersGraphicDevice.Graphics.DrawString(ActiveEvent.Name, fntListView, Brushes.Black, 10 + PosX, 4 + PosY);
                VisibleIndex++;
            }
            Index++;
            if (ActiveEvent.ShowChildren)
            {
                for (int L = 0; L < ActiveEvent.ListChildren.Count; L++)
                {
                    DrawLayer(ref Index, Indent + 1, ref VisibleIndex, ActiveEvent.ListChildren[L]);
                }
            }
        }

        public void FinalizeLayerSelection(AnimationClass.AnimationLayer ActiveLayerSelection)
        {
            Owner.pgAnimationProperties.SelectedObject = ActiveLayerSelection;
            ActiveLayer = ActiveLayerSelection;
            Owner.panTimelineViewer.SetActiveLayer(ActiveLayer);
            Owner.AnimationViewer.SetActiveLayer(ActiveLayer);
            ActiveLayerSelection.IsSelected = true;

            Owner.ActiveAnimation.ListTimelineEvent.Clear();
            foreach (KeyValuePair<uint, GroupTimeline> ActiveListGroups in ActiveLayerSelection.DicGroupEvent)
                ActiveListGroups.Value.ListEvent.Clear();

            foreach (KeyValuePair<uint, GroupTimeline> ActiveListGroups in ActiveLayerSelection.DicGroupEvent)
            {
                if (ActiveListGroups.Value.GroupIndex == -1)
                    Owner.ActiveAnimation.ListTimelineEvent.Add(ActiveListGroups.Value);
                else
                    ActiveLayerSelection.DicGroupEvent[(uint)ActiveListGroups.Value.GroupIndex].ListEvent.Add(ActiveListGroups.Value);
            }

            foreach (KeyValuePair<int, List<Timeline>> ActiveListEvents in ActiveLayerSelection.DicTimelineEvent)
            {
                foreach (Timeline ActiveEvent in ActiveListEvents.Value)
                {
                    GroupTimeline ActiveGroup = null;
                    if (ActiveEvent.GroupIndex >= 0 && ActiveLayerSelection.DicGroupEvent.TryGetValue((uint)ActiveEvent.GroupIndex, out ActiveGroup))
                        ActiveGroup.ListEvent.Add(ActiveEvent);
                    else
                        Owner.ActiveAnimation.ListTimelineEvent.Add(ActiveEvent);
                }
            }

            Owner.panTimelineViewer.UpdateTimelineVisibleItems();
        }

        public void SelectAnimationLayer(AnimationClass.AnimationLayer ActiveLayer, bool IsSelected)
        {
            for (int i = 0; i < ActiveLayer.ListChildren.Count; i++)
            {
                ActiveLayer.ListChildren[i].IsSelected = IsSelected;
                SelectAnimationLayer(ActiveLayer.ListChildren[i], IsSelected);
            }
        }

        public void UpdateAnimationLayerVisibleItems()
        {
            AnimationLayersVisibleItemCount = 0;

            for (int i = 0; i < Owner.ActiveAnimation.ListAnimationLayer.Count; i++)
            {
                AnimationLayersVisibleItemCount++;
                UpdateAnimationLayerVisibleItems(Owner.ActiveAnimation.ListAnimationLayer[i]);
            }

            if (AnimationLayersVisibleItemCount > 5)
            {
                Owner.vsbAnimationLayer.Maximum = AnimationLayersVisibleItemCount - 5;
                Owner.vsbAnimationLayer.Visible = true;
            }
            else
                Owner.vsbAnimationLayer.Visible = false;
        }

        public void UpdateAnimationLayerVisibleItems(AnimationClass.AnimationLayer ActiveGroup)
        {
            if (!ActiveGroup.ShowChildren)
                return;

            for (int i = 0; i < ActiveGroup.ListChildren.Count; i++)
            {
                AnimationLayersVisibleItemCount++;
                UpdateAnimationLayerVisibleItems(ActiveGroup.ListChildren[i]);
            }
        }

        public AnimationClass.AnimationLayer GetAnimationLayer(int TargetIndex, bool Remove = false)
        {
            int CurrentIndex = 0;
            AnimationClass.AnimationLayer TempLayer;

            for (int L = 0; L < Owner.ActiveAnimation.ListAnimationLayer.Count; L++)
            {
                if (CurrentIndex == TargetIndex)
                {
                    AnimationLayersDragDropSelectedItemIndent = 0;
                    if (Remove)
                    {
                        TempLayer = Owner.ActiveAnimation.ListAnimationLayer[L];
                        Owner.ActiveAnimation.ListAnimationLayer[L] = null;
                        return TempLayer;
                    }
                    else
                        return Owner.ActiveAnimation.ListAnimationLayer[L];
                }

                CurrentIndex++;
                TempLayer = GetAnimationLayer(TargetIndex, ref CurrentIndex, Owner.ActiveAnimation.ListAnimationLayer[L], Remove);
                if (TempLayer != null)
                    return TempLayer;
            }
            return null;
        }

        public AnimationClass.AnimationLayer GetAnimationLayer(int TargetIndex, ref int CurrentIndex, AnimationClass.AnimationLayer ActiveLayer, bool Remove, int Indent = 1)
        {
            AnimationClass.AnimationLayer TempLayer;

            for (int i = 0; i < ActiveLayer.ListChildren.Count; i++)
            {
                if (CurrentIndex == TargetIndex)
                {
                    AnimationLayersDragDropSelectedItemIndent = Indent;
                    if (Remove)
                    {
                        TempLayer = ActiveLayer.ListChildren[i];
                        ActiveLayer.ListChildren[i] = null;
                        return TempLayer;
                    }
                    else
                        return ActiveLayer.ListChildren[i];
                }
                else
                {
                    CurrentIndex++;
                    TempLayer = GetAnimationLayer(TargetIndex, ref CurrentIndex, ActiveLayer.ListChildren[i], Remove, Indent + 1);
                    if (TempLayer != null)
                        return TempLayer;
                }
            }
            return null;
        }

        public void InsertAnimationLayers(int TargetIndex, AnimationClass.AnimationLayer ActiveEvent, bool InsertInGroup)
        {
            int CurrentIndex = 0;

            for (int i = 0; i < Owner.ActiveAnimation.ListAnimationLayer.Count; i++)
            {
                if (Owner.ActiveAnimation.ListAnimationLayer[i] == null)
                {
                    CurrentIndex++;
                    continue;
                }
                if (CurrentIndex == TargetIndex)
                {
                    if (InsertInGroup && Owner.ActiveAnimation.ListAnimationLayer[i] != Owner.ActiveAnimation.ListAnimationLayer.EngineLayer)
                    {
                        Owner.ActiveAnimation.ListAnimationLayer[i].ListChildren.Add(ActiveEvent);
                        return;
                    }
                    else
                    {
                        Owner.ActiveAnimation.ListAnimationLayer.Insert(i, ActiveEvent);
                        return;
                    }
                }
                if (InsertAnimationLayers(TargetIndex, ref CurrentIndex, Owner.ActiveAnimation.ListAnimationLayer[i], ActiveEvent, InsertInGroup))
                    return;
            }
            if (CurrentIndex <= TargetIndex)
            {
                Owner.ActiveAnimation.ListAnimationLayer.Add(ActiveEvent);
                return;
            }
        }

        public bool InsertAnimationLayers(int TargetIndex, ref int CurrentIndex, AnimationClass.AnimationLayer ActiveGroup, AnimationClass.AnimationLayer ActiveEvent, bool InsertInGroup)
        {
            if (CurrentIndex == TargetIndex)
            {
                ActiveGroup.ListChildren.Insert(0, ActiveEvent);
                return true;
            }

            CurrentIndex++;
            if (!ActiveGroup.ShowChildren)
                return false;

            for (int i = 0; i < ActiveGroup.ListChildren.Count; i++)
            {
                if (ActiveGroup.ListChildren[i] == null)
                {
                    CurrentIndex++;
                    continue;
                }

                if (InsertInGroup && CurrentIndex == TargetIndex)
                {
                    ActiveGroup.ListChildren[i].ListChildren.Add(ActiveEvent);
                    return true;
                }
                else if (InsertAnimationLayers(TargetIndex, ref CurrentIndex, ActiveGroup.ListChildren[i], ActiveEvent, InsertInGroup))
                    return true;
            }
            return false;
        }

        public void FinishAnimationLayersRemove()
        {
            for (int i = 0; i < Owner.ActiveAnimation.ListAnimationLayer.Count; i++)
            {
                if (Owner.ActiveAnimation.ListAnimationLayer[i] == null)
                    Owner.ActiveAnimation.ListAnimationLayer.RemoveAt(i--);
                else
                    FinishAnimationLayersRemove(Owner.ActiveAnimation.ListAnimationLayer[i]);
            }
        }

        public void FinishAnimationLayersRemove(AnimationClass.AnimationLayer ActiveGroup)
        {
            for (int i = 0; i < ActiveGroup.ListChildren.Count; i++)
            {
                if (ActiveGroup.ListChildren[i] == null)
                    ActiveGroup.ListChildren.RemoveAt(i--);
                else
                    FinishAnimationLayersRemove(ActiveGroup.ListChildren[i]);
            }
        }

        #endregion
    }
}
