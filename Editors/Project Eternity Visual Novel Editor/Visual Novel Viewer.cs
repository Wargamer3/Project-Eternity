using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public enum ScriptLinkType { None = 0, FromDialog, ToDialog };

    internal unsafe class VisualNovelViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        private Texture2D imgScriptTopLeft;
        private Texture2D imgScriptTopMiddle;
        private Texture2D imgScriptTopRight;
        private Texture2D imgScriptMiddleLeft;
        private Texture2D imgScriptMiddleMiddle;
        private Texture2D imgScriptMiddleRight;
        private Texture2D imgScriptBottomLeft;
        private Texture2D imgScriptBottomMiddle;
        private Texture2D imgScriptBottomRight;

        public VisualNovel ActiveVisualNovel;
        public SpriteFont fntFrameCount;
        public List<Point> MovingScripts;//Used to detect if you are holding the left click after selecting an object and from where.

        public Point ScriptEditorOrigin;//Point from where to start drawing.
        public List<Dialog> ListDialogSelected;
        public int ScriptLink;//Script used as starting point.
        public Point ScriptLinkStartingPoint;//Point from which to start drawing a line to the mouse to preview the script linking.
        public ScriptLinkType ScriptLinkTypeChoice;//Type of the ScriptLink, is it an object linking an other or and object being linked to an other.

        public bool DrawScripts;
        public int MouseX;
        public int MouseY;
        public int BoxWidth = 100;
        public int BoxHeight = 50;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;
            ScriptEditorOrigin = new Point();
            ListDialogSelected = new List<Dialog>();
            MovingScripts = new List<Point>();

            content = new ContentManager(Services, "Content");

            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            sprPixel = content.Load<Texture2D>("Pixel");

            imgScriptTopLeft = content.Load<Texture2D>("Menus/Scripts/ScriptTopLeft2");
            imgScriptTopMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptTopMiddle2");
            imgScriptTopRight = content.Load<Texture2D>("Menus/Scripts/ScriptTopRight2");
            imgScriptMiddleLeft = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleLeft2");
            imgScriptMiddleMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleMiddle2");
            imgScriptMiddleRight = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleRight2");
            imgScriptBottomLeft = content.Load<Texture2D>("Menus/Scripts/ScriptBottomLeft2");
            imgScriptBottomMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptBottomMiddle2");
            imgScriptBottomRight = content.Load<Texture2D>("Menus/Scripts/ScriptBottomRight2");

            fntFrameCount = content.Load<SpriteFont>("Fonts/Arial8");
        }

        public void Preload()
        {
            OnCreateControl();
        }

        private void DrawScriptBox(CustomSpriteBatch g, int X, int Y, int Width, int Height)
        {
            g.Draw(imgScriptTopLeft, new Vector2(X, Y), Color.White);
            g.Draw(imgScriptMiddleLeft, new Rectangle(X,
                                                Y + imgScriptTopLeft.Height,
                                                imgScriptMiddleLeft.Width,
                                                Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height), Color.White);
            g.Draw(imgScriptBottomLeft, new Vector2(X, Y + Height - imgScriptBottomLeft.Height), Color.White);

            g.Draw(imgScriptTopMiddle, new Rectangle(X + imgScriptTopLeft.Width,
                                                Y,
                                                Width - imgScriptTopRight.Width,
                                                imgScriptTopMiddle.Height), Color.White);

            g.Draw(imgScriptMiddleMiddle, new Rectangle(X + imgScriptMiddleLeft.Width,
                                              Y + imgScriptTopLeft.Height,
                                              Width - imgScriptMiddleLeft.Width - imgScriptMiddleRight.Width,
                                              Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height), Color.White);
            
            g.Draw(imgScriptBottomMiddle, new Rectangle(X + imgScriptBottomLeft.Width,
                                              Y + Height - imgScriptBottomMiddle.Height,
                                              Width - imgScriptBottomLeft.Width - imgScriptBottomRight.Width,
                                              imgScriptBottomMiddle.Height), Color.White);

            g.Draw(imgScriptTopRight, new Vector2(X + Width - imgScriptTopRight.Width, Y), Color.White);
            g.Draw(imgScriptMiddleRight, new Rectangle(X + Width - imgScriptMiddleRight.Width,
                                            Y + imgScriptTopRight.Height,
                                            imgScriptMiddleRight.Width,
                                            Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height), Color.White);
            g.Draw(imgScriptBottomRight, new Vector2(X + Width - imgScriptBottomRight.Width, Y + Height - imgScriptBottomRight.Height), Color.White);
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            if (DrawScripts)
            {
                g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                // Clear to the default control background color.
                Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);
                GraphicsDevice.Clear(backColor);
                DrawScriptEditor();
            }
            else
            {
                ActiveVisualNovel.Update(new GameTime());
                ActiveVisualNovel.BeginDraw(g);

                GraphicsDevice.SetRenderTarget(null);
                g.Begin(SpriteSortMode.Deferred, null);

                // Clear to the default control background color.
                Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);
                GraphicsDevice.Clear(backColor);

                ActiveVisualNovel.Draw(g);
            }

            g.End();

            Thread.Sleep(13);
        }

        public SimpleAnimation CopyAnimatedCharacter(string Name, string Path)
        {
            SimpleAnimation NewCharacter = new SimpleAnimation(Name, Path, new AnimationLooped(Path));
            NewCharacter.ActiveAnimation.Content = content;
            NewCharacter.ActiveAnimation.Load();

            return NewCharacter;
        }

        public void AddBackground(string Name, string Path)
        {
            VisualNovelBackground NewBackground = new VisualNovelBackground(Name, Path, content.Load<Texture2D>("Visual Novels/Backgrounds/" + Path));
            ActiveVisualNovel.ListBackground.Add(NewBackground);
        }

        private void DrawScriptEditor()
        {
            int TimelineLimitX = 100;
            if (ScriptEditorOrigin.X <= TimelineLimitX)
            {
                GameScreen.DrawLine(g, new Vector2(TimelineLimitX - ScriptEditorOrigin.X, 0), new Vector2(TimelineLimitX - ScriptEditorOrigin.X, this.Height), Color.Gray);
            }

            foreach (Dialog ActiveDialog in ActiveVisualNovel.ListDialog)
            {
                if (ActiveDialog.Position.X + 90 - ScriptEditorOrigin.X < 0
                    && ActiveDialog.Position.X - 10 - ScriptEditorOrigin.X > this.Width
                    && ActiveDialog.Position.Y - ScriptEditorOrigin.Y - BoxHeight < 0
                    && ActiveDialog.Position.Y - ScriptEditorOrigin.Y > this.Height)
                    continue;

                //Draw the image for Script.
                DrawScriptBox(g, ActiveDialog.Position.X - ScriptEditorOrigin.X, ActiveDialog.Position.Y - ScriptEditorOrigin.Y, BoxWidth, BoxHeight);

                //If the Script is selected, rectangle it.
                if (ListDialogSelected.Contains(ActiveDialog))
                {
                    GameScreen.DrawRectangle(g, new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X,
                                                ActiveDialog.Position.Y - ScriptEditorOrigin.Y),
                                new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth,
                                            ActiveDialog.Position.Y - ScriptEditorOrigin.Y + BoxHeight), Color.Black);
                }

                //If it's in the Timeline, draw it's frame number.
                if (ActiveVisualNovel.Timeline.Contains(ActiveDialog))
                {
                    g.DrawString(fntFrameCount, "Frame " + (ActiveVisualNovel.Timeline.IndexOf((Dialog)ActiveDialog) + 1),
                        new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + 2,
                                    ActiveDialog.Position.Y - ScriptEditorOrigin.Y), Color.White);
                }

                g.Draw(sprPixel, new Rectangle(ActiveDialog.Position.X - ScriptEditorOrigin.X - 10,
                    ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 2, 7, 7), Color.Black);
                g.DrawString(fntFrameCount, "Dialog", new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + 2,
                    ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 12), Color.White);

                g.DrawStringRightAligned(fntFrameCount, "Dialogs", new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth - 5,
                    ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 24), Color.White);
                g.Draw(sprPixel, new Rectangle(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth + 7,
                    ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 28, 7, 7), Color.Black);

                if (string.IsNullOrEmpty(ActiveDialog.TextPreview))
                {
                    if (!string.IsNullOrEmpty(ActiveDialog.Text))
                    {
                        g.DrawStringRightAligned(fntFrameCount, TextHelper.FitToWidth(fntFrameCount, ActiveDialog.Text, 80)[0],
                            new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth - 5,
                            ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 36), Color.White);
                    }
                }
                else
                {
                    g.DrawStringRightAligned(fntFrameCount, TextHelper.FitToWidth(fntFrameCount, ActiveDialog.TextPreview, 80)[0],
                        new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth - 5,
                        ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 36), Color.White);
                }

                for (int D2 = 0; D2 < ActiveDialog.ListNextDialog.Count; D2++)
                {
                    GameScreen.DrawLine(g, new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth + 10, ActiveDialog.Position.Y - ScriptEditorOrigin.Y + 31),
                                new Vector2(ActiveVisualNovel.ListDialog[ActiveDialog.ListNextDialog[D2]].Position.X - ScriptEditorOrigin.X - 7, ActiveVisualNovel.ListDialog[ActiveDialog.ListNextDialog[D2]].Position.Y - ScriptEditorOrigin.Y + 5), Color.Black);
                }

                if (ActiveDialog.CutsceneBefore != null && ActiveDialog.CutsceneBefore.ListCutsceneBehavior.Count > 0)
                {
                    g.DrawStringRightAligned(fntFrameCount, ActiveDialog.CutsceneBefore.DicActionScript.Count.ToString(),
                        new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth - 25,
                        ActiveDialog.Position.Y - ScriptEditorOrigin.Y), Color.Red);
                }
                if (ActiveDialog.CutsceneAfter != null && ActiveDialog.CutsceneAfter.DicActionScript.Count > 0)
                {
                    g.DrawStringRightAligned(fntFrameCount, ActiveDialog.CutsceneAfter.DicActionScript.Count.ToString(),
                        new Vector2(ActiveDialog.Position.X - ScriptEditorOrigin.X + BoxWidth - 10,
                        ActiveDialog.Position.Y - ScriptEditorOrigin.Y), Color.Red);
                }
            }
            //Draw a line between the mouse and the starting LinkedScript seletected.
            if (ScriptLinkTypeChoice != ScriptLinkType.None)
                GameScreen.DrawLine(g, new Vector2(ScriptLinkStartingPoint.X - ScriptEditorOrigin.X, ScriptLinkStartingPoint.Y - ScriptEditorOrigin.Y), new Vector2(MouseX, MouseY), Color.Black);
        }
    }
}
