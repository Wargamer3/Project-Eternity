using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelWait : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelWait(DeathmatchMap Map, Squad ActiveSquad)
            : base("Wait", Map)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
