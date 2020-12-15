using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scene;

namespace ProjectEternity.Editors.SceneEditor
{
    internal class SceneTimelineViewerControl : GraphicsDeviceControl
    {
        private const int HeaderHeight = 20;
        private const int BoxWidth = 100;
        private const int BoxHeight = 30;

        private SceneScreen Scene;
        private SceneEvent DragDropScene;
        private int DragDropSceneColumnOriginal;

        private int DragDropSceneColumnCurrent;
        private int DragDropSceneRowCurrent;

        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public SceneEditor Owner;
        public SpriteFont fntCalibri8;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");
            sprPixel = content.Load<Texture2D>("Pixel");
            fntCalibri8 = content.Load<SpriteFont>("Fonts/Calibri8");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
        }

        public void Preload(SceneScreen Scene)
        {
            this.Scene = Scene;
            OnCreateControl();
        }

        #region Mouse Events

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (DragDropScene == null)
                {
                    int Row = (e.Y - HeaderHeight) / BoxHeight;
                    int Column = e.X / BoxWidth;
                    List<SceneEvent> ListEvent;

                    if (Scene.DicSceneEventByFrame.TryGetValue(Column, out ListEvent) && Row < ListEvent.Count)
                    {
                        DragDropScene = ListEvent[Row];
                        DragDropSceneColumnOriginal = Column;
                        DragDropSceneColumnCurrent = Column;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Owner.cmsSceneEvents.Show(Owner, Owner.PointToClient(Cursor.Position));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                if (DragDropScene != null)
                {
                    DragDropSceneRowCurrent = (e.Y - HeaderHeight - BoxHeight / 2) / BoxHeight;
                    DragDropSceneColumnCurrent = e.X / BoxWidth;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                if (DragDropScene != null && DragDropSceneColumnOriginal != DragDropSceneColumnCurrent)
                {
                    int Row = (e.Y - HeaderHeight) / BoxHeight;
                    int Column = e.X / BoxWidth;

                    if (DragDropSceneRowCurrent > Scene.DicSceneEventByFrame[DragDropSceneColumnCurrent].Count)
                    {
                        DragDropSceneRowCurrent = Scene.DicSceneEventByFrame[DragDropSceneColumnCurrent].Count;
                    }

                    Scene.DicSceneEventByFrame[DragDropSceneColumnOriginal].Remove(DragDropScene);
                    Scene.DicSceneEventByFrame[DragDropSceneColumnCurrent].Insert(DragDropSceneRowCurrent, DragDropScene);

                    DragDropScene = null;
                    DragDropSceneColumnOriginal = -1;
                    DragDropSceneColumnCurrent = -1;
                    DragDropSceneRowCurrent = -1;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            int Row = (e.Y - HeaderHeight) / BoxHeight;
            int Column = e.X / BoxWidth;
            List<SceneEvent> ListEvent;

            if (Scene.DicSceneEventByFrame.TryGetValue(Column, out ListEvent) && Row < ListEvent.Count)
            {
                EditSceneEvent(ListEvent[Row]);
            }
        }

        #endregion

        private void EditSceneEvent(SceneEvent EventToEdit)
        {
            TabPage NewTab = new TabPage();
            NewTab.Tag = EventToEdit;
            NewTab.Padding = new Padding(3);
            NewTab.Text = EventToEdit.SceneEventType + "      ";
            Control NewTimelineEditor = EventToEdit.GetTimelineEditor();
            NewTimelineEditor.Dock = DockStyle.Fill;
            NewTab.Controls.Add(NewTimelineEditor);
            Owner.tcSceneEvents.TabPages.Add(NewTab);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            g.Begin();

            List<SceneEvent> ListSceneEvent;

            g.Draw(sprPixel, new Rectangle(0, 0, Scene.MaxSceneEvent * BoxWidth, HeaderHeight), Color.Black);
            for (int i = 0; i < Scene.MaxSceneEvent; ++i)
            {
                int X = i * BoxWidth;
                g.Draw(sprPixel, new Rectangle(X, 1, BoxWidth, HeaderHeight - 2), Color.Gray);
                g.DrawStringMiddleAligned(fntCalibri8, (i + 1).ToString(), new Vector2(X + BoxWidth / 2, 5), Color.Black);

                if (Scene.DicSceneEventByFrame.TryGetValue(i, out ListSceneEvent))
                {
                    for (int E = 0; E < ListSceneEvent.Count; ++E)
                    {
                        int Y = E * BoxHeight + HeaderHeight;
                        g.Draw(sprPixel, new Rectangle(X, Y, BoxWidth, BoxHeight), Color.Black);
                        g.Draw(sprPixel, new Rectangle(X, Y + 1, BoxWidth, BoxHeight - 2), Color.Gray);
                        g.DrawStringMiddleAligned(fntCalibri8, ListSceneEvent[E].SceneEventType, new Vector2(X, Y), Color.Black);
                    }
                }

                g.Draw(sprPixel, new Rectangle(X, 0, 1, Height), Color.Black);
            }

            g.Draw(sprPixel, new Rectangle(DragDropSceneColumnCurrent * BoxWidth, DragDropSceneRowCurrent * BoxHeight, BoxWidth, 1), Color.Black);

            g.End();

            Thread.Sleep(1);
        }
    }
}
