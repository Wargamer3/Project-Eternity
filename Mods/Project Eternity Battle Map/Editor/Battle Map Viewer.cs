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
    public interface IMapEditorTab
    {
        BattleMapViewerControl BattleMapViewer { get; set; }
        IMapHelper Helper { get; set; }
        TabPage InitTab(MenuStrip mnuToolBar);
        bool TabProcessCmdKey(ref Message msg, System.Windows.Forms.Keys keyData);
        void TabOnMouseDown(MouseEventArgs e);
        void TabOnMouseUp(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e, int MouseX, int MouseY);
        void OnMapResize(int NewMapSizeX, int NewMapSizeY);
        void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice);
        void OnMapLoaded();
        void DrawInfo(ToolStripStatusLabel tslInformation);
    }

    public class BattleMapViewerControl : GraphicsDeviceControl
    {
        public enum ScriptLinkTypes { None, Trigger, Event };

        public BattleMap ActiveMap;
        public ContentManager content;
        private CustomSpriteBatch g;
        private Texture2D sprPixel;
        public int SelectedTilesetIndex;
        public int SelectedListLayerIndex;
        public Rectangle TileReplacementZone;

        private HScrollBar sclMapWidth;
        private VScrollBar sclMapHeight;
        public ContextMenuStrip cmsScriptMenu;
        private System.Diagnostics.Stopwatch Timer;

        public IMapEditorTab ActiveTab;
        public MapScriptGUIHelper ScriptHelper;
        public IMapHelper Helper;
        public TilesetViewerControl TilesetViewer;

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
            this.SizeChanged += new System.EventHandler(this.BattleMapViewerControl_SizeChanged);

            Timer = new System.Diagnostics.Stopwatch();
            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");
            g = new CustomSpriteBatch(new SpriteBatch(GraphicsDevice));
            ScriptHelper = new MapScriptGUIHelper();
            ScriptHelper.Load(content, g, GraphicsDevice);
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
            // 
            // cmsScriptMenu
            // 
            this.cmsScriptMenu.Name = "cmsScriptMenu";
            this.cmsScriptMenu.Size = new System.Drawing.Size(141, 26);
        }

        public void SetListMapScript(List<MapScript> ListMapScript)
        {
            ScriptHelper.SetListMapScript(ListMapScript);
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

                if (ActiveTab != null)
                {
                    ActiveTab.DrawMap(g, GraphicsDevice);
                }
                else
                {
                    DrawMap();
                }
            }
        }

        public void DrawMap()
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

            if (TileReplacementZone.Width > 0 && TileReplacementZone.Height > 0)
            {
                GameScreen.DrawRectangle(g, new Vector2(TileReplacementZone.X * ActiveMap.TileSize.X, TileReplacementZone.Y * ActiveMap.TileSize.Y), new Vector2(TileReplacementZone.Right * ActiveMap.TileSize.X, TileReplacementZone.Bottom * ActiveMap.TileSize.Y), Color.Red);
            }
            g.End();
        }

        public int GetRealTopLayerIndex(int LayerIndex)
        {
            int RealIndex = 0;
            int LastTopLayerIndex = -1;

            foreach (object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                if (!(ActiveLayer is ISubMapLayer))
                {
                    ++LastTopLayerIndex;
                }
                if (RealIndex == LayerIndex)
                {
                    return LastTopLayerIndex;
                }

                ++RealIndex;
            }

            return LastTopLayerIndex;
        }

        public void UpdateDimensions(int MaxX, int MaxY)
        {
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

        private void sclMapWidth_Scroll(object sender, ScrollEventArgs e)
        {
            switch (0)
            {
                case 0:
                case 1:
                    ActiveMap.Camera2DPosition.X = e.NewValue;
                    ScriptHelper.ScriptStartingPos.X = e.NewValue;
                    break;

                case 2:
                    ScriptHelper.ScriptStartingPos.X = e.NewValue;
                    break;
            }
        }

        private void sclMapHeight_Scroll(object sender, ScrollEventArgs e)
        {
            switch (0)
            {
                case 0:
                case 1:
                    ActiveMap.Camera2DPosition.Y = e.NewValue;
                    break;

                case 2:
                    ScriptHelper.ScriptStartingPos.Y = e.NewValue;
                    break;
            }
        }
        
        private void BattleMapViewerControl_SizeChanged(object sender, EventArgs e)
        {
            if (ActiveMap == null)
            {
                return;
            }

            ActiveMap.Resize(Width, Height);

            RefreshScrollbars();
        }
    }
}
