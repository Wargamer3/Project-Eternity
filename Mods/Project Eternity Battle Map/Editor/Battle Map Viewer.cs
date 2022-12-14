using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapViewerControl : GraphicsDeviceControl
    {
        public enum ScriptLinkTypes { None, Trigger, Event };

        public BattleMap ActiveMap;
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public int ViewerIndex;

        private HScrollBar sclMapWidth;
        private VScrollBar sclMapHeight;
        public ContextMenuStrip cmsScriptMenu;
        private ToolStripMenuItem tsmDeleteScript;
        private System.Diagnostics.Stopwatch Timer;

        public MapScriptGUIHelper Helper;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
            this.SizeChanged += new System.EventHandler(this.BattleMapViewerControl_SizeChanged);

            Timer = new System.Diagnostics.Stopwatch();
            Mouse.WindowHandle = this.Handle;


            content = new ContentManager(Services, "Content");
            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            Helper = new MapScriptGUIHelper();
            Helper.Load(content, g, GraphicsDevice);
            sprPixel = content.Load<Texture2D>("Pixel");

            this.sclMapWidth = new HScrollBar();
            this.sclMapHeight = new VScrollBar();

            // 
            // sclMapWidth
            // 
            this.sclMapWidth.Location = new System.Drawing.Point(0, 506);
            this.sclMapWidth.Name = "sclMapWidth";
            this.sclMapWidth.Size = new System.Drawing.Size(650, 17);
            this.sclMapWidth.TabIndex = 3;
            this.sclMapWidth.Scroll += new ScrollEventHandler(this.sclMapWidth_Scroll);
            // 
            // sclMapHeight
            // 
            this.sclMapHeight.Location = new System.Drawing.Point(650, 0);
            this.sclMapHeight.Name = "sclMapHeight";
            this.sclMapHeight.Size = new System.Drawing.Size(17, 506);
            this.sclMapHeight.TabIndex = 4;
            this.sclMapHeight.Scroll += new ScrollEventHandler(this.sclMapHeight_Scroll);
            Controls.Add(this.sclMapWidth);
            Controls.Add(this.sclMapHeight);

            this.cmsScriptMenu = new ContextMenuStrip();
            this.tsmDeleteScript = new ToolStripMenuItem();
            // 
            // cmsScriptMenu
            // 
            this.cmsScriptMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmDeleteScript});
            this.cmsScriptMenu.Name = "cmsScriptMenu";
            this.cmsScriptMenu.Size = new System.Drawing.Size(141, 26);

            // 
            // tsmDeleteScript
            //
            this.tsmDeleteScript.Name = "tsmDeleteScript";
            this.tsmDeleteScript.Size = new System.Drawing.Size(140, 22);
            this.tsmDeleteScript.Text = "Delete Script";
            this.tsmDeleteScript.Click += tsmDeleteScript_Click;
        }

        public void SetListMapScript(List<MapScript> ListMapScript)
        {
            Helper.SetListMapScript(ListMapScript);
        }

        public void RefreshScrollbars()
        {
            //Give the picture box it's new size(its maximum size will make sure it fits)
            System.Drawing.Size NewMapSize = new System.Drawing.Size(ActiveMap.MapSize.X * ActiveMap.TileSize.X, ActiveMap.MapSize.Y * ActiveMap.TileSize.Y);

            //Initialise the scroll bars.
            if (NewMapSize.Width >= Width)
            {
                sclMapWidth.Maximum = ActiveMap.MapSize.X;
                sclMapWidth.Visible = true;
            }
            else
                sclMapWidth.Visible = false;

            sclMapWidth.Location = new System.Drawing.Point(0, this.Height - sclMapWidth.Height);
            sclMapWidth.Width = this.Width - sclMapHeight.Width;

            if (NewMapSize.Height >= Height)
            {
                sclMapHeight.Maximum = ActiveMap.MapSize.Y;
                sclMapHeight.Visible = true;
            }
            else
                sclMapHeight.Visible = false;

            sclMapHeight.Location = new System.Drawing.Point(this.Width - sclMapHeight.Width, 0);
            sclMapHeight.Height = this.Height - sclMapWidth.Height;
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
            Thread.Sleep(33);
            if (ActiveMap != null)
            {
                double elapsed = Timer.Elapsed.TotalSeconds;
                Timer.Restart();

                ActiveMap.Update(new GameTime(TimeSpan.FromSeconds(elapsed), TimeSpan.FromSeconds(elapsed)));

                for (int S = ActiveMap.ListGameScreen.Count - 1; S >= 0; --S)
                    ActiveMap.ListGameScreen[S].Update(new GameTime(TimeSpan.FromSeconds(elapsed), TimeSpan.FromSeconds(elapsed)));
                // Clear to the default control background color.
                Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

                GraphicsDevice.Clear(backColor);

                if (ViewerIndex == 2)
                    Helper.DrawScripts();
                else
                    DrawMap();
            }
        }

        private void DrawMap()
        {
            GraphicsDevice.Clear(Color.Black);

            if (ActiveMap.ListTileSet.Count > 0)
            {
                ActiveMap.BeginDraw(g);
            }

            GraphicsDevice.SetRenderTarget(null);
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (ActiveMap.ListTileSet.Count > 0)
            {
                ActiveMap.Draw(g);
            }

            g.End();
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            ActiveMap.EndDraw(g);
            g.End();
        }
                
        #region Scripting

        private void tsmDeleteScript_Click(object sender, EventArgs e)
        {
            Helper.DeleteSelectedScript();
        }

        public void Scripting_MouseDown(MouseEventArgs e)
        {
            Helper.Select(e.Location);
        }

        public void Scripting_MouseUp(MouseEventArgs e)
        {
            Helper.Scripting_MouseUp(e.Location, (e.Button & MouseButtons.Left) == MouseButtons.Left, (e.Button & MouseButtons.Right) == MouseButtons.Right);
        }

        public void Scripting_MouseMove(MouseEventArgs e)
        {
            int MaxX, MaxY;
            Helper.MoveScript(e.Location, out MaxX, out MaxY);

            if (MaxX >= Width)
            {
                sclMapWidth.Maximum = MaxX - Size.Width;
                sclMapWidth.Visible = true;
            }
            else
                sclMapWidth.Visible = false;

            if (MaxY >= Height)
            {
                sclMapHeight.Maximum = MaxY - Size.Height;
                sclMapHeight.Visible = true;
            }
            else
                sclMapHeight.Visible = false;
        }

        #endregion

        #region Zones

        public void Zones_MouseDown(MouseEventArgs e)
        {
        }

        public void Zones_MouseUp(MouseEventArgs e)
        {
        }

        public void Zones_MouseMove(MouseEventArgs e)
        {
        }

        #endregion

        private void sclMapWidth_Scroll(object sender, ScrollEventArgs e)
        {
            switch (ViewerIndex)
            {
                case 0:
                case 1:
                    ActiveMap.CameraPosition.X = e.NewValue;
                    break;

                case 2:
                    Helper.ScriptStartingPos.X = e.NewValue;
                    break;
            }
        }

        private void sclMapHeight_Scroll(object sender, ScrollEventArgs e)
        {
            switch (ViewerIndex)
            {
                case 0:
                case 1:
                    ActiveMap.CameraPosition.Y = e.NewValue;
                    break;

                case 2:
                    Helper.ScriptStartingPos.Y = e.NewValue;
                    break;
            }
        }
        
        private void BattleMapViewerControl_SizeChanged(object sender, EventArgs e)
        {
            if (ActiveMap == null)
            {
                return;
            }

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Width, Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;

            ActiveMap.fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

            RefreshScrollbars();
        }
    }
}
