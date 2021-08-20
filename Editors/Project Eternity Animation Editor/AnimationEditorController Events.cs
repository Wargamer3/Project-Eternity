using System;
using System.Drawing;
using System.Windows.Forms;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Editors.AnimationEditor
{
    internal partial class AnimationEditorController
    {
        public void ProjectEternityAnimationEditor_Shown(object sender, EventArgs e)
        {
            ProjectEternityAnimationEditor_Resize(sender, e);
            FinalizeLayerSelection(Owner.ActiveAnimation.ListAnimationLayer[0]);
            UpdateAnimationLayerVisibleItems();
            Owner.panTimelineViewer.Refresh();
            DrawAnimationLayers();
            Owner.panTimelineViewer.OnKeyFrameChange(0);
        }

        public void ProjectEternityAnimationEditor_Resize(object sender, EventArgs e)
        {
            if (Owner.ActiveAnimation == null)
                return;

            Owner.AnimationViewer.UpdateOffset(Owner.AnimationViewer.Width, Owner.AnimationViewer.Height);
            Owner.panTimelineViewer.panTimelineViewer_Resize(sender, e);
        }

        public void cbShowBorderBoxes_CheckedChanged(object sender, EventArgs e)
        {
            Owner.AnimationViewer.ShowBorderBoxes = Owner.cbShowBorderBoxes.Checked;
        }

        public void cbShowNextPositions_CheckedChanged(object sender, EventArgs e)
        {
            Owner.AnimationViewer.ShowNextPositions = Owner.cbShowNextPositions.Checked;
        }

        #region Animation Viewer

        public void tmrAnimation_Tick(object sender, EventArgs e)
        {
            Owner.panTimelineViewer.Update(Owner.tmrAnimation.Interval);
        }

        public void btnPlay_Click(object sender, EventArgs e)
        {
            Owner.tmrAnimation.Enabled = true;
            Owner.AnimationViewer.IsPlaying = true;
        }

        public void btnPause_Click(object sender, EventArgs e)
        {
            Owner.tmrAnimation.Enabled = false;
            Owner.AnimationViewer.IsPlaying = false;
        }

        public void btnStop_Click(object sender, EventArgs e)
        {
            Owner.tmrAnimation.Enabled = false;
            Owner.AnimationViewer.IsPlaying = false;
            Owner.panTimelineViewer.OnKeyFrameChange(0);
        }

        #endregion

        #region Animation Layers

        public void btnAddLayer_Click(object sender, EventArgs e)
        {
            Owner.ActiveAnimation.AddLayer(new AnimationClass.AnimationLayer(Owner.ActiveAnimation, "New Layer"));

            UpdateAnimationLayerVisibleItems();
            DrawAnimationLayers();
        }

        public void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            if (Owner.ActiveAnimation.ListAnimationLayer.Count > 1 && ActiveLayer != null)
            {
                Owner.ActiveAnimation.ListAnimationLayer.Remove(ActiveLayer);
                Owner.pgAnimationProperties.SelectedObject = null;
                UpdateAnimationLayerVisibleItems();
                DrawAnimationLayers();
            }
        }

        public void panAnimationLayers_Paint(object sender, PaintEventArgs e)
        {
            DrawAnimationLayers();
        }

        public void vsbAnimationLayer_Scroll(object sender, ScrollEventArgs e)
        {
            DrawAnimationLayers();
        }

        public void panAnimationLayers_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AnimationLayersDragDropSelectedItemIndex = -2;
                int VisibleIndex = 0;

                for (int i = 0; i < AnimationLayersVisibleItemCount && VisibleIndex < AnimationLayersVisibleItemCount; i++)
                {
                    ActiveAnimationLayersEvent = GetAnimationLayer(i);

                    if (e.Y >= (VisibleIndex) * AnimationLayerItemHeight && e.Y < (VisibleIndex + 1) * AnimationLayerItemHeight && i >= Owner.vsbAnimationLayer.Value)
                    {
                        AnimationLayersDragDropSelectedItemIndex = i;
                    }
                    else
                    {
                        if (Control.ModifierKeys != Keys.Shift)
                        {
                            ActiveAnimationLayersEvent.IsSelected = false;
                            SelectAnimationLayer(ActiveAnimationLayersEvent, false);
                        }
                    }
                    if (i >= Owner.vsbAnimationLayer.Value)
                        VisibleIndex++;
                }
                if (AnimationLayersDragDropSelectedItemIndex >= 0)
                {
                    ActiveAnimationLayersEvent = GetAnimationLayer(AnimationLayersDragDropSelectedItemIndex);

                    int PosX = 1 + AnimationLayersDragDropSelectedItemIndent * 5;
                    int NextPosX = PosX + 22;

                    //IsVisible
                    if (e.X >= PosX && e.X < NextPosX)
                    {
                        ActiveAnimationLayersEvent.IsVisible = !ActiveAnimationLayersEvent.IsVisible;
                        AnimationLayersDragDropSelectedItemIndex = -2;
                        UpdateAnimationLayerVisibleItems();
                    }
                    //ShowChildren
                    else if (e.X >= NextPosX && e.X < NextPosX + 10)
                    {
                        ActiveAnimationLayersEvent.ShowChildren = !ActiveAnimationLayersEvent.ShowChildren;
                        AnimationLayersDragDropSelectedItemIndex = -2;
                        UpdateAnimationLayerVisibleItems();
                    }
                    //IsLocked
                    else if (e.X >= NextPosX + 10 && e.X < NextPosX + 32)
                    {
                        ActiveAnimationLayersEvent.IsLocked = !ActiveAnimationLayersEvent.IsLocked;
                        AnimationLayersDragDropSelectedItemIndex = -2;
                        UpdateAnimationLayerVisibleItems();
                    }
                    else
                    {
                        FinalizeLayerSelection(ActiveAnimationLayersEvent);
                    }
                }
                Owner.panTimelineViewer.Refresh();
                DrawAnimationLayers();
            }
        }

        public void panAnimationLayers_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (ActiveAnimationLayersEvent != null && ActiveAnimationLayersEvent != Owner.ActiveAnimation.ListAnimationLayer.EngineLayer)
                {
                    AnimationLayersDragDropInsertIndex = -2;
                    Owner.DoDragDrop(ActiveAnimationLayersEvent, DragDropEffects.Move);
                }
            }
        }

        public void panAnimationLayers_DragEnter(object sender, DragEventArgs e)
        {
            //The data from the drag source is moved to the target.
            e.Effect = DragDropEffects.Move;
            AnimationLayersDragDropInsertIndex = -2;
        }

        public void panAnimationLayers_DragDrop(object sender, DragEventArgs e)
        {
            if (AnimationLayersDragDropInsertIndex >= 0)
            {
                AnimationClass.AnimationLayer ItemToMove = GetAnimationLayer(AnimationLayersDragDropSelectedItemIndex, true);
                InsertAnimationLayers(AnimationLayersDragDropInsertIndex, ItemToMove, false);
                FinishAnimationLayersRemove();
            }
            else if (AnimationLayersDragDropInsertIndex == -1)
            {
                Point MousePos = Owner.panAnimationLayers.PointToClient(new Point(e.X, e.Y));

                int NewDragDropInsertIndex = MousePos.Y / AnimationLayerItemHeight + Owner.vsbAnimationLayer.Value;
                if (NewDragDropInsertIndex < AnimationLayersVisibleItemCount)
                {
                    if (NewDragDropInsertIndex != AnimationLayersDragDropSelectedItemIndex)
                    {
                        AnimationClass.AnimationLayer SelectItem = GetAnimationLayer(NewDragDropInsertIndex);
                        AnimationClass.AnimationLayer ItemToMove = GetAnimationLayer(AnimationLayersDragDropSelectedItemIndex, true);
                        InsertAnimationLayers(NewDragDropInsertIndex, ItemToMove, true);
                    }
                }
                FinishAnimationLayersRemove();
            }
            AnimationLayersDragDropInsertIndex = -1;
            DrawAnimationLayers();
        }

        public void panAnimationLayers_DragOver(object sender, DragEventArgs e)
        {
            Point MousePos = Owner.panAnimationLayers.PointToClient(new Point(e.X, e.Y));
            int NewDragDropInsertIndex = MousePos.Y / AnimationLayerItemHeight + Owner.vsbAnimationLayer.Value;

            if (NewDragDropInsertIndex < AnimationLayersVisibleItemCount - 1)
            {
                if (NewDragDropInsertIndex != AnimationLayersDragDropSelectedItemIndex)
                {
                    int YValue = MousePos.Y % AnimationLayerItemHeight;
                    if (YValue < AnimationLayerItemHeight * 0.25f || YValue > AnimationLayerItemHeight * 0.75f)
                    {
                        AnimationLayersDragDropInsertIndex = (MousePos.Y + AnimationLayerItemHeight / 2) / AnimationLayerItemHeight + Owner.vsbAnimationLayer.Value;
                    }
                    else
                    {
                        AnimationLayersDragDropInsertIndex = -1;
                    }
                }
                else
                {
                    AnimationLayersDragDropInsertIndex = -2;
                }
            }
            else
            {
                AnimationLayersDragDropInsertIndex = AnimationLayersVisibleItemCount - 1;
            }

            DrawAnimationLayers();
        }

        #endregion
    }
}
