using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Editor
{
    public class Scene3DViewerControl : GraphicsDeviceControl
    {
        public enum SelectionTypes { None, Selected, MoveX, MoveY, MoveZ, RotateX, RotateY, RotateZ, ScaleX, ScaleY, ScaleZ };
        public enum SelectionModes { Move, Rotate, Scale };

        public delegate Object3D GetObjectUnderMouseDelegate(MouseEventArgs e);
        public delegate void OnObjectSelectedDelegate(Object3D SelectedObject);
        protected delegate void UpdateDelegate(GameTime gameTime);
        protected delegate void DrawDelegate(CustomSpriteBatch g);

        public GetObjectUnderMouseDelegate GetObjectUnderMouse;
        public OnObjectSelectedDelegate OnObjectSelected;
        protected UpdateDelegate DoUpdate;
        protected DrawDelegate DoDraw;

        public Camera3D ActiveCamera;
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;

        private Object3D SelectedObject;
        private SelectionTypes SelectionType;
        public Lines3D BackgroundGrid;
        public CrossArrow3D MoveHelper;
        public CrossRing3D RotationHelper;
        public SelectionModes SelectionMode;
        private System.Diagnostics.Stopwatch Timer;

        private SpriteFont fntScriptName;
        private System.Drawing.Point MousePosOld;
        private System.Drawing.Point MousePosOriginal;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;
            Timer = new System.Diagnostics.Stopwatch();
            content = new ContentManager(Services, "Content");
            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            sprPixel = content.Load<Texture2D>("Pixel");
            fntScriptName = content.Load<SpriteFont>("Fonts/Calibri8");
        }

        public void Preload()
        {
            OnCreateControl();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            switch (SelectionType)
            {
                case SelectionTypes.MoveX:
                    SelectedObject.MoveWithMouseX(e.X, e.Y, MousePosOld.X, MousePosOld.Y, GraphicsDevice.Viewport,
                                                                                                ActiveCamera.View,
                                                                                                ActiveCamera.Projection,
                                                                                                Matrix.Identity);
                    MoveHelper.Position = SelectedObject.Position;
                    break;

                case SelectionTypes.MoveY:
                    SelectedObject.MoveWithMouseY(e.X, e.Y, MousePosOld.X, MousePosOld.Y, GraphicsDevice.Viewport,
                                                                                                ActiveCamera.View,
                                                                                                ActiveCamera.Projection,
                                                                                                Matrix.Identity);
                    MoveHelper.Position = SelectedObject.Position;
                    break;

                case SelectionTypes.MoveZ:
                    SelectedObject.MoveWithMouseZ(e.X, e.Y, MousePosOld.X, MousePosOld.Y, GraphicsDevice.Viewport,
                                                                                                ActiveCamera.View,
                                                                                                ActiveCamera.Projection,
                                                                                                Matrix.Identity);
                    MoveHelper.Position = SelectedObject.Position;
                    break;

                case SelectionTypes.RotateX:
                    SelectedObject.RotateWithMouseX(e.X, e.Y, GraphicsDevice.Viewport,
                                                                                                ActiveCamera.View,
                                                                                                ActiveCamera.Projection,
                                                                                                Matrix.Identity);
                    RotationHelper.Position = SelectedObject.Position;
                    break;

                case SelectionTypes.RotateY:
                    SelectedObject.RotateWithMouseY(e.X, e.Y, GraphicsDevice.Viewport,
                                                                                                ActiveCamera.View,
                                                                                                ActiveCamera.Projection,
                                                                                                Matrix.Identity);
                    RotationHelper.Position = SelectedObject.Position;
                    break;
            }

            MousePosOld = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (SelectionType != SelectionTypes.None)
            {
                SelectionType = SelectionTypes.Selected;
                MoveHelper.UnSelectArrow();
                RotationHelper.UnSelectRing();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MousePosOriginal = e.Location;

            base.OnMouseDown(e);

            Focus();

            if (e.Button == MouseButtons.Left)
            {
                //Select AI Tunnel.
                if (SelectionType == SelectionTypes.None)
                {
                    Object3D ActiveObject3D = GetObjectUnderMouse(e);
                    OnObjectSelected(ActiveObject3D);
                    SelectObject(ActiveObject3D);
                }
                else if (SelectionType == SelectionTypes.Selected)
                {
                    if (SelectionMode == SelectionModes.Move)
                    {
                        HandleMouseMove(e);
                    }
                    else if (SelectionMode == SelectionModes.Rotate)
                    {
                        HandleMouseRotation(e);
                    }

                    //User didn't click on a helper.
                    if (SelectionType == SelectionTypes.Selected)
                    {
                        Object3D ActiveObject3D = GetObjectUnderMouse(e);
                        OnObjectSelected(ActiveObject3D);
                        SelectObject(ActiveObject3D);
                    }
                }
            }
        }

        public void SelectObject(Object3D ActiveObject3D)
        {
            if (ActiveObject3D != null)
            {
                SelectedObject = ActiveObject3D;
                SelectionType = SelectionTypes.Selected;
                MoveHelper.Position = ActiveObject3D.Position;
                RotationHelper.Position = ActiveObject3D.Position;
            }
            else
            {
                SelectedObject = null;
                SelectionType = SelectionTypes.None;
                MoveHelper.UnSelectArrow();
                RotationHelper.UnSelectRing();
            }
        }

        private void HandleMouseMove(MouseEventArgs e)
        {
            if (MoveHelper.ArrowXCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.MoveX;
                MoveHelper.SelectArrowX();
            }
            else if (MoveHelper.ArrowYCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.MoveY;
                MoveHelper.SelectArrowY();
            }
            else if (MoveHelper.ArrowZCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.MoveZ;
                MoveHelper.SelectArrowZ();
            }
        }

        private void HandleMouseRotation(MouseEventArgs e)
        {
            if (RotationHelper.RingXCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.RotateX;
                RotationHelper.SelectRingX();
            }
            else if (RotationHelper.RingYCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.RotateY;
                RotationHelper.SelectRingY();
            }
            else if (RotationHelper.RingZCollideWithMouse(e.X, e.Y, GraphicsDevice.Viewport,
                ActiveCamera.View,
                ActiveCamera.Projection))
            {
                SelectionType = SelectionTypes.RotateZ;
                RotationHelper.SelectRingZ();
            }
        }

        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);
            
            double elapsed = Timer.Elapsed.TotalSeconds;
            Timer.Restart();
            DoUpdate(new GameTime(TimeSpan.FromSeconds(elapsed), TimeSpan.FromSeconds(elapsed)));
            DoDraw(g);

            BackgroundGrid.Draw(g, ActiveCamera.View);

            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);
            g.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (SelectedObject != null)
            {
                if (SelectionType != SelectionTypes.None)
                {
                    if (SelectionMode == SelectionModes.Move)
                    {
                        MoveHelper.Draw(g, ActiveCamera.View);
                    }
                    else if (SelectionMode == SelectionModes.Rotate)
                    {
                        RotationHelper.Draw(g, ActiveCamera.View);
                    }
                }
            }

            Thread.Sleep(1);
        }
    }
}
