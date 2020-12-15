using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class AnimationTimelineViewer : UserControl
    {
        private enum TimelineResizeSides { None, All, Left, Right, MoveKeyFrame };

        //Buffer used to draw in the panDrawingSurface.
        private BufferedGraphicsContext panTimelineViewerContext;
        private BufferedGraphics panTimelineViewerGraphicDevice;
        private Graphics panTimelineViewerGraphics;

        private TimelineResizeSides TimelineResizeSide;
        private Timeline ActiveTimelineEvent;

        private MouseEventArgs MouseEventOriginal;
        private MouseEventArgs MouseEventOld;

        private int TimelineStartX;
        private int TimelineHeight;
        private int ScrollbarStartIndex;
        private int ScrollbarWidth;
        private bool ScrollbarSelected;

        private int TimelineDragDropInsertIndex;
        private int TimelineDragDropSelectedItemIndex;
        private int TimelineDragDropSelectedItemIndent;//Used only to know where the Group arrow is located.
        private int TimelineActiveItems;
        private int TimelineVisibleItemCount;

        private int ActiveKeyFrame { get { return ActiveAnimation.ActiveKeyFrame; } set { ActiveAnimation.ActiveKeyFrame = value; } }

        private int MaximumKeyFrame;
        private int PlaybackStartKeyFrame;
        private int PlaybackEndKeyFrame;

        private SolidBrush PlaybackBrush;
        private Font fntTimeline;
        private Font fntListView;

        private AnimationClass.AnimationLayer ActiveLayer;
        private AnimationClassEditor ActiveAnimation;

        public delegate void TimelineChangedHandler();

        public delegate void TimelineSelectedHandler(Timeline SelectedTimelineEvent);

        public delegate void KeyFrameSelectedHandler(AnimationObjectKeyFrame SelectedKeyFrame);

        //SaveCurrentAnimation
        public event TimelineChangedHandler TimelineChanged;

        public event TimelineSelectedHandler TimelineSelected;

        public event KeyFrameSelectedHandler KeyFrameSelected;

        public AnimationTimelineViewer()
        {
            InitializeComponent();

            MaximumKeyFrame = 1200;

            TimelineDragDropInsertIndex = -1;
            TimelineDragDropSelectedItemIndex = -1;
            TimelineStartX = 200;
            TimelineVisibleItemCount = 5;
            ScrollbarStartIndex = 0;
            ScrollbarWidth = 258;

            PlaybackStartKeyFrame = 0;
            PlaybackEndKeyFrame = -1;
            PlaybackBrush = new SolidBrush(Color.FromArgb(120, Color.LightBlue));

            //Create a new buffer based on the picturebox.
            panTimelineViewerGraphics = CreateGraphics();
            this.panTimelineViewerContext = BufferedGraphicsManager.Current;
            this.panTimelineViewerContext.MaximumBuffer = new Size(Width, Height);
            this.panTimelineViewerGraphicDevice = panTimelineViewerContext.Allocate(panTimelineViewerGraphics, new Rectangle(0, 0, Width, Height));

            PlaybackBrush = new SolidBrush(Color.FromArgb(120, Color.LightBlue));
            fntTimeline = new Font("Arial", 6);
            fntListView = new Font("Arial", 8);
        }

        public void Update(int Interval)
        {
            if (GameScreen.FMODSystem != null)
                GameScreen.FMODSystem.System.update();

            ActiveAnimation.Update(Interval);

            //End of Animation, loop back at the beginning.
            if (PlaybackEndKeyFrame >= 0)
            {
                if (ActiveKeyFrame > PlaybackEndKeyFrame)
                    OnKeyFrameChange(PlaybackStartKeyFrame);
            }
            else
            {
                if (ActiveKeyFrame > MaximumKeyFrame)
                    OnKeyFrameChange(0);
            }

            DrawTimeline();
        }

        public void SetActiveAnimation(AnimationClassEditor ActiveAnimation)
        {
            this.ActiveAnimation = ActiveAnimation;
        }

        public void SetActiveLayer(AnimationClass.AnimationLayer ActiveLayer)
        {
            this.ActiveLayer = ActiveLayer;
        }

        public void OnTimelineChanged()
        {
            if (TimelineChanged != null)
                TimelineChanged();
        }

        public void OnTimelineSelected(Timeline ActiveTimelineEvent)
        {
            ActiveAnimation.ListSelectedObjects.Add(ActiveTimelineEvent);

            if (TimelineSelected != null)
                TimelineSelected(ActiveTimelineEvent);

            if (TimelineChanged != null)
                TimelineChanged();
        }

        public void OnKeyFrameSelected(AnimationObjectKeyFrame ActiveKeyFrame)
        {
            if (KeyFrameSelected != null)
                KeyFrameSelected(ActiveKeyFrame);
        }

        #region Events

        public void panTimelineViewer_Resize(object sender, EventArgs e)
        {
            if (ActiveAnimation == null)
                return;

            TimelineHeight = Height - 20;
            TimelineVisibleItemCount = TimelineHeight / 20;

            panTimelineViewerContext.MaximumBuffer = new Size(Width, Height);
            panTimelineViewerGraphicDevice = panTimelineViewerContext.Allocate(panTimelineViewerGraphics, new Rectangle(0, 0, Width, Height));

            //Update Timeline to show the new items.
            UpdateTimelineVisibleItems();
            DrawTimeline();
        }

        private void panTimelineViewer_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventOriginal = e;
            MouseEventOld = e;

            if (e.X < 0)
                return;

            #region Left Button

            if (e.Button == MouseButtons.Left)
            {
                if (e.X < TimelineStartX)
                {
                    TimelineDragDropSelectedItemIndex = -2;
                    int VisibleIndex = 0;

                    for (int i = 0; i < TimelineActiveItems && VisibleIndex < TimelineVisibleItemCount; i++)
                    {
                        ActiveTimelineEvent = GetTimelineEvent(i);

                        //End of TimelineEvent list.
                        if (ActiveTimelineEvent == null)
                            break;

                        if (e.Y >= (VisibleIndex + 1) * 20 && e.Y < (VisibleIndex + 2) * 20 && i >= vsbTimeline.Value)
                        {
                            TimelineDragDropSelectedItemIndex = i;
                        }
                        else
                        {
                            if (Control.ModifierKeys != Keys.Shift)
                            {
                                ActiveAnimation.ListSelectedObjects.Remove(ActiveTimelineEvent);
                                GroupTimeline ActiveGroup = ActiveTimelineEvent as GroupTimeline;

                                if (ActiveGroup != null)
                                    SelectTimelineGroup(ActiveGroup, false);
                            }
                        }
                        if (i >= vsbTimeline.Value)
                            VisibleIndex++;
                    }

                    if (TimelineDragDropSelectedItemIndex >= 0)
                    {
                        ActiveTimelineEvent = GetTimelineEvent(TimelineDragDropSelectedItemIndex);
                        GroupTimeline ActiveGroup = ActiveTimelineEvent as GroupTimeline;

                        if (ActiveGroup != null)
                        {
                            //Expend/Collapse the group
                            if (e.X >= 1 + TimelineDragDropSelectedItemIndent * 5 && e.X < 9 + TimelineDragDropSelectedItemIndent * 5)
                            {
                                ActiveGroup.IsOpen = !ActiveGroup.IsOpen;
                                TimelineDragDropSelectedItemIndex = -2;
                                UpdateTimelineVisibleItems();
                            }
                            else
                            {
                                SelectTimelineGroup(ActiveGroup, true);
                                OnTimelineSelected(ActiveTimelineEvent);
                            }
                        }
                        else
                        {
                            OnTimelineSelected(ActiveTimelineEvent);
                        }
                    }
                    DrawTimeline();
                }
                else if (e.Y <= 20)
                {
                    OnKeyFrameChange((e.X - TimelineStartX + ScrollbarStartIndex) / 8);
                    PlaybackStartKeyFrame = ActiveKeyFrame;
                    PlaybackEndKeyFrame = -1;
                    DrawTimeline();
                }
                else if (e.Y >= Height - 20)
                {
                    ScrollbarSelected = true;
                }
                else if (ActiveTimelineEvent == null)
                {
                    int MouseFrameHalf = (e.X - TimelineStartX + ScrollbarStartIndex + 4) / 8;
                    int MouseEventFrame = (e.X - TimelineStartX + ScrollbarStartIndex) / 8;

                    for (int i = vsbTimeline.Value, VisibleIndex = 0; i < TimelineActiveItems && VisibleIndex < TimelineVisibleItemCount; i++, VisibleIndex++)
                    {
                        if (e.Y < (VisibleIndex + 1) * 20 || e.Y >= (VisibleIndex + 2) * 20)
                            continue;

                        Timeline ActiveEvent = GetTimelineEvent(i);
                        ActiveTimelineEvent = ActiveEvent;

                        AnimationObjectKeyFrame ActiveAnimationObjectKeyFrame = null;

                        if (MouseEventFrame == ActiveEvent.SpawnFrame)
                            TimelineResizeSide = TimelineResizeSides.Left;
                        else if (MouseFrameHalf == ActiveEvent.DeathFrame)
                            TimelineResizeSide = TimelineResizeSides.Right;
                        else if (ActiveEvent.TryGetValue(MouseEventFrame, out ActiveAnimationObjectKeyFrame))
                        {
                            OnKeyFrameSelected(ActiveAnimationObjectKeyFrame);
                            TimelineResizeSide = TimelineResizeSides.MoveKeyFrame;
                        }
                        else if (MouseEventFrame > ActiveEvent.SpawnFrame && MouseEventFrame < ActiveEvent.DeathFrame)
                            TimelineResizeSide = TimelineResizeSides.All;
                    }
                }
            }

            #endregion

            #region Right Button

            else if (e.Button == MouseButtons.Right)
            {
                Timeline ActiveEvent;

                if (e.X < TimelineStartX)
                {
                    for (int i = vsbTimeline.Value, VisibleIndex = 0; i < TimelineActiveItems && VisibleIndex < TimelineVisibleItemCount; i++, VisibleIndex++)
                    {
                        ActiveEvent = GetTimelineEvent(i);

                        if (e.Y < (VisibleIndex + 1) * 20 || e.Y >= (VisibleIndex + 2) * 20)
                        {
                            ActiveAnimation.ListSelectedObjects.Remove(ActiveEvent);
                            continue;
                        }

                        ActiveAnimation.ListSelectedObjects.Add(ActiveEvent);
                        ActiveTimelineEvent = ActiveEvent;
                    }
                    DrawTimeline();
                }
                else
                {
                    ActiveKeyFrame = (e.X - TimelineStartX + ScrollbarStartIndex) / 8;

                    for (int i = vsbTimeline.Value, VisibleIndex = 0; i < TimelineActiveItems && VisibleIndex < TimelineVisibleItemCount; i++, VisibleIndex++)
                    {
                        if (e.Y < (VisibleIndex + 1) * 20 || e.Y >= (VisibleIndex + 2) * 20)
                            continue;

                        ActiveEvent = GetTimelineEvent(i);

                        ActiveTimelineEvent = ActiveEvent;
                    }
                }
            }

            #endregion
        }

        private void panTimelineViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ScrollbarSelected = false;

                if (ActiveTimelineEvent != null && e.X == MouseEventOld.X && e.Y == MouseEventOld.Y)
                {
                    int MouseFrame = (e.X - TimelineStartX + ScrollbarStartIndex) / 8;
                    AnimationObjectKeyFrame Move;

                    if (ActiveTimelineEvent.TryGetValue(MouseFrame, out Move))
                        OnKeyFrameSelected(Move);
                }

                if (TimelineResizeSide != TimelineResizeSides.None)
                    TimelineChanged();

                TimelineResizeSide = TimelineResizeSides.None;
            }
            else
            {
                if (e.X < TimelineStartX)
                {
                    tsmCreateGroup.Visible = true;
                    tsmDeleteItem.Visible = true;
                    tsmDeleteKeyFrame.Visible = false;
                    tsmInsertKeyFrame.Visible = false;

                    cmsTimelineOptions.Show(this, e.Location);
                }
                else if (ActiveTimelineEvent != null)
                {
                    int MouseFrame = (e.X - TimelineStartX + ScrollbarStartIndex) / 8;
                    AnimationObjectKeyFrame Move;

                    if (ActiveTimelineEvent.TryGetValue(MouseFrame, out Move))
                    {
                        tsmCreateGroup.Visible = false;
                        tsmDeleteItem.Visible = false;
                        tsmDeleteKeyFrame.Visible = true;
                        tsmInsertKeyFrame.Visible = false;
                    }
                    else
                    {
                        tsmCreateGroup.Visible = false;
                        tsmDeleteItem.Visible = false;
                        tsmDeleteKeyFrame.Visible = false;
                        tsmInsertKeyFrame.Visible = true;
                    }

                    cmsTimelineOptions.Show(this, e.Location);
                }
            }
        }

        private void panTimelineViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0)
                return;

            int MouseFrame = (e.X - TimelineStartX + ScrollbarStartIndex) / 8;

            #region Left Button

            if (e.Button == MouseButtons.Left)
            {
                int MouseFrameOld = (MouseEventOld.X - TimelineStartX + ScrollbarStartIndex) / 8;
                int MouseFrameOriginal = (MouseEventOriginal.X - TimelineStartX + ScrollbarStartIndex) / 8;

                if (ScrollbarSelected)
                {
                    if (e.X > 20 + TimelineStartX && e.X < Width - 20 && e.Y > TimelineHeight)
                    {
                        int Change = e.X - MouseEventOld.X;
                        HorizontalScrollbarMove(e, Change);
                    }
                }
                else
                {
                    if (e.X < TimelineStartX && TimelineResizeSide == TimelineResizeSides.None)
                    {
                        if (ActiveTimelineEvent != null && TimelineDragDropSelectedItemIndex >= 0)
                        {
                            TimelineDragDropInsertIndex = -2;
                            DoDragDrop(ActiveTimelineEvent, DragDropEffects.Move);
                        }
                    }
                    else if (e.Y <= 20 && TimelineResizeSide == TimelineResizeSides.None)
                    {
                        if (Control.ModifierKeys == Keys.Alt)
                        {
                            if (MouseFrame > PlaybackStartKeyFrame)
                                PlaybackEndKeyFrame = MouseFrame;
                        }
                        else
                            OnKeyFrameChange(MouseFrame);
                    }
                    else if (ActiveTimelineEvent != null)
                    {
                        if (MouseFrame == MouseFrameOld || MouseFrame < ActiveTimelineEvent.SpawnFrame)
                            return;

                        AnimationObjectKeyFrame ExistingKeyFrameObject;

                        #region Move Timeline

                        switch (TimelineResizeSide)
                        {
                            #region Left

                            case TimelineResizeSides.Left:
                                if (MouseFrame >= ActiveTimelineEvent.DeathFrame || MouseFrame < 0)
                                    return;

                                if (ActiveTimelineEvent.TryGetValue(MouseFrame, out ExistingKeyFrameObject))
                                    ActiveTimelineEvent.DeleteKeyFrame(MouseFrame);

                                //Remove the Spawner from the Timeline.
                                ActiveLayer.RemoveTimelineEvent(ActiveTimelineEvent.SpawnFrame, ActiveTimelineEvent);

                                ActiveTimelineEvent.DeleteKeyFrame(MouseFrameOriginal);
                                ActiveTimelineEvent.CreateKeyFrame(ActiveLayer, MouseFrame);
                                MouseEventOriginal = e;

                                //Replace the old Tag.
                                ActiveTimelineEvent.SpawnFrame = MouseFrame;

                                //Add the Spawner to its new Key Frame.
                                ActiveLayer.AddTimelineEvent(ActiveTimelineEvent.SpawnFrame, ActiveTimelineEvent);
                                break;

                            #endregion

                            #region Right

                            case TimelineResizeSides.Right:
                                if (ActiveTimelineEvent.SpawnFrame >= MouseFrame)
                                    return;

                                int NextKeyFrame;
                                ActiveTimelineEvent.GetSurroundingKeyFrames(MouseFrame - 1, out _, out NextKeyFrame);
                                if (NextKeyFrame >= MouseFrame)//Don't reduce the timeline if there's a keyframe there
                                    return;

                                ActiveTimelineEvent.DeathFrame = MouseFrame;

                                //Update the maximum frame.
                                if (ActiveTimelineEvent.DeathFrame > MaximumKeyFrame)
                                {
                                    MaximumKeyFrame = ActiveTimelineEvent.DeathFrame + 1;
                                }
                                break;

                            #endregion

                            #region All

                            case TimelineResizeSides.All:
                                MouseEventOld = e;

                                //Remove the Spawner from the Timeline.
                                ActiveLayer.RemoveTimelineEvent(ActiveTimelineEvent.SpawnFrame, ActiveTimelineEvent);

                                int Difference = MouseFrame - MouseFrameOld;

                                ActiveTimelineEvent.MoveTimeline(Difference);

                                //Add the Spawner to its new Key Frame.
                                ActiveLayer.AddTimelineEvent(ActiveTimelineEvent.SpawnFrame, ActiveTimelineEvent);

                                //Update the maximum frame.
                                if (ActiveTimelineEvent.DeathFrame > MaximumKeyFrame)
                                {
                                    MaximumKeyFrame = ActiveTimelineEvent.DeathFrame + 1;
                                }
                                break;

                            #endregion

                            case TimelineResizeSides.MoveKeyFrame:

                                if (MouseFrame < ActiveTimelineEvent.DeathFrame)
                                {
                                    ActiveTimelineEvent.MoveKeyFrame(MouseFrameOriginal, MouseFrame);
                                    MouseEventOriginal = e;
                                }
                                break;
                        }

                        #endregion

                        TimelineChanged();
                    }
                }
                DrawTimeline();
            }

            #endregion

            else if (e.Button == MouseButtons.Right)
            {
            }
            else
            {
                ActiveTimelineEvent = null;
            }

            if (ActiveTimelineEvent == null)
            {
                if (e.X < TimelineStartX)
                    return;

                for (int i = vsbTimeline.Value, VisibleIndex = 0; i < TimelineActiveItems && VisibleIndex < TimelineVisibleItemCount; i++, VisibleIndex++)
                {
                    if (e.Y < (VisibleIndex + 1) * 20 || e.Y >= (VisibleIndex + 2) * 20)
                        continue;

                    Timeline ItemTag = GetTimelineEvent(i);

                    if (ItemTag != null)
                    {
                        if (MouseFrame == ItemTag.SpawnFrame || MouseFrame == ItemTag.DeathFrame)
                            Cursor.Current = Cursors.VSplit;
                        else if (MouseFrame < ItemTag.DeathFrame)
                            Cursor.Current = Cursors.Hand;
                    }
                }
            }

            MouseEventOld = e;
        }

        private void tsmInsertKeyFrame_Click(object sender, EventArgs e)
        {
            ActiveTimelineEvent.CreateKeyFrame(ActiveLayer, ActiveKeyFrame);
            DrawTimeline();
        }

        private void tsmCreateGroup_Click(object sender, EventArgs e)
        {
            GroupTimeline NewGroup = new GroupTimeline();
            ActiveAnimation.ListTimelineEvent.Add(NewGroup);
            uint InsertIndex = 0;

            while (ActiveLayer.DicGroupEvent.ContainsKey(InsertIndex))
                InsertIndex++;

            NewGroup.KeyValue = InsertIndex;
            ActiveLayer.DicGroupEvent.Add(InsertIndex, NewGroup);
            UpdateTimelineVisibleItems();
            DrawTimeline();
        }

        private void tsmDeleteKeyFrame_Click(object sender, EventArgs e)
        {
            ActiveTimelineEvent.DeleteKeyFrame(ActiveKeyFrame);
            DrawTimeline();
        }

        private void tsmDeleteItem_Click(object sender, EventArgs e)
        {
            ActiveAnimation.ListTimelineEvent.Remove(ActiveTimelineEvent);
            ActiveLayer.RemoveTimelineEvent(ActiveTimelineEvent.SpawnFrame, ActiveTimelineEvent);
            UpdateTimelineVisibleItems();
            DrawTimeline();
        }

        private void panTimelineViewer_DragEnter(object sender, DragEventArgs e)
        {
            //The data from the drag source is moved to the target.
            e.Effect = DragDropEffects.Move;
            TimelineDragDropInsertIndex = -2;
        }

        private void panTimelineViewer_DragDrop(object sender, DragEventArgs e)
        {
            if (TimelineDragDropInsertIndex >= 0)
            {
                Timeline ItemToMove = GetTimelineEvent(TimelineDragDropSelectedItemIndex, true);
                InsertTimelineEvent(TimelineDragDropInsertIndex, ItemToMove, false);
                FinishTimelineEventRemove();
            }
            else if (TimelineDragDropInsertIndex == -1)
            {
                Point MousePos = PointToClient(new Point(e.X, e.Y));

                int NewDragDropInsertIndex = (MousePos.Y - 20) / 21 + vsbTimeline.Value;
                if (NewDragDropInsertIndex < TimelineActiveItems)
                {
                    if (NewDragDropInsertIndex != TimelineDragDropSelectedItemIndex)
                    {
                        GroupTimeline SelectGroup = GetTimelineEvent(NewDragDropInsertIndex) as GroupTimeline;
                        Timeline ItemToMove = GetTimelineEvent(TimelineDragDropSelectedItemIndex, true);

                        if (SelectGroup != null)
                            InsertTimelineEvent(NewDragDropInsertIndex, ItemToMove, true);
                    }
                }
                FinishTimelineEventRemove();
            }
            TimelineDragDropInsertIndex = -1;
            DrawTimeline();
        }

        private void panTimelineViewer_DragOver(object sender, DragEventArgs e)
        {
            Point MousePos = PointToClient(new Point(e.X, e.Y));
            if (MousePos.X < TimelineStartX)
            {
                int NewDragDropInsertIndex = (MousePos.Y - 20) / 21 + vsbTimeline.Value;
                if (NewDragDropInsertIndex < TimelineActiveItems)
                {
                    if (NewDragDropInsertIndex != TimelineDragDropSelectedItemIndex)
                    {
                        GroupTimeline SelectGroup = GetTimelineEvent(NewDragDropInsertIndex) as GroupTimeline;

                        if (SelectGroup != null)
                            TimelineDragDropInsertIndex = -1;
                        else
                            TimelineDragDropInsertIndex = (MousePos.Y - 10) / 21 + vsbTimeline.Value;
                    }
                    else
                        TimelineDragDropInsertIndex = -2;
                }
                else
                    TimelineDragDropInsertIndex = TimelineActiveItems;

                DrawTimeline();
            }
            else
                TimelineDragDropInsertIndex = -2;
        }

        #endregion

        public void OnKeyFrameChange(int NewKeyFrame)
        {
            //Clear the active view and reload everyhing up to NewKeyFrame
            if (NewKeyFrame < ActiveKeyFrame)
            {
                ActiveAnimation.CurrentQuote = "";
                ActiveAnimation.ListActiveSFX.Clear();
                ActiveKeyFrame = NewKeyFrame;

                if (ActiveAnimation.ActiveAnimationBackground != null)
                {
                    ActiveAnimation.ActiveAnimationBackground.ResetCamera();
                }

                for (int L = 0; L < ActiveAnimation.ListAnimationLayer.Count; L++)
                    ActiveAnimation.ListAnimationLayer[L].ResetAnimationLayer();

                for (int i = 0; i <= NewKeyFrame; i++)
                {
                    if (GameScreen.FMODSystem != null)
                        GameScreen.FMODSystem.System.update();

                    ActiveAnimation.UpdateKeyFrame(i);
                }
            }
            else
            {
                for (int i = ActiveKeyFrame; i <= NewKeyFrame; i++)
                {
                    if (GameScreen.FMODSystem != null)
                        GameScreen.FMODSystem.System.update();

                    ActiveAnimation.UpdateKeyFrame(i);
                }
            }
            ActiveKeyFrame = NewKeyFrame;
            // Removed selected objects that are not visible in the current key frame. (Because they were selected from the timline)
            for (int A = ActiveAnimation.ListSelectedObjects.Count - 1; A >= 0; --A)
            {
                VisibleTimeline SelectedVisibleTimeline = ActiveAnimation.ListSelectedObjects[A] as VisibleTimeline;
                if (SelectedVisibleTimeline != null && !ActiveLayer.ListVisibleObject.Contains(SelectedVisibleTimeline))
                {
                    ActiveAnimation.ListSelectedObjects.Remove(SelectedVisibleTimeline);
                    ActiveAnimation.MultipleSelectionRectangle = Microsoft.Xna.Framework.Rectangle.Empty;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawTimeline();
        }

        private void DrawTimeline()
        {
            panTimelineViewerGraphicDevice.Graphics.Clear(Color.White);
            int TimelineWidth = Width - TimelineStartX;

            //Draw Key Frame limit lines.
            for (int X = ScrollbarStartIndex < 8 ? -ScrollbarStartIndex : -ScrollbarStartIndex % 8, i = ScrollbarStartIndex < 8 ? 0 : 5 - (ScrollbarStartIndex / 8) % 5; X < TimelineStartX + TimelineWidth - 1 - TimelineStartX; X += 8)
            {
                if (i == 0)
                {
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, TimelineStartX + X, 0, TimelineStartX + X, 5);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, TimelineStartX + X, 15, TimelineStartX + X, 20);
                    i = 4;
                }
                else
                {
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Gray, TimelineStartX + X, 0, TimelineStartX + X, 5);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Gray, TimelineStartX + X, 15, TimelineStartX + X, 20);
                    i--;
                }
            }
            panTimelineViewerGraphicDevice.Graphics.DrawRectangle(Pens.Black, new Rectangle(TimelineStartX, 0, TimelineWidth - 1, TimelineHeight - 1));
            panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 0, 20, TimelineStartX + TimelineWidth - 1, 20);

            int VisibleIndex = 0;
            int Index = 0;

            if (ActiveAnimation != null)
            {
                for (int i = 0; i < ActiveAnimation.ListTimelineEvent.Count && VisibleIndex < TimelineVisibleItemCount; i++)
                {
                    Timeline ActiveEvent = ActiveAnimation.ListTimelineEvent[i];

                    GroupTimeline ActiveGroup = ActiveEvent as GroupTimeline;

                    if (ActiveGroup != null)
                    {
                        DrawGroup(ref Index, ref VisibleIndex, 0, ActiveGroup);
                    }
                    else
                    {
                        if (Index >= vsbTimeline.Value)
                            DrawTimelineEvent(0, ref VisibleIndex, ActiveEvent);

                        ++Index;
                    }

                    if (TimelineDragDropInsertIndex >= 0)
                        panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 5, 20 + (TimelineDragDropInsertIndex - vsbTimeline.Value) * 21, TimelineStartX - 5, 20 + (TimelineDragDropInsertIndex - vsbTimeline.Value) * 21);
                }

                if (PlaybackEndKeyFrame >= 0)
                {
                    panTimelineViewerGraphicDevice.Graphics.FillRectangle(PlaybackBrush, new Rectangle(TimelineStartX + PlaybackStartKeyFrame * 8 - ScrollbarStartIndex, 0, (PlaybackEndKeyFrame - PlaybackStartKeyFrame) * 8 - ScrollbarStartIndex, TimelineHeight - 1));
                }

                //Draw the timeline cursor.
                panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.LightSalmon, new Rectangle(TimelineStartX + ActiveKeyFrame * 8 - ScrollbarStartIndex, 0, 8, 20));
                panTimelineViewerGraphicDevice.Graphics.DrawRectangle(Pens.Red, new Rectangle(TimelineStartX + ActiveKeyFrame * 8 - ScrollbarStartIndex, 0, 8, 20));
                panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Red, TimelineStartX + ActiveKeyFrame * 8 - ScrollbarStartIndex, 0, TimelineStartX + ActiveKeyFrame * 8 - ScrollbarStartIndex, TimelineHeight - 1);
            }

            //Draw the Key Frames.
            for (int X = -ScrollbarStartIndex % 40 + 1, i = (ScrollbarStartIndex / 40) * 5; X < TimelineWidth - 1; X += 40, i += 5)
                panTimelineViewerGraphicDevice.Graphics.DrawString(i.ToString(), fntTimeline, Brushes.Black, TimelineStartX + X, 6);

            #region Scrollbar

            panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.DarkGray, new Rectangle(TimelineStartX, TimelineHeight, TimelineWidth, 20));
            panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(TimelineStartX + 1, TimelineHeight + 1, 18, 18));
            panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(TimelineStartX + TimelineWidth - 19, TimelineHeight + 1, 18, 18));

            int ScrollbarPos = (int)(ScrollbarWidth / (float)TimelineWidth * ScrollbarStartIndex);
            float ScrollbarActualWidth = Width - TimelineStartX - 40 - ScrollbarWidth;

            ScrollbarPos = (int)(ScrollbarStartIndex * (ScrollbarActualWidth / MaximumKeyFrame));

            panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.LightGray, new Rectangle(TimelineStartX + 20 + ScrollbarPos, TimelineHeight + 1, ScrollbarWidth, 18));

            #endregion

            panTimelineViewerGraphicDevice.Render();
        }

        private void DrawGroup(ref int Index, ref int VisibleIndex, int Indent, GroupTimeline ActiveGroup)
        {
            if (Index >= vsbTimeline.Value)
            {
                if (ActiveAnimation.ListSelectedObjects.Contains(ActiveGroup))
                    panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.CornflowerBlue, new Rectangle(10, 20 + VisibleIndex * 21, 100, 20));

                if (ActiveGroup.IsOpen)
                {
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 1 + Indent * 5, 30 + VisibleIndex * 21, 6 + Indent * 5, 30 + VisibleIndex * 21);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 1 + Indent * 5, 30 + VisibleIndex * 21, 6 + Indent * 5, 25 + VisibleIndex * 21);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 6 + Indent * 5, 25 + VisibleIndex * 21, 6 + Indent * 5, 30 + VisibleIndex * 21);
                }
                else
                {
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 2 + Indent * 5, 22 + VisibleIndex * 21, 6 + Indent * 5, 26 + VisibleIndex * 21);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 2 + Indent * 5, 30 + VisibleIndex * 21, 6 + Indent * 5, 26 + VisibleIndex * 21);
                    panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, 2 + Indent * 5, 22 + VisibleIndex * 21, 2 + Indent * 5, 30 + VisibleIndex * 21);
                }
                panTimelineViewerGraphicDevice.Graphics.DrawString(ActiveGroup.Name, fntListView, Brushes.Black, 10 + Indent * 5, 20 + VisibleIndex * 21);
                ++VisibleIndex;
            }
            Index++;

            if (ActiveGroup.IsOpen)
            {
                for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
                {
                    Timeline ChildEvent = ActiveGroup.ListEvent[i];
                    GroupTimeline ChildGroup = ChildEvent as GroupTimeline;

                    if (ChildGroup != null)
                    {
                        DrawGroup(ref Index, ref VisibleIndex, Indent + 1, ChildGroup);
                    }
                    else
                    {
                        if (Index >= vsbTimeline.Value)
                            DrawTimelineEvent(Indent + 1, ref VisibleIndex, ChildEvent);
                        Index++;
                    }
                }
            }
        }

        private void DrawTimelineEvent(int Indent, ref int VisibleIndex, Timeline ActiveTimeline)
        {
            if (ActiveAnimation.ListSelectedObjects.Contains(ActiveTimeline))
                panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.CornflowerBlue, new Rectangle(10, 20 + VisibleIndex * 21, 100, 20));

            panTimelineViewerGraphicDevice.Graphics.DrawString(ActiveTimeline.Name, fntListView, Brushes.Black, 10 + Indent * 5, 20 + VisibleIndex * 21);
            int StartPos = Math.Max(TimelineStartX, TimelineStartX + ActiveTimeline.SpawnFrame * 8 - ScrollbarStartIndex);
            int VisibleWidth = ActiveTimeline.DeathFrame * 8 - ScrollbarStartIndex;
            VisibleWidth -= StartPos - TimelineStartX;

            if (VisibleWidth > 0)
            {
                VisibleWidth = Math.Min(VisibleWidth, Width);
                //Draw life line.
                panTimelineViewerGraphicDevice.Graphics.FillRectangle(Brushes.Gray, new Rectangle(StartPos, 20 + VisibleIndex * 21, VisibleWidth, 20));
                panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.White, StartPos, 20 + VisibleIndex * 21, StartPos + VisibleWidth, 20 + VisibleIndex * 21);
                panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.White, StartPos, 20 + VisibleIndex * 21, StartPos, 40 + VisibleIndex * 21);
                panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, StartPos + VisibleWidth, 20 + VisibleIndex * 21, StartPos + VisibleWidth, 40 + VisibleIndex * 21);
                panTimelineViewerGraphicDevice.Graphics.DrawLine(Pens.Black, StartPos, 40 + VisibleIndex * 21, StartPos + VisibleWidth, 40 + VisibleIndex * 21);

                ActiveTimeline.DrawKeyFrames(panTimelineViewerGraphicDevice.Graphics, TimelineStartX, ScrollbarStartIndex, VisibleIndex);
            }

            ++VisibleIndex;
        }

        private void HorizontalScrollbarMove(MouseEventArgs e, int Value)
        {
            float ScrollbarActualWidth = Width - TimelineStartX - 40 - ScrollbarWidth;

            ScrollbarStartIndex = Math.Min(MaximumKeyFrame, ScrollbarStartIndex + (int)(Value * (MaximumKeyFrame / ScrollbarActualWidth)));

            if (ScrollbarStartIndex < 0)
                ScrollbarStartIndex = 0;

            int VisibleFrames = (Width - TimelineStartX) / 8;

            if (ScrollbarStartIndex > MaximumKeyFrame * 0.95f)
            {
                MaximumKeyFrame += 100;
                int LargeChange = MaximumKeyFrame / (Width - TimelineStartX - 40);
                ScrollbarWidth = (int)Math.Max(10, ScrollbarActualWidth / 10);
            }
            DrawTimeline();
        }

        private void vsbTimeline_Scroll(object sender, ScrollEventArgs e)
        {
            DrawTimeline();
        }

        private void SelectTimelineGroup(GroupTimeline ActiveGroup, bool IsSelected)
        {
            for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
            {
                if (IsSelected)
                {
                    if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveGroup.ListEvent[i]))
                        ActiveAnimation.ListSelectedObjects.Add(ActiveGroup.ListEvent[i]);
                }
                else
                {
                    ActiveAnimation.ListSelectedObjects.Remove(ActiveGroup.ListEvent[i]);
                }

                GroupTimeline ChildGroup = ActiveGroup.ListEvent[i] as GroupTimeline;

                if (ChildGroup != null)
                    SelectTimelineGroup(ChildGroup, IsSelected);
            }
        }

        public void UpdateTimelineVisibleItems()
        {
            TimelineActiveItems = 0;

            for (int i = 0; i < ActiveAnimation.ListTimelineEvent.Count; i++)
            {
                TimelineActiveItems++;

                GroupTimeline ActiveGroup = ActiveAnimation.ListTimelineEvent[i] as GroupTimeline;

                if (ActiveGroup != null)
                    UpdateTimelineVisibleItems(ActiveGroup);
            }

            if (TimelineActiveItems > TimelineVisibleItemCount)
                vsbTimeline.Maximum = 1 + TimelineActiveItems - TimelineVisibleItemCount;
        }

        private void UpdateTimelineVisibleItems(GroupTimeline ActiveGroup)
        {
            if (!ActiveGroup.IsOpen)
                return;

            for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
            {
                TimelineActiveItems++;

                GroupTimeline ChildGroup = ActiveGroup.ListEvent[i] as GroupTimeline;

                if (ChildGroup != null)
                    UpdateTimelineVisibleItems(ChildGroup);
            }
        }

        private Timeline GetTimelineEvent(int TargetIndex, bool Remove = false)
        {
            int CurrentIndex = 0;
            Timeline TempEvent;

            for (int i = 0; i < ActiveAnimation.ListTimelineEvent.Count; i++)
            {
                if (CurrentIndex == TargetIndex)
                {
                    TimelineDragDropSelectedItemIndent = 0;
                    if (Remove)
                    {
                        TempEvent = ActiveAnimation.ListTimelineEvent[i];
                        ActiveAnimation.ListTimelineEvent[i] = null;
                        return TempEvent;
                    }
                    else
                        return ActiveAnimation.ListTimelineEvent[i];
                }

                CurrentIndex++;
                GroupTimeline ActiveGroup = ActiveAnimation.ListTimelineEvent[i] as GroupTimeline;

                if (ActiveGroup != null)
                {
                    TempEvent = GetTimelineEventFromGroup(TargetIndex, ref CurrentIndex, ActiveGroup, Remove);
                    if (TempEvent != null)
                        return TempEvent;
                }
            }
            return null;
        }

        private Timeline GetTimelineEventFromGroup(int TargetIndex, ref int CurrentIndex, GroupTimeline ActiveGroup, bool Remove, int Indent = 1)
        {
            if (!ActiveGroup.IsOpen)
                return null;

            Timeline TempEvent;

            for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
            {
                if (CurrentIndex == TargetIndex)
                {
                    TimelineDragDropSelectedItemIndent = Indent;
                    if (Remove)
                    {
                        TempEvent = ActiveGroup.ListEvent[i];
                        ActiveGroup.ListEvent[i] = null;
                        return TempEvent;
                    }
                    else
                        return ActiveGroup.ListEvent[i];
                }
                else
                {
                    CurrentIndex++;
                    GroupTimeline ChildGroup = ActiveGroup.ListEvent[i] as GroupTimeline;
                    if (ChildGroup != null)
                    {
                        TempEvent = GetTimelineEventFromGroup(TargetIndex, ref CurrentIndex, ChildGroup, Remove, Indent + 1);
                        if (TempEvent != null)
                            return TempEvent;
                    }
                }
            }
            return null;
        }

        private void InsertTimelineEvent(int TargetIndex, Timeline ActiveEvent, bool InsertInGroup)
        {
            int CurrentIndex = 0;

            for (int i = 0; i < ActiveAnimation.ListTimelineEvent.Count; i++)
            {
                if (ActiveAnimation.ListTimelineEvent[i] == null)
                {
                    if (TargetIndex == i)
                    {
                        break;
                    }
                    else
                    {
                        CurrentIndex++;
                        continue;
                    }
                }

                GroupTimeline ActiveGroup = ActiveAnimation.ListTimelineEvent[i] as GroupTimeline;

                if (CurrentIndex == TargetIndex)
                {
                    if (InsertInGroup && ActiveGroup != null)
                    {
                        ActiveEvent.GroupIndex = (int)ActiveGroup.KeyValue;
                        ActiveGroup.ListEvent.Add(ActiveEvent);
                        return;
                    }
                    else
                    {
                        ActiveEvent.GroupIndex = -1;
                        ActiveAnimation.ListTimelineEvent.Insert(i, ActiveEvent);
                        return;
                    }
                }
                if (ActiveGroup != null)
                {
                    if (InsertTimelineEvent(TargetIndex, ref CurrentIndex, ActiveGroup, ActiveEvent, InsertInGroup))
                        return;
                }
                else
                {
                    CurrentIndex++;
                }
            }
            if (CurrentIndex <= TargetIndex)
            {
                ActiveEvent.GroupIndex = -1;
                ActiveAnimation.ListTimelineEvent.Insert(CurrentIndex, ActiveEvent);
                return;
            }
        }

        private bool InsertTimelineEvent(int TargetIndex, ref int CurrentIndex, GroupTimeline ActiveGroup, Timeline ActiveEvent, bool InsertInGroup)
        {
            if (CurrentIndex == TargetIndex)
            {
                ActiveEvent.GroupIndex = ActiveGroup.GroupIndex;
                ActiveGroup.ListEvent.Insert(0, ActiveEvent);
                return true;
            }

            CurrentIndex++;
            if (!ActiveGroup.IsOpen)
                return false;

            for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
            {
                if (ActiveGroup.ListEvent[i] == null)
                {
                    CurrentIndex++;
                    continue;
                }

                GroupTimeline ChildGroup = ActiveGroup.ListEvent[i] as GroupTimeline;
                if (ChildGroup != null)
                {
                    if (InsertInGroup && CurrentIndex == TargetIndex)
                    {
                        ActiveEvent.GroupIndex = (int)ChildGroup.KeyValue;
                        ChildGroup.ListEvent.Add(ActiveEvent);
                        return true;
                    }
                    else if (InsertTimelineEvent(TargetIndex, ref CurrentIndex, ChildGroup, ActiveEvent, InsertInGroup))
                        return true;
                }
                else
                {
                    if (CurrentIndex == TargetIndex)
                    {
                        ActiveGroup.ListEvent.Insert(i, ActiveEvent);
                        return true;
                    }
                    CurrentIndex++;
                }
            }
            return false;
        }

        private void FinishTimelineEventRemove()
        {
            for (int i = 0; i < ActiveAnimation.ListTimelineEvent.Count; i++)
            {
                if (ActiveAnimation.ListTimelineEvent[i] == null)
                    ActiveAnimation.ListTimelineEvent.RemoveAt(i--);
                else
                {
                    GroupTimeline ActiveGroup = ActiveAnimation.ListTimelineEvent[i] as GroupTimeline;

                    if (ActiveGroup != null)
                        FinishTimelineEventRemove(ActiveGroup);
                }
            }
        }

        private void FinishTimelineEventRemove(GroupTimeline ActiveGroup)
        {
            for (int i = 0; i < ActiveGroup.ListEvent.Count; i++)
            {
                if (ActiveGroup.ListEvent[i] == null)
                    ActiveGroup.ListEvent.RemoveAt(i--);
                else
                {
                    GroupTimeline ChildGroup = ActiveGroup.ListEvent[i] as GroupTimeline;

                    if (ChildGroup != null)
                        FinishTimelineEventRemove(ChildGroup);
                }
            }
        }
    }
}
