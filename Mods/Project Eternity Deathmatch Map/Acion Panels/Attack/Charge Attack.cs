using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelChargeAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "ChargeAttack";

        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private List<Vector3> ListMVHoverPoints;

        public ActionPanelChargeAttack(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelChargeAttack(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveSquad.Position.Z].ListProp)
            {
                ActiveProp.FinishMoving(ActiveSquad, ListMVHoverPoints);
            }

            ActiveSquad.CurrentLeader.ChargeAttack();
            Map.FinalizeMovement(ActiveSquad, (int)Map.GetTerrain(ActiveSquad.Position).MovementCost, ListMVHoverPoints);
            ActiveSquad.EndTurn();
            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            RemoveAllSubActionPanels();
        }

        public override void DoRead(ByteReader BR)
        {
            throw new NotImplementedException();
        }

        public override void DoWrite(ByteWriter BW)
        {
            throw new NotImplementedException();
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelChargeAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
