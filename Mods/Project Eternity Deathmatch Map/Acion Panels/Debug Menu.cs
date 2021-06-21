using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDebug : ActionPanelDeathmatch
    {
        Squad ActiveSquad;
        private List<List<BaseEffect>> ListSquadEffect;

        public ActionPanelDebug(Squad ActiveSquad, DeathmatchMap Map)
            : base("Info", Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ListSquadEffect = new List<List<BaseEffect>>(ActiveSquad.UnitsAliveInSquad);
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                List<BaseEffect> ListUnitEffect = new List<BaseEffect>();
                foreach (List<BaseEffect> ListPilotEffect in ActiveSquad[U].Pilot.Effects.GetEffects().Values)
                {
                    ListUnitEffect.AddRange(ListPilotEffect);
                }
                ListSquadEffect.Add(ListUnitEffect);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
            {
                int X = 20 + U * 205;
                GameScreen.DrawBox(g, new Vector2(X, 20), 200, 400, Color.White);
                GameScreen.DrawText(g, ActiveSquad[U].RelativePath, new Vector2(X + 5, 25), Color.White);
                for (int E = 0; E < ListSquadEffect[U].Count; ++E)
                {
                    GameScreen.DrawText(g, ListSquadEffect[U][E].ToString(), new Vector2(X + 10, 45 + E * 20), Color.White);
                }
            }
        }
    }
}
