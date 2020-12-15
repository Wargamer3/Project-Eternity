using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMapChange : ActionPanelDeathmatch
    {
        private SpriteFont fntFinlanderFont;

        private List<BattleMapScreen.BattleMap> ListMapChangeChoice;

        public ActionPanelMapChange(DeathmatchMap Map, SpriteFont fntFinlanderFont)
            : base("Change Map", Map)
        {
            this.fntFinlanderFont = fntFinlanderFont;

            ListMapChangeChoice = new List<BattleMapScreen.BattleMap>();
        }

        public override void OnSelect()
        {
            ListMapChangeChoice = ActionPanelMapSwitch.GetActiveSubMaps(Map);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                Map.ListGameScreen.Remove(Map);
                Map.ListGameScreen.Insert(0, ListMapChangeChoice[ActionMenuCursor]);
                RemoveAllSubActionPanels();
            }
            if (InputHelper.InputUpPressed())
            {
                --ActionMenuCursor;
                if (ActionMenuCursor < 0)
                    ActionMenuCursor = ListMapChangeChoice.Count - 1;
            }
            else if (InputHelper.InputDownPressed())
            {
                ++ActionMenuCursor;
                if (ActionMenuCursor >= ListMapChangeChoice.Count)
                    ActionMenuCursor = 0;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = Constants.Width / 2 - 200;
            float Y = Constants.Height / 2 - 50;

            GameScreen.DrawBox(g, new Vector2(X, Y), 400, 10 + ListMapChangeChoice.Count * 22, Color.White);
            foreach (BattleMapScreen.BattleMap ActiveMap in ListMapChangeChoice)
            {
                g.DrawString(fntFinlanderFont, ActiveMap.MapName, new Vector2(X + 5, Y), Color.White);
                Y += 22;
            }
            Y = Constants.Height / 2 - 50;
            GameScreen.DrawLine(g, new Vector2(X + 5, Y + 6 + ActionMenuCursor * 22), new Vector2(X + 375, Y + 6 + ActionMenuCursor * 22), Color.FromNonPremultiplied(255, 255, 255, 127), 20);
        }
    }
}
