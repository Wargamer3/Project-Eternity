﻿using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public enum ScriptLinkType { None = 0, FromDialog, ToDialog };

    internal class VisualNovelViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public Texture2D imgScript;

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
            imgScript = content.Load<Texture2D>("Script");
            fntFrameCount = content.Load<SpriteFont>("Fonts/Calibri8");
        }

        public void Preload()
        {
            OnCreateControl();
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
            if (ScriptEditorOrigin.X <= imgScript.Width)
                GameScreen.DrawLine(g, new Vector2(imgScript.Width - ScriptEditorOrigin.X, 0), new Vector2(imgScript.Width - ScriptEditorOrigin.X, this.Height), Color.Gray);
            for (int D = 0; D < ActiveVisualNovel.ListDialog.Count; D++)
            {
                if (ActiveVisualNovel.ListDialog[D].Position.X + 90 - ScriptEditorOrigin.X < 0
                    && ActiveVisualNovel.ListDialog[D].Position.X - 10 - ScriptEditorOrigin.X > this.Width
                    && ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y - imgScript.Height < 0
                    && ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y > this.Height)
                    continue;

                //Draw the image for Script.
                g.Draw(imgScript, new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y), Color.White);

                //If the Script is selected, rectangle it.
                if (ListDialogSelected.Contains(ActiveVisualNovel.ListDialog[D]))
                {
                    GameScreen.DrawRectangle(g, new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X,
                                                ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y),
                                new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + imgScript.Width,
                                            ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + imgScript.Height), Color.Black);
                }

                //If it's in the Timeline, draw it's frame number.
                if (ActiveVisualNovel.Timeline.Contains(ActiveVisualNovel.ListDialog[D]))
                {
                    g.DrawString(fntFrameCount, "Frame " + (ActiveVisualNovel.Timeline.IndexOf((Dialog)ActiveVisualNovel.ListDialog[D]) + 1),
                        new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 2,
                                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y), Color.Black);
                }
                g.Draw(sprPixel, new Rectangle(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X - 10,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 2, 7, 7), Color.Black);
                g.DrawString(fntFrameCount, "Dialog", new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 2,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 12), Color.Black);

                g.DrawString(fntFrameCount, "Dialogs", new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 42,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 24), Color.Black);
                g.Draw(sprPixel, new Rectangle(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 83,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 28, 7, 7), Color.Black);

                g.DrawString(fntFrameCount, "Scripts", new Vector2(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 44,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 36), Color.Black);
                g.Draw(sprPixel, new Rectangle(ActiveVisualNovel.ListDialog[D].Position.X - ScriptEditorOrigin.X + 83,
                    ActiveVisualNovel.ListDialog[D].Position.Y - ScriptEditorOrigin.Y + 40, 7, 7), Color.Black);
                
                Dialog CurrentDialog = (Dialog)ActiveVisualNovel.ListDialog[D];
                for (int D2 = 0; D2 < CurrentDialog.ListNextDialog.Count; D2++)
                {
                    GameScreen.DrawLine(g, new Vector2(CurrentDialog.Position.X - ScriptEditorOrigin.X + 86, CurrentDialog.Position.Y - ScriptEditorOrigin.Y + 31),
                                new Vector2(ActiveVisualNovel.ListDialog[CurrentDialog.ListNextDialog[D2]].Position.X - ScriptEditorOrigin.X - 7, ActiveVisualNovel.ListDialog[CurrentDialog.ListNextDialog[D2]].Position.Y - ScriptEditorOrigin.Y + 5), Color.Black);
                }
            }
            //Draw a line between the mouse and the starting LinkedScript seletected.
            if (ScriptLinkTypeChoice != ScriptLinkType.None)
                GameScreen.DrawLine(g, new Vector2(ScriptLinkStartingPoint.X - ScriptEditorOrigin.X, ScriptLinkStartingPoint.Y - ScriptEditorOrigin.Y), new Vector2(MouseX, MouseY), Color.Black);
        }
    }
}
