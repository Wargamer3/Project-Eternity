using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    /// <summary>
    /// Used to make menus.
    /// </summary>
    public abstract class ActionPanelWorldMap : BattleMapActionPanel
    {
        protected WorldMap Map;

        public ActionPanelWorldMap(string Name, WorldMap Map, bool CanCancel = true)
            : base(Name, Map.ListActionMenuChoice, new KeyboardInput(), CanCancel)
        {
            this.Map = Map;
        }

        protected override void OnCancelPanel()
        {
            Map.sndCancel.Play();
        }

        public void DrawNextChoice(CustomSpriteBatch g)
        {
            //Draw the action panel.
            float X = (Map.CursorPosition.X - Map.Camera2DPosition.X + 1) * Map.TileSize.X;
            float Y = (Map.CursorPosition.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y;

            if (X + MinActionMenuWidth >= Constants.Width)
                X = Constants.Width - MinActionMenuWidth;

            int MenuHeight = (ListNextChoice.Count) * PannelHeight + 6 * 2;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;

            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), MinActionMenuWidth, MenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            for (int A = 0; A < ListNextChoice.Count; A++)
            {
                TextHelper.DrawText(g, ListNextChoice[A].ToString(), new Vector2(X, Y), Color.White);
                Y += PannelHeight;
            }

            Y = (Map.CursorPosition.Y - Map.Camera2DPosition.Y) * Map.TileSize.Y;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X, 9 + (int)Y + ActionMenuCursor * PannelHeight, MinActionMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }
    }
}
