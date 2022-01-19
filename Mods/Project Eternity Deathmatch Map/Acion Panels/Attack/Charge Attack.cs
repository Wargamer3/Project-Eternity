using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelChargeAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "ChargeAttack";

        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private readonly DelayedAttack ActiveDelayedAttack;

        public ActionPanelChargeAttack(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelChargeAttack(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentLeader.ChargeAttack();
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
