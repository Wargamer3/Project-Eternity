﻿using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Editor;
using static ProjectEternity.GameScreens.BattleMapScreen.Terrain;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TilesetViewerControl : GraphicsDeviceControl
    {
        public ContentManager content;
        private SpriteBatch g;
        private Texture2D sprPixel;
        private Texture2D sprTileset;
        private Point TileSize;

        public Point DrawOffset;
        public List<Rectangle> ListTileBrush;//X, Y position of the cursor in the TilePreview, used to select the origin for the next Tile.
        public List<Color> ListTileBrushColor;
        public List<TilesetPreset> ListSmartTilesetPresets;
        public List<Texture2D> ListTilesetPresetsSprite;

        public TilesetViewerControl()
        {
            ListSmartTilesetPresets = new List<TilesetPreset>();
            ListTilesetPresetsSprite = new List<Texture2D>();
            ListTileBrush = new List<Rectangle>();
            ListTileBrushColor = new List<Color>();
            ListTileBrush.Add(new Rectangle());
            ListTileBrushColor.Add(Color.Red);
            ListTileBrushColor.Add(Color.Yellow);
            ListTileBrushColor.Add(Color.Green);
            ListTileBrushColor.Add(Color.Blue);
        }

        protected override void Initialize()
        {
            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };

            Mouse.WindowHandle = this.Handle;

            content = new ContentManager(Services, "Content");

            g = new SpriteBatch(GraphicsDevice);
            sprPixel = content.Load<Texture2D>("Pixel");
        }

        public void Preload()
        {
            OnCreateControl();
        }

        public void InitTileset(Texture2D sprTileset, Point TileSize)
        {
            this.sprTileset = sprTileset;
            this.TileSize = TileSize;
            ListTileBrush[0] = new Rectangle(0, 0, TileSize.X, TileSize.Y);
        }

        public void InitTileset(string Tileset, Point TileSize)
        {
            if (!string.IsNullOrEmpty(Tileset))
            {
                sprTileset = content.Load<Texture2D>("Maps/Tilesets/" + Tileset);
            }
            this.TileSize = TileSize;
            ListTileBrush[0] = new Rectangle(0, 0, TileSize.X, TileSize.Y);
        }

        public void SelectTile(Point TileToSelect, bool ExpendSelection, int BrushIndex)
        {
            while (BrushIndex >= ListTileBrush.Count)
            {
                ListTileBrush.Add(ListTileBrush[0]);
            }
            while (BrushIndex >= ListTileBrushColor.Count)
            {
                ListTileBrushColor.Add(Color.FromNonPremultiplied(Core.RandomHelper.Next(255), Core.RandomHelper.Next(255), Core.RandomHelper.Next(255), 255));
            }

            Rectangle TileBrushSize = ListTileBrush[BrushIndex];

            if (ExpendSelection)
            {
                if (TileToSelect.X > TileBrushSize.X)
                {
                    TileBrushSize.Width = TileToSelect.X - TileBrushSize.X + TileSize.X;
                }
                if (TileToSelect.Y > TileBrushSize.Y)
                {
                    TileBrushSize.Height = TileToSelect.Y - TileBrushSize.Y + TileSize.Y;
                }
            }
            else
            {
                TileBrushSize.Location = TileToSelect;
                TileBrushSize.Width = TileSize.X;
                TileBrushSize.Height = TileSize.Y;
            }

            ListTileBrush[BrushIndex] = TileBrushSize;
        }

        public Point GetTileFromBrush(Point MapTileToPaint, int BrushIndex)
        {
            while (BrushIndex >= ListTileBrush.Count)
            {
                ListTileBrush.Add(ListTileBrush[0]);
            }
            while (BrushIndex >= ListTileBrushColor.Count)
            {
                ListTileBrushColor.Add(Color.FromNonPremultiplied(Core.RandomHelper.Next(255), Core.RandomHelper.Next(255), Core.RandomHelper.Next(255), 255));
            }

            Rectangle TileBrushPosition = ListTileBrush[BrushIndex];

            if (ListSmartTilesetPresets.Count > 0)
            {
            }

            Point RealTile = new Point();
            RealTile.X = TileBrushPosition.X + MapTileToPaint.X % TileBrushPosition.Width;
            RealTile.Y = TileBrushPosition.Y + MapTileToPaint.Y % TileBrushPosition.Height;

            return RealTile;
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            // Clear to the default control background color.
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            int FirstLineY = -DrawOffset.Y;

            if (ListSmartTilesetPresets.Count > 0)
            {
                Point Position = Point.Zero;
                for (int i = 0; i < ListSmartTilesetPresets.Count; i++)
                {
                    TilesetPreset ActivePreset = ListSmartTilesetPresets[i];
                    ActivePreset.DrawPreview(g, Position, ListTilesetPresetsSprite[i]);
                    Position.X += TileSize.X;
                }
                FirstLineY += TileSize.Y;
            }

            if (sprTileset != null)
            {
                g.Draw(sprTileset, new Vector2(-DrawOffset.X, FirstLineY), Color.White);

                for (int B = 0; B < ListTileBrush.Count; B++)
                {
                    Rectangle TileBrushSize = ListTileBrush[B];
                    Color DrawColor = ListTileBrushColor[B];
                    g.Draw(sprPixel, new Rectangle(TileBrushSize.X - DrawOffset.X, TileBrushSize.Y - DrawOffset.Y, TileBrushSize.Width, 1), DrawColor);
                    g.Draw(sprPixel, new Rectangle(TileBrushSize.X - DrawOffset.X, TileBrushSize.Y - DrawOffset.Y, 1, TileBrushSize.Height), DrawColor);
                    g.Draw(sprPixel, new Rectangle(TileBrushSize.X - DrawOffset.X, TileBrushSize.Y - DrawOffset.Y + TileBrushSize.Height, TileBrushSize.Width, 1), DrawColor);
                    g.Draw(sprPixel, new Rectangle(TileBrushSize.X - DrawOffset.X + TileBrushSize.Width, TileBrushSize.Y - DrawOffset.Y, 1, TileBrushSize.Height), DrawColor);
                }
            }

            g.End();
        }
    }
}