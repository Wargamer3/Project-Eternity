using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelWait : ActionPanelDeathmatch
    {
        private const string PanelName = "Wait";

        private readonly Squad ActiveSquad;
        private readonly List<Vector3> ListMVHoverPoints;

        public ActionPanelWait(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelWait(DeathmatchMap Map, Squad ActiveSquad, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ListMVHoverPoints = ListMVHoverPoints;
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveSquad, (int)Map.GetTerrain(ActiveSquad).MovementCost, ListMVHoverPoints);
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
