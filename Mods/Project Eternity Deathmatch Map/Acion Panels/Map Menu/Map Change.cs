using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMapChange : ActionPanelDeathmatch
    {
        private const string PanelName = "Change Map";

        private SpriteFont fntFinlanderFont;

        private List<BattleMapScreen.BattleMap> ListMapChangeChoice;

        public ActionPanelMapChange(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            this.fntFinlanderFont = Map.fntFinlanderFont;

            ListMapChangeChoice = new List<BattleMapScreen.BattleMap>();
        }

        public ActionPanelMapChange(DeathmatchMap Map, SpriteFont fntFinlanderFont)
            : base(PanelName, Map)
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
            if (ActiveInputManager.InputConfirmPressed())
            {
                Map.ListGameScreen.Remove(Map);
                Map.ListGameScreen.Insert(0, ListMapChangeChoice[ActionMenuCursor]);
                RemoveAllSubActionPanels();
            }
            if (ActiveInputManager.InputUpPressed())
            {
                --ActionMenuCursor;
                if (ActionMenuCursor < 0)
                    ActionMenuCursor = ListMapChangeChoice.Count - 1;
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++ActionMenuCursor;
                if (ActionMenuCursor >= ListMapChangeChoice.Count)
                    ActionMenuCursor = 0;
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActionMenuCursor = BR.ReadInt32();
            int ListMapChangeChoiceCount = BR.ReadInt32();
            ListMapChangeChoice = new List<BattleMapScreen.BattleMap>(ListMapChangeChoiceCount);
            for (int C = 0; C < ListMapChangeChoice.Count; ++C)
            {
                DeathmatchMap NewMap = new DeathmatchMap();
                NewMap.MapName = BR.ReadString();
                ListMapChangeChoice.Add(NewMap);
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActionMenuCursor);
            BW.AppendInt32(ListMapChangeChoice.Count);
            for (int C = 0; C < ListMapChangeChoice.Count; ++C)
            {
                BW.AppendString(ListMapChangeChoice[C].MapName);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMapChange(Map);
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
