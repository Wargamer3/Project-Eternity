using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Editor;
using Microsoft.Xna.Framework.Graphics;

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
        public Point ActiveTile;//X, Y position of the cursor in the TilePreview, used to select the origin for the next Tile.

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
        }

        public void InitTileset(string Tileset, Point TileSize)
        {
            if (!string.IsNullOrEmpty(Tileset))
            {
                sprTileset = content.Load<Texture2D>("Maps/Tilesets/" + Tileset);
            }
            this.TileSize = TileSize;
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

            if (sprTileset != null)
            {
                g.Draw(sprTileset, new Vector2(-DrawOffset.X, -DrawOffset.Y), Color.White);
                g.Draw(sprPixel, new Rectangle(ActiveTile.X - DrawOffset.X, ActiveTile.Y - DrawOffset.Y, TileSize.X, 1), Color.Red);
                g.Draw(sprPixel, new Rectangle(ActiveTile.X - DrawOffset.X, ActiveTile.Y - DrawOffset.Y, 1, TileSize.Y), Color.Red);
                g.Draw(sprPixel, new Rectangle(ActiveTile.X - DrawOffset.X, ActiveTile.Y - DrawOffset.Y + TileSize.Y, TileSize.X, 1), Color.Red);
                g.Draw(sprPixel, new Rectangle(ActiveTile.X - DrawOffset.X + TileSize.X, ActiveTile.Y - DrawOffset.Y, 1, TileSize.Y), Color.Red);
            }

            g.End();
        }
    }
}