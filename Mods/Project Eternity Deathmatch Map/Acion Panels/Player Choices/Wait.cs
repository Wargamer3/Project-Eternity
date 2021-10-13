using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelWait : ActionPanelDeathmatch
    {
        private const string PanelName = "Wait";

        private Squad ActiveSquad;

        public ActionPanelWait(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelWait(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveSquad, (int)Map.GetTerrain(ActiveSquad).MovementCost);
            ActiveSquad.EndTurn();
            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);

            Map.ActiveSquadIndex = -1;
            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelWait(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
