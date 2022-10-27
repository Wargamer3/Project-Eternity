using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class AnimationViewerControl : GraphicsDeviceControl
    {
        private enum SelectionChoices { None, Camera }

        private enum BitmapActions { None, Move, Rotate, ScaleX, ScaleY, ScaleAll, Origin, Extra };

        private enum AxisLocks { None, X, Y };

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        private BitmapActions BitmapAction;
        private double OriginalRotatingAngle;
        private double OldRotatingAngle;
        private Microsoft.Xna.Framework.Vector2 OriginalScaleVector;
        private Point OriginScalePosition;
        private AxisLocks AxisLock;

        public AnimationClassEditor ActiveAnimation;
        private AnimationClass.AnimationLayer ActiveLayer;
        public List<AnimationClassEditor> ListOldAnimation;
        public Microsoft.Xna.Framework.Content.ContentManager content;
        private CustomSpriteBatch g;
        public bool IsPlaying;
        public bool ShowBorderBoxes;
        public bool ShowNextPositions;
        private SelectionChoices SelectionChoice;

        private int ActiveKeyFrame { get { return ActiveAnimation.ActiveKeyFrame; } set { ActiveAnimation.ActiveKeyFrame = value; } }

        #region Cursors

        public Cursor RotateUp;
        public Cursor RotateDown;
        public Cursor RotateLeft;
        public Cursor RotateRight;
        public Cursor RotateUpLeft;
        public Cursor RotateUpRight;
        public Cursor RotateDownLeft;
        public Cursor RotateDownRight;

        #endregion

        //ViewportOffset is used to allow the user to move objects outside the Viewport.
        private Microsoft.Xna.Framework.Point ViewportOffset;

        private Microsoft.Xna.Framework.Point ZoomCenter;
        public Microsoft.Xna.Framework.Point Camera;
        public float Zoom;
        public Microsoft.Xna.Framework.Matrix MouseTransformationMatrix;

        private MouseEventArgs MouseEventOriginal = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        private MouseEventArgs MouseEventOld = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        public delegate void TimelineChangedHandler();

        public delegate void LayersChangedHandler();

        public delegate void TimelineSelectedHandler(Timeline SelectedTimeline);

        public delegate void KeyFrameSelectedHandler(int SelectedKeyFrame);

        public event TimelineChangedHandler TimelineChanged;

        public event TimelineChangedHandler TimelineSelectionChanged;

        public event LayersChangedHandler LayersChanged;

        public event TimelineSelectedHandler TimelineSelected;

        public AnimationViewerControl()
        {
            InitializeComponent();
        }

        private void OnTimelineSelected(Timeline SelectedTimeline)
        {
            TimelineSelected(SelectedTimeline);
        }

        private void OnLayersChanged()
        {
            LayersChanged();
        }

        private void OnTimelineChanged()
        {
            TimelineChanged();
        }

        private void OnTimelineSelectionChanged()
        {
            TimelineSelectionChanged();
        }

        protected override void Initialize()
        {
            Bitmap b = new Bitmap("Content/Cursors/UpLeft.png");
            RotateUpLeft = CreateCursor(b, 4, 4);
            b = new Bitmap("Content/Cursors/UpRight.png");
            RotateUpRight = CreateCursor(b, 13, 4);
            b = new Bitmap("Content/Cursors/DownLeft.png");
            RotateDownLeft = CreateCursor(b, 4, 13);
            b = new Bitmap("Content/Cursors/DownRight.png");
            RotateDownRight = CreateCursor(b, 13, 13);
            b = new Bitmap("Content/Cursors/Up.png");
            RotateUp = CreateCursor(b, 13, 13);
            b = new Bitmap("Content/Cursors/Down.png");
            RotateDown = CreateCursor(b, 13, 13);
            b = new Bitmap("Content/Cursors/Left.png");
            RotateLeft = CreateCursor(b, 13, 13);
            b = new Bitmap("Content/Cursors/Right.png");
            RotateRight = CreateCursor(b, 13, 13);

            Zoom = 1;
            IsPlaying = false;
            ShowBorderBoxes = true;
            ListOldAnimation = new List<AnimationClassEditor>();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = this.Handle;

            content = new Microsoft.Xna.Framework.Content.ContentManager(Services, "Content");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        private static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }

        public void Preload()
        {
            OnCreateControl();
        }

        #region Spawn object

        public void tsmNewItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem Sender = (ToolStripMenuItem)sender;
            Timeline SelectedTimeline = (Timeline)Sender.Tag;

            List<VisibleTimeline> ListNewItem = SelectedTimeline.CreateNewEditorItem(ActiveAnimation, ActiveLayer, ActiveKeyFrame, MouseToAnimationCoords(MouseEventOld.Location));

            foreach (var NewItem in ListNewItem)
            {
                OnTimelineSelected(NewItem);
                ActiveAnimation.ListTimelineEvent.Add(NewItem);

                ActiveLayer.AddTimelineEvent(ActiveKeyFrame, NewItem);
                ActiveLayer.ListVisibleObject.Add(NewItem);
            }

            OnTimelineSelectionChanged();
            OnTimelineChanged();
        }

        public void tsmAnimation_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem("Animations", "Select an animation to import", true));
        }

        public void tsmEditPolygon_Click(object sender, EventArgs e)
        {
            foreach (Timeline ActiveTimeline in ActiveAnimation.ListSelectedObjects)
            {
                if (ActiveTimeline is PolygonCutterTimeline)
                {
                    PolygonCutterHelper NewSpawner = new PolygonCutterHelper(ActiveLayer.renderTarget,
                        ((PolygonCutterTimeline)ActiveTimeline).ListPolygon, false);

                    if (NewSpawner.ShowDialog() == DialogResult.OK)
                    {
                        ActiveTimeline.CreateOrRetriveKeyFrame(ActiveLayer, ActiveKeyFrame);
                    }
                }
                return;
            }
        }

        public void tsmEditPolygonCutterOrigin_Click(object sender, EventArgs e)
        {
            foreach (Timeline ActiveTimeline in ActiveAnimation.ListSelectedObjects)
            {
                if (ActiveTimeline is PolygonCutterTimeline)
                {
                    PolygonCutterHelper NewSpawner = new PolygonCutterHelper(ActiveLayer.renderTarget,
                    ((PolygonCutterTimeline)ActiveTimeline).ListPolygon, true);

                    if (NewSpawner.ShowDialog() == DialogResult.OK)
                    {
                        PolygonCutterTimeline.PolygonCutterKeyFrame ActiveKeyFrame = (PolygonCutterTimeline.PolygonCutterKeyFrame)ActiveTimeline.CreateOrRetriveKeyFrame(ActiveLayer, ActiveTimeline.SpawnFrame);
                        ActiveKeyFrame.ListPolygon = NewSpawner.PolygonCutterViewer.ListPolygon;
                    }
                    return;
                }
            }
        }

        #endregion

        #region Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();

            if (Control.ModifierKeys == Keys.Alt)
            {
                SelectionChoice = SelectionChoices.Camera;
            }

            if (ActiveLayer.IsLocked)
                return;

            Point Real = MouseToAnimationCoords(e.Location);

            #region Left Click

            if (e.Button == MouseButtons.Left)
            {
                if (Control.ModifierKeys != Keys.Shift && ActiveAnimation.MultipleSelectionRectangle.Width > 0)
                {
                    int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                    int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;

                    if (BitmapAction == BitmapActions.Rotate)
                    {
                        OldRotatingAngle = Math.Atan2(Real.Y - OriginY, Real.X - OriginX);
                        if (OldRotatingAngle < 0)
                            OldRotatingAngle += Math.PI * 2;
                        OriginalRotatingAngle = OldRotatingAngle;
                    }
                    else if (BitmapAction == BitmapActions.ScaleAll || BitmapAction == BitmapActions.ScaleX || BitmapAction == BitmapActions.ScaleY)
                    {
                        OriginalScaleVector = new Microsoft.Xna.Framework.Vector2(Real.X - ActiveAnimation.MultipleSelectionRectangle.X - ActiveAnimation.MultipleSelectionOrigin.X,
                                                                                  Real.Y - ActiveAnimation.MultipleSelectionRectangle.Y - ActiveAnimation.MultipleSelectionOrigin.Y);
                        OriginalScaleVector.Normalize();
                        OriginScalePosition = new Point(Real.X, Real.Y);
                    }
                }

                if (BitmapAction == BitmapActions.None || (BitmapAction == BitmapActions.Move && ActiveAnimation.ListSelectedObjects.Count == 1 && Control.ModifierKeys == Keys.Control))
                {
                    #region Check for Extras

                    for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                    {
                        if (ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListVisibleObject[A]))
                        {
                            if (ActiveLayer.ListVisibleObject[A].MouseDownExtra(Real.X, Real.Y))
                            {
                                BitmapAction = BitmapActions.Extra;
                            }
                        }
                    }

                    for (int A = 0; A < ActiveLayer.ListPolygonCutter.Count; A++)
                    {
                        if (ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListPolygonCutter[A]))
                        {
                            if (ActiveLayer.ListPolygonCutter[A].MouseDownExtra(Real.X, Real.Y))
                            {
                                BitmapAction = BitmapActions.Extra;
                            }
                        }
                    }

                    #endregion

                    if (BitmapAction == BitmapActions.None)
                    {
                        ActiveAnimation.MultipleSelectionRectangle = Microsoft.Xna.Framework.Rectangle.Empty;

                        if (BitmapAction == BitmapActions.None)
                        {
                            SelectItems(Real.X, Real.Y);
                        }

                        int BitmapsSelectedCount = ActiveAnimation.ListSelectedObjects.Count;

                        if (BitmapsSelectedCount > 0)
                        {
                            if (BitmapsSelectedCount == 1 && BitmapAction == BitmapActions.None)
                                BitmapAction = BitmapActions.Move;

                            OnTimelineChanged();
                        }
                    }
                }
            }

            #endregion

            #region Right Click

            else if (e.Button == MouseButtons.Right)
            {
                ActiveAnimation.ListSelectedObjects.Clear();

                if (ActiveLayer == ActiveAnimation.ListAnimationLayer.EngineLayer)
                    return;

                tsmEditOrigin.Visible = false;

                tsmEditPolygon.Visible = false;
                tsmEditPolygonCutterOrigin.Visible = false;

                for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                {
                    if (ActiveLayer.ListVisibleObject[A].CanSelect(Real.X, Real.Y))
                    {
                        ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListVisibleObject[A]);
                        BitmapAction = BitmapActions.Move;
                        tsmEditOrigin.Visible = true;
                    }
                }
                for (int P = 0; P < ActiveLayer.ListPolygonCutter.Count; P++)
                {
                    if (ActiveLayer.ListPolygonCutter[P].CanSelect(Real.X, Real.Y))
                    {
                        ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListPolygonCutter[P]);
                        BitmapAction = BitmapActions.Move;
                        tsmEditPolygon.Visible = true;
                        tsmEditPolygonCutterOrigin.Visible = true;
                    }
                }

                cmsAnimationViewerOptions.Show(this, PointToClient(Cursor.Position));
            }

            #endregion

            Draw();

            MouseEventOriginal = e;
            MouseEventOld = e;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (ActiveLayer.IsLocked)
                return;

            if (AxisLock == AxisLocks.X)
                e = new MouseEventArgs(e.Button, e.Clicks, MouseEventOld.X, e.Y, e.Delta);
            else if (AxisLock == AxisLocks.Y)
                e = new MouseEventArgs(e.Button, e.Clicks, e.X, MouseEventOld.Y, e.Delta);

            Point Real = MouseToAnimationCoords(e.Location);

            #region Mouse Left

            if (e.Button == MouseButtons.Left)
            {
                if (SelectionChoice == SelectionChoices.Camera)
                {
                    Camera.X -= MouseEventOld.X - e.X;
                    Camera.Y -= MouseEventOld.Y - e.Y;
                    UpdateTransformationMatrix();
                }
                else
                {
                    double Angle = 0;
                    double PositiveAngle = 0;
                    double AngleChange = 0;
                    double ScaleChange = 0;

                    if (ActiveAnimation.MultipleSelectionRectangle.Width > 0)
                    {
                        if (BitmapAction == BitmapActions.Origin)
                        {
                            ActiveAnimation.MultipleSelectionOrigin = new Microsoft.Xna.Framework.Point(
                                ActiveAnimation.MultipleSelectionOrigin.X + e.X - MouseEventOld.X,
                                ActiveAnimation.MultipleSelectionOrigin.Y + e.Y - MouseEventOld.Y);
                        }
                        else
                        {
                            #region MultipleSelectionRectangle actions

                            if (BitmapAction == BitmapActions.Rotate)
                            {
                                int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                                int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;
                                PositiveAngle = Angle = Math.Atan2(Real.Y - OriginY, Real.X - OriginX);
                                if (Angle < 0)
                                    PositiveAngle += Math.PI * 2;
                                AngleChange = PositiveAngle - OldRotatingAngle;
                            }
                            else if (BitmapAction == BitmapActions.Move)
                            {
                                ActiveAnimation.MultipleSelectionRectangle.X += e.X - MouseEventOld.X;
                                ActiveAnimation.MultipleSelectionRectangle.Y += e.Y - MouseEventOld.Y;
                            }
                            else if (BitmapAction == BitmapActions.ScaleX || BitmapAction == BitmapActions.ScaleY || BitmapAction == BitmapActions.ScaleAll)
                            {
                                Microsoft.Xna.Framework.Vector2 MouseVector = new Microsoft.Xna.Framework.Vector2(Real.X,
                                                                                                                  Real.Y);

                                Microsoft.Xna.Framework.Vector2 OriginVector = new Microsoft.Xna.Framework.Vector2(ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X,
                                                                                                                   ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y);

                                Microsoft.Xna.Framework.Vector2 OriginalScaleResult = new Microsoft.Xna.Framework.Vector2(OriginScalePosition.X, OriginScalePosition.Y) * OriginalScaleVector;
                                Microsoft.Xna.Framework.Vector2 OriginResult = OriginVector * OriginalScaleVector;
                                Microsoft.Xna.Framework.Vector2 MouseResult = MouseVector * OriginalScaleVector;
                                Microsoft.Xna.Framework.Vector2 FinalResult = (OriginalScaleResult - OriginResult) - (MouseResult - OriginResult);
                                Microsoft.Xna.Framework.Vector2 FinalResult2 = (OriginalScaleResult - OriginResult);

                                double Distance = Math.Sqrt(Math.Pow(FinalResult.X, 2) + Math.Pow(FinalResult.Y, 2)) * Math.Sign(FinalResult.X + FinalResult.Y);

                                double DistanceScaleOrigin = FinalResult2.Length();

                                ScaleChange = Distance / DistanceScaleOrigin;
                            }

                            #endregion

                            if (BitmapAction == BitmapActions.Extra)
                            {
                                Real = MouseToAnimationCoords(MouseEventOld.Location);

                                foreach (VisibleTimeline ActiveObject in ActiveAnimation.ListSelectedObjects)
                                {
                                    VisibleTimeline ActiveTimeline = ActiveObject as VisibleTimeline;
                                    if (ActiveTimeline != null)
                                    {
                                        ActiveTimeline.MouseMoveExtra(ActiveKeyFrame, Real.X, Real.Y, e.X - MouseEventOld.X, e.Y - MouseEventOld.Y);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Timeline ActiveObject in ActiveAnimation.ListSelectedObjects)
                                {
                                    VisibleTimeline ActiveTimeline = ActiveObject as VisibleTimeline;
                                    if (ActiveTimeline != null)
                                    {
                                        MouseMoveAnimationObject(ActiveTimeline, e.X - MouseEventOriginal.X, e.Y - MouseEventOriginal.Y, Angle, AngleChange, ScaleChange);
                                        ActiveTimeline.OnUpdatePosition(new Microsoft.Xna.Framework.Vector2(e.X - MouseEventOld.X, e.Y - MouseEventOld.Y));
                                    }
                                }
                                OldRotatingAngle = PositiveAngle;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Mouse None

            else if (e.Button == MouseButtons.None && (ActiveAnimation.ListSelectedObjects.Count > 1 || (ActiveAnimation.ListSelectedObjects.Count == 1 && Control.ModifierKeys != Keys.Control)))
            {
                Cursor = Cursors.Default;

                if (ActiveAnimation.MultipleSelectionRectangle.Width > 0)
                {
                    BitmapAction = BitmapActions.None;
                    int RecMinX = ActiveAnimation.MultipleSelectionRectangle.X;
                    int RecMinY = ActiveAnimation.MultipleSelectionRectangle.Y;
                    int RecMaxX = RecMinX + ActiveAnimation.MultipleSelectionRectangle.Width;
                    int RecMaxY = RecMinY + ActiveAnimation.MultipleSelectionRectangle.Height;

                    if (Real.X >= RecMinX + ActiveAnimation.MultipleSelectionOrigin.X - 2 && Real.X <= RecMinX + ActiveAnimation.MultipleSelectionOrigin.X + 2 &&
                        Real.Y >= RecMinY + ActiveAnimation.MultipleSelectionOrigin.Y - 2 && RecMinY <= RecMinY + ActiveAnimation.MultipleSelectionOrigin.Y + 2)
                    {
                        BitmapAction = BitmapActions.Origin;
                        Cursor = Cursors.Hand;
                    }
                    else if (Real.X > RecMinX && Real.X < RecMaxX &&
                        Real.Y > RecMinY && Real.Y < RecMaxY)
                    {
                        BitmapAction = BitmapActions.Move;
                        Cursor = Cursors.SizeAll;
                    }

                    #region Check for Rotation

                    //Left
                    else if (Real.X >= RecMinX - 20 && Real.X <= RecMinX - 10 &&
                             Real.Y >= RecMinY && Real.Y <= RecMaxY)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateLeft;
                    }
                    //Right
                    else if (Real.X >= RecMaxX + 10 && Real.X <= RecMaxX + 20 &&
                             Real.Y >= RecMinY && Real.Y <= RecMaxY)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateRight;
                    }
                    //Up
                    else if (Real.X >= RecMinX && Real.X <= RecMaxX &&
                             Real.Y >= RecMinY - 20 && Real.Y <= RecMinY - 10)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateUp;
                    }
                    //Down
                    else if (Real.X >= RecMinX && Real.X <= RecMaxX &&
                             Real.Y >= RecMaxY + 10 && Real.Y <= RecMaxY + 20)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateDown;
                    }
                    //Up Left
                    else if (Real.X >= RecMinX - 20 && Real.X <= RecMinX - 10 &&
                    Real.Y >= RecMinY - 20 && Real.Y <= RecMinY - 10)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateUpLeft;
                    }
                    //Up Right
                    else if (Real.X >= RecMaxX + 10 && Real.X <= RecMaxX + 20 &&
                        Real.Y >= RecMinY - 20 && Real.Y <= RecMinY - 10)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateUpRight;
                    }
                    //Down Left
                    else if (Real.X >= RecMinX - 20 && Real.X <= RecMinX - 10 &&
                        Real.Y >= RecMaxY + 10 && Real.Y <= RecMaxY + 20)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateDownLeft;
                    }
                    //Down Right
                    else if (Real.X >= RecMaxX + 10 && Real.X <= RecMaxX + 20 &&
                        Real.Y >= RecMaxY + 10 && Real.Y <= RecMaxY + 20)
                    {
                        BitmapAction = BitmapActions.Rotate;
                        Cursor = RotateDownRight;
                    }

                    #endregion

                    #region Check for Scale

                    //Left
                    else if (Real.X >= RecMinX - 10 && Real.X <= RecMinX &&
                             Real.Y >= RecMinY && Real.Y <= RecMaxY)
                    {
                        BitmapAction = BitmapActions.ScaleX;
                        Cursor = Cursors.SizeWE;
                    }
                    //Right
                    else if (Real.X >= RecMaxX && Real.X <= RecMaxX + 10 &&
                             Real.Y >= RecMinY && Real.Y <= RecMaxY)
                    {
                        BitmapAction = BitmapActions.ScaleX;
                        Cursor = Cursors.SizeWE;
                    }
                    //Up
                    else if (Real.X >= RecMinX && Real.X <= RecMaxX &&
                             Real.Y >= RecMinY - 10 && Real.Y <= RecMinY)
                    {
                        BitmapAction = BitmapActions.ScaleY;
                        Cursor = Cursors.SizeNS;
                    }
                    //Down
                    else if (Real.X >= RecMinX && Real.X <= RecMaxX &&
                             Real.Y >= RecMaxY && Real.Y <= RecMaxY + 10)
                    {
                        BitmapAction = BitmapActions.ScaleY;
                        Cursor = Cursors.SizeNS;
                    }
                    //Up Left
                    else if (Real.X >= RecMinX - 10 && Real.X <= RecMinX &&
                             Real.Y >= RecMinY - 10 && Real.Y <= RecMinY)
                    {
                        BitmapAction = BitmapActions.ScaleAll;
                        Cursor = Cursors.SizeNWSE;
                    }
                    //Up Right
                    else if (Real.X >= RecMaxX && Real.X <= RecMaxX + 10 &&
                             Real.Y >= RecMinY - 10 && Real.Y <= RecMinY)
                    {
                        BitmapAction = BitmapActions.ScaleAll;
                        Cursor = Cursors.SizeNESW;
                    }
                    //Down Left
                    else if (Real.X >= RecMinX - 10 && Real.X <= RecMinX &&
                             Real.Y >= RecMaxY && Real.Y <= RecMaxY + 10)
                    {
                        BitmapAction = BitmapActions.ScaleAll;
                        Cursor = Cursors.SizeNESW;
                    }
                    //Down Right
                    else if (Real.X >= RecMaxX && Real.X <= RecMaxX + 10 &&
                             Real.Y >= RecMaxY && Real.Y <= RecMaxY + 10)
                    {
                        BitmapAction = BitmapActions.ScaleAll;
                        Cursor = Cursors.SizeNWSE;
                    }

                    #endregion
                }
            }

            #endregion

            MouseEventOld = e;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            SelectionChoice = SelectionChoices.None;

            if (ActiveLayer.IsLocked)
                return;

            if (BitmapAction != BitmapActions.None)
            {
                foreach (VisibleTimeline ActiveObject in ActiveAnimation.ListSelectedObjects)
                {
                    if (!ActiveObject.ContainsKey(ActiveKeyFrame))
                        continue;

                    VisibleAnimationObjectKeyFrame KeyFrame = (VisibleAnimationObjectKeyFrame)ActiveObject.Get(ActiveKeyFrame);

                    KeyFrame.Position = ActiveObject.Position;

                    if (BitmapAction == BitmapActions.ScaleX || BitmapAction == BitmapActions.ScaleY || BitmapAction == BitmapActions.ScaleAll)
                    {
                        KeyFrame.ScaleFactor = ActiveObject.ScaleFactor;
                    }

                    ActiveObject.MouseUpExtra();
                }

                SaveCurrentAnimation();

                BitmapAction = BitmapActions.None;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (Control.ModifierKeys == Keys.Control)
            {
                UpdateZoom(e.Delta / 1200f);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.X:
                    AxisLock = AxisLocks.X;
                    break;

                case Keys.Z:
                    AxisLock = AxisLocks.Y;
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            AxisLock = AxisLocks.None;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Point Movement = Point.Empty;

            switch (keyData)
            {
                case Keys.Up:
                    Movement.Y = -1;
                    break;

                case Keys.Down:
                    Movement.Y = 1;
                    break;

                case Keys.Left:
                    Movement.X = -1;
                    break;

                case Keys.Right:
                    Movement.X = 1;
                    break;
            }

            if (Movement.X != 0 || Movement.Y != 0)
            {
                BitmapAction = BitmapActions.Move;
                if (ActiveAnimation.MultipleSelectionRectangle.Width > 0)
                {
                    ActiveAnimation.MultipleSelectionRectangle.X += Movement.X;
                    ActiveAnimation.MultipleSelectionRectangle.Y += Movement.Y;
                }
                foreach (VisibleTimeline ActiveObject in ActiveAnimation.ListSelectedObjects)
                {
                    MouseMoveAnimationObject(ActiveObject, Movement.X, Movement.Y, 0, 0, 0);
                    ActiveObject.OnUpdatePosition(new Microsoft.Xna.Framework.Vector2(Movement.X, Movement.Y));
                    VisibleAnimationObjectKeyFrame KeyFrame = (VisibleAnimationObjectKeyFrame)ActiveObject.Get(ActiveKeyFrame);

                    KeyFrame.Position = ActiveObject.Position;
                }

                SaveCurrentAnimation();

                BitmapAction = BitmapActions.None;
                return true;
            }
            return false;
        }

        #endregion

        public void SetActiveLayer(AnimationClass.AnimationLayer ActiveLayer)
        {
            this.ActiveLayer = ActiveLayer;
        }

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
        }

        public void SelectItems(int PosX, int PosY)
        {
            bool CanSelect = true;
            int AnimationOriginX, AnimationOriginY, AnimationOriginMaxX, AnimationOriginMaxY;
            ActiveAnimation.AnimationOrigin.GetMinMax(out AnimationOriginX, out AnimationOriginY, out AnimationOriginMaxX, out AnimationOriginMaxY);

            int ActiveBitmapSelected = -2;
            int ActiveMarkerSelected = -2;
            int ActivePolygonCutterSelected = -2;

            #region Select a Visible Object

            for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
            {
                //Select Item.
                if (CanSelect && ActiveLayer.ListVisibleObject[A].CanSelect(PosX, PosY))
                {
                    //Single item selection.
                    if (Control.ModifierKeys != Keys.Shift)
                    {
                        //Force the loop to exit
                        if (ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListVisibleObject[A]))
                        {
                            CanSelect = false;
                            ActiveBitmapSelected = -1;
                        }
                        else
                            ActiveBitmapSelected = A;
                    }
                    //Multiple item selection.
                    else
                    {
                        if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListVisibleObject[A]))
                        {
                            CanSelect = false;
                            ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListVisibleObject[A]);
                            OnTimelineSelected(ActiveLayer.ListVisibleObject[A]);
                        }
                    }
                }
            }

            if (ActiveBitmapSelected >= 0)
            {
                ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListVisibleObject[ActiveBitmapSelected]);
                OnTimelineSelected(ActiveLayer.ListVisibleObject[ActiveBitmapSelected]);
                CanSelect = false;
            }

            #endregion

            #region Select a Marker

            for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
            {
                //Select Item.
                if (CanSelect && ActiveLayer.ListActiveMarker[M].CanSelect(PosX, PosY))
                {
                    //Single item selection.
                    if (Control.ModifierKeys != Keys.Shift)
                    {
                        //Force the loop to exit while allowing ActiveMarkerSelected to take its last value.
                        if (ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListActiveMarker[M]))
                        {
                            CanSelect = false;
                            ActiveMarkerSelected = -1;
                        }
                        else
                            ActiveMarkerSelected = M;
                    }
                    //Multiple item selection.
                    else
                    {
                        if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListActiveMarker[M]))
                        {
                            CanSelect = false;
                            ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListActiveMarker[M]);
                            OnTimelineSelected(ActiveLayer.ListActiveMarker[M]);
                        }
                    }
                }
            }

            if (ActiveMarkerSelected >= 0)
            {
                ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListActiveMarker[ActiveMarkerSelected]);
                OnTimelineSelected(ActiveLayer.ListActiveMarker[ActiveMarkerSelected]);
                CanSelect = false;
            }

            #endregion

            #region Select a Polygon Cutter

            for (int P = 0; P < ActiveLayer.ListPolygonCutter.Count; P++)
            {
                foreach (Polygon ActivePolygon in ActiveLayer.ListPolygonCutter[P].ListPolygon)
                {
                    //Select Item.
                    if (ActivePolygon.PolygonCollisionPerTriangle(PosX, PosY) &&
                        CanSelect)
                    {
                        //Single item selection.
                        if (Control.ModifierKeys != Keys.Shift)
                        {
                            //Force the loop to exit while allowing ActiveMarkerSelected to take its last value.
                            if (ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListPolygonCutter[P]))
                            {
                                CanSelect = false;
                                ActivePolygonCutterSelected = -1;
                            }
                            else
                                ActivePolygonCutterSelected = P;
                        }
                        //Multiple item selection.
                        else
                        {
                            if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveLayer.ListPolygonCutter[P]))
                            {
                                CanSelect = false;
                                ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListPolygonCutter[P]);
                                OnTimelineSelected(ActiveLayer.ListPolygonCutter[P]);
                            }
                        }
                    }
                }
            }

            if (ActivePolygonCutterSelected >= 0)
            {
                ActiveAnimation.ListSelectedObjects.Add(ActiveLayer.ListPolygonCutter[ActivePolygonCutterSelected]);
                OnTimelineSelected(ActiveLayer.ListPolygonCutter[ActivePolygonCutterSelected]);
                CanSelect = false;
            }

            #endregion

            #region Select AnimationOrigin

            if (PosX >= AnimationOriginX && PosX < AnimationOriginX + ActiveAnimation.AnimationOrigin.Width &&
                PosY >= AnimationOriginY && PosY < AnimationOriginY + ActiveAnimation.AnimationOrigin.Height &&
                    CanSelect)
            {
                //Single item selection.
                if (Control.ModifierKeys != Keys.Shift)
                {
                    //Force the loop to exit.
                    if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveAnimation.AnimationOrigin))
                    {
                        ActiveAnimation.ListSelectedObjects.Add(ActiveAnimation.AnimationOrigin);
                        OnTimelineSelected(ActiveAnimation.AnimationOrigin);
                    }
                    CanSelect = false;
                }
                //Multiple item selection.
                else
                {
                    if (!ActiveAnimation.ListSelectedObjects.Contains(ActiveAnimation.AnimationOrigin))
                    {
                        CanSelect = false;
                        ActiveAnimation.ListSelectedObjects.Add(ActiveAnimation.AnimationOrigin);
                        OnTimelineSelected(ActiveAnimation.AnimationOrigin);
                    }
                }
            }
            else
            {
                //Single item selection.
                if (Control.ModifierKeys != Keys.Shift)
                    ActiveAnimation.ListSelectedObjects.Remove(ActiveAnimation.AnimationOrigin);
            }

            #endregion

            #region Finish Visible Object selection

            for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
            {
                //Mouse isn't selecting the item but the item was selected.
                if (Control.ModifierKeys != Keys.Shift && (ActiveBitmapSelected >= 0 || ActiveBitmapSelected == -2))
                {
                    //Only one item was selected and this is not that one.
                    if (ActiveBitmapSelected != A)
                    {
                        ActiveAnimation.ListSelectedObjects.Remove(ActiveLayer.ListVisibleObject[A]);
                    }
                }
            }

            #endregion

            #region Finish Marker selection

            for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
            {
                //Mouse isn't selecting the item but the item was selected.
                if (Control.ModifierKeys != Keys.Shift)
                {
                    //Only one item was selected and this is not that one.
                    if (ActiveMarkerSelected != M)
                    {
                        ActiveAnimation.ListSelectedObjects.Remove(ActiveLayer.ListActiveMarker[M]);
                    }
                }
            }

            #endregion

            #region Finish Polygon Cutter selection

            for (int P = 0; P < ActiveLayer.ListPolygonCutter.Count; P++)
            {
                //Mouse isn't selecting the item but the item was selected.
                if (Control.ModifierKeys != Keys.Shift)
                {
                    //Only one item was selected and this is not that one.
                    if (ActivePolygonCutterSelected != P)
                    {
                        ActiveAnimation.ListSelectedObjects.Remove(ActiveLayer.ListPolygonCutter[P]);
                    }
                }
            }

            #endregion

            for (int A = ActiveAnimation.ListSelectedObjects.Count - 1; A >= 0; --A)
            {
                if (ActiveAnimation.ActiveKeyFrame >= ActiveAnimation.ListSelectedObjects[A].DeathFrame
                    || ActiveAnimation.ActiveKeyFrame < ActiveAnimation.ListSelectedObjects[A].SpawnFrame)
                {
                    ActiveAnimation.ListSelectedObjects.RemoveAt(A);
                }
            }
        }

        public void MouseMoveAnimationObject(VisibleTimeline ActiveTimeline, int TotalTranslationX, int TotalTranslationY, double AngleWithMouse, double AngleChange, double ScaleChange)
        {
            AnimationObjectKeyFrame Temp;

            if (!ActiveTimeline.TryGetValue(ActiveKeyFrame, out Temp))
            {
                OnTimelineChanged();
            }

            VisibleAnimationObjectKeyFrame ActiveObjectKeyFrame = (VisibleAnimationObjectKeyFrame)ActiveTimeline.CreateOrRetriveKeyFrame(ActiveLayer, ActiveKeyFrame);

            if (BitmapAction == BitmapActions.Rotate)
            {
                int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;

                //Rotate multiple selection rectangle.
                if (ActiveAnimation.MultipleSelectionRectangle.Width > 0)
                {
                    double BitmapAngleFromCenter = Math.Atan2(ActiveObjectKeyFrame.Position.Y - OriginY,
                                                       ActiveObjectKeyFrame.Position.X - OriginX);

                    double NextAngle = BitmapAngleFromCenter - OriginalRotatingAngle + AngleWithMouse;
                    double DistanceFromCenter = Math.Sqrt(Math.Pow(ActiveObjectKeyFrame.Position.Y - OriginY, 2) +
                                                           Math.Pow(ActiveObjectKeyFrame.Position.X - OriginX, 2));

                    ActiveTimeline.Position = new Microsoft.Xna.Framework.Vector2(
                                    OriginX + (int)(Math.Cos(NextAngle) * DistanceFromCenter),
                                    OriginY + (int)(Math.Sin(NextAngle) * DistanceFromCenter));

                    ActiveTimeline.Angle += (float)AngleChange;
                }
            }
            else if (BitmapAction == BitmapActions.Move)
            {
                //Set the Key Frame position to the mouse position.
                ActiveTimeline.Position = new Microsoft.Xna.Framework.Vector2(
                            ActiveObjectKeyFrame.Position.X + TotalTranslationX,
                            ActiveObjectKeyFrame.Position.Y + TotalTranslationY);
            }
            else if (BitmapAction == BitmapActions.ScaleAll)
            {
                double CurrentScaleValueX = ActiveObjectKeyFrame.ScaleFactor.X - ScaleChange;
                double CurrentScaleValueY = ActiveObjectKeyFrame.ScaleFactor.Y - ScaleChange;

                int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;

                double DistanceFromCenterX = OriginX - ActiveObjectKeyFrame.Position.X;
                double DistanceFromCenterY = OriginY - ActiveObjectKeyFrame.Position.Y;

                //Set the Key Frame position to the mouse position.
                ActiveTimeline.Position = new Microsoft.Xna.Framework.Vector2(
                            OriginX - (int)(CurrentScaleValueX * DistanceFromCenterX),
                            OriginY - (int)(CurrentScaleValueY * DistanceFromCenterY));

                ActiveTimeline.ScaleFactor = new Microsoft.Xna.Framework.Vector2((float)CurrentScaleValueX, (float)CurrentScaleValueY);
            }
            else if (BitmapAction == BitmapActions.ScaleX)
            {
                double CurrentScaleValueX = ActiveObjectKeyFrame.ScaleFactor.X - ScaleChange;
                double CurrentScaleValueY = ActiveObjectKeyFrame.ScaleFactor.Y;

                int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;

                double DistanceFromCenterX = OriginX - ActiveObjectKeyFrame.Position.X;
                double DistanceFromCenterY = OriginY - ActiveObjectKeyFrame.Position.Y;

                //Set the Key Frame position to the mouse position.
                ActiveTimeline.Position = new Microsoft.Xna.Framework.Vector2(
                            OriginX - (int)(CurrentScaleValueX * DistanceFromCenterX),
                            OriginY - (int)(CurrentScaleValueY * DistanceFromCenterY));
                ActiveTimeline.ScaleFactor = new Microsoft.Xna.Framework.Vector2((float)CurrentScaleValueX, (float)CurrentScaleValueY);
            }
            else if (BitmapAction == BitmapActions.ScaleY)
            {
                double CurrentScaleValueX = ActiveObjectKeyFrame.ScaleFactor.X;
                double CurrentScaleValueY = ActiveObjectKeyFrame.ScaleFactor.Y - ScaleChange;

                int OriginX = ActiveAnimation.MultipleSelectionRectangle.X + ActiveAnimation.MultipleSelectionOrigin.X;
                int OriginY = ActiveAnimation.MultipleSelectionRectangle.Y + ActiveAnimation.MultipleSelectionOrigin.Y;

                double DistanceFromCenterX = OriginX - ActiveObjectKeyFrame.Position.X;
                double DistanceFromCenterY = OriginY - ActiveObjectKeyFrame.Position.Y;

                //Set the Key Frame position to the mouse position.
                ActiveTimeline.Position = new Microsoft.Xna.Framework.Vector2(
                            OriginX - (int)(CurrentScaleValueX * DistanceFromCenterX),
                            OriginY - (int)(CurrentScaleValueY * DistanceFromCenterY));
                ActiveTimeline.ScaleFactor = new Microsoft.Xna.Framework.Vector2((float)CurrentScaleValueX, (float)CurrentScaleValueY);
            }

            ActiveTimeline.PositionOld = ActiveTimeline.Position;
            ActiveObjectKeyFrame.AngleInRad = ActiveTimeline.Angle;
        }

        public void SaveCurrentAnimation()
        {
            ListOldAnimation.Add((AnimationClassEditor)ActiveAnimation.Copy());
            if (ListOldAnimation.Count > 10)
            {
                ListOldAnimation.RemoveAt(0);
            }
        }

        public void Undo()
        {
            //Last animation in the list is the current one.
            ActiveAnimation = ListOldAnimation[ListOldAnimation.Count - 2];
            ListOldAnimation.Remove(ActiveAnimation);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Microsoft.Xna.Framework.Color backColor = new Microsoft.Xna.Framework.Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            bool IsInEditMode = !IsPlaying;


            Microsoft.Xna.Framework.Matrix t = g.Scale;
            g.Scale = Microsoft.Xna.Framework.Matrix.Identity;

            BeginDraw(g, IsInEditMode, ShowBorderBoxes, ShowNextPositions);

            g.Scale = t;

            ActiveAnimation.DrawEditor(g, Width, Height, IsInEditMode, ShowBorderBoxes, ShowNextPositions);

            Thread.Sleep(1);
        }

        public void LoadBackgroundPreview(string BackgroundName)
        {
            ActiveAnimation.ActiveAnimationBackground = AnimationBackground.LoadAnimationBackground(BackgroundName, content, GraphicsDevice);
        }

        public Point MouseToAnimationCoords(Point MousePos)
        {
            Microsoft.Xna.Framework.Vector2 TransformedVector = Microsoft.Xna.Framework.Vector2.Transform(new Microsoft.Xna.Framework.Vector2(MousePos.X, MousePos.Y), MouseTransformationMatrix);
            return new Point((int)TransformedVector.X, (int)TransformedVector.Y);
        }

        /// <summary>
        /// Update thew ViewportOffset in case of screen resizing.
        /// </summary>
        /// <param name="NewWidth">Width of the new screen.</param>
        /// <param name="NewHeight">Height of the new screen.</param>
        public void UpdateOffset(int NewWidth, int NewHeight)
        {
            ViewportOffset.X = (NewWidth - AnimationClassEditor.Width) / 2;
            ViewportOffset.Y = (NewHeight - AnimationClassEditor.Height) / 2;

            Microsoft.Xna.Framework.Matrix Projection = Microsoft.Xna.Framework.Matrix.CreateOrthographicOffCenter(0, NewWidth, NewHeight, 0, 0, 1);
            Microsoft.Xna.Framework.Matrix HalfPixelOffset = Microsoft.Xna.Framework.Matrix.CreateTranslation(-0.5f + ViewportOffset.X, -0.5f + ViewportOffset.Y, 0);

            ActiveAnimation.UpdateProjection(HalfPixelOffset * Projection);

            ZoomCenter = new Microsoft.Xna.Framework.Point(ViewportOffset.X + AnimationClassEditor.Width / 2, ViewportOffset.Y + AnimationClassEditor.Height / 2);
            UpdateTransformationMatrix();
        }

        public void UpdateZoom(float ZoomValueToAdd)
        {
            Zoom += ZoomValueToAdd;
            if (Zoom <= 0)
                Zoom = ZoomValueToAdd;

            UpdateTransformationMatrix();
        }

        public void UpdateTransformationMatrix()
        {
            if (Zoom == 1)
            {
                Camera = Microsoft.Xna.Framework.Point.Zero;
            }

            MouseTransformationMatrix = Microsoft.Xna.Framework.Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(-ZoomCenter.X - Camera.X, -ZoomCenter.Y - Camera.Y, 0)) *
                                          Microsoft.Xna.Framework.Matrix.CreateScale(new Microsoft.Xna.Framework.Vector3(1f / Zoom, 1f / Zoom, 1f)) *
                                          Microsoft.Xna.Framework.Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(AnimationClassEditor.Width / 2, AnimationClassEditor.Height / 2, 0));

            g.Scale = Microsoft.Xna.Framework.Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(-AnimationClassEditor.Width / 2, -AnimationClassEditor.Height / 2, 0)) *
                                          Microsoft.Xna.Framework.Matrix.CreateScale(new Microsoft.Xna.Framework.Vector3(Zoom, Zoom, 1f)) *
                                          Microsoft.Xna.Framework.Matrix.CreateTranslation(new Microsoft.Xna.Framework.Vector3(ZoomCenter.X + Camera.X, ZoomCenter.Y + Camera.Y, 0));
        }

        public void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                Name = Items[I].Substring(0, Items[0].Length - 4).Substring(19);
                if (Name != null)
                {
                    AnimationClassEditor ImportAnimation = new AnimationClassEditor(Name);
                    ImportAnimation.Content = ActiveAnimation.Content;

                    ImportAnimation.Load();

                    for (int L = 0; L < ImportAnimation.ListAnimationLayer.Count; L++)
                    {
                        ActiveAnimation.ListAnimationLayer.Add(ImportAnimation.ListAnimationLayer[L]);
                    }
                    //Force reloading of the scene.
                    int OriginalKeyFrame = ActiveKeyFrame;
                    ActiveKeyFrame++;
                    OnKeyFrameChange(OriginalKeyFrame);
                    ActiveKeyFrame = OriginalKeyFrame;

                    //Update Timeline to show the new items.
                    OnTimelineSelectionChanged();
                    OnTimelineChanged();
                    OnLayersChanged();
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g, bool IsInEditMode, bool ShowBorderBoxes, bool ShowNextPositions)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            ActiveAnimation.BeginDraw(g);

            g.End();
            g.GraphicsDevice.SetRenderTarget(null);

            for (int L = 0; L < ActiveAnimation.ListAnimationLayer.Count; L++)
            {
                if (ActiveAnimation.ListAnimationLayer[L].renderTarget == null
                    || ActiveAnimation.ListAnimationLayer[L].renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth
                    ||  ActiveAnimation.ListAnimationLayer[L].renderTarget.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    ActiveAnimation.ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                        g.GraphicsDevice,
                        g.GraphicsDevice.PresentationParameters.BackBufferWidth,
                        g.GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
                }

                GraphicsDevice.SetRenderTarget(ActiveAnimation.ListAnimationLayer[L].renderTarget);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil,  Microsoft.Xna.Framework.Color.Transparent, 0, 0);

                ActiveAnimation.DrawLayer(g, ActiveAnimation.ListAnimationLayer[L], IsInEditMode, ShowBorderBoxes, ShowNextPositions, null);
            }
        }
    }
}
