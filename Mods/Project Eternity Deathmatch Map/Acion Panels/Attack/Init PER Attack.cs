using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelInitAttackPER : ActionPanelDeathmatch
    {
        private const string PanelName = "InitAttackPER";

        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private readonly PERAttack ActivePERAttack;

        public ActionPanelInitAttackPER(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelInitAttackPER(DeathmatchMap Map, int ActivePlayerIndex, PERAttack ActivePERAttack)
            : base(PanelName, Map)
        {
            ActiveSquad = ActivePERAttack.Owner;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActivePERAttack = ActivePERAttack;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Stack<Tuple<int, int>> ListMAPAttackTarget = Map.GetEnemies(true, new List<Vector3>() { ActivePERAttack.Position });

            Map.AttackWithMAPAttack(ActivePlayerIndex, Map.ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad), ListMAPAttackTarget);
            RemoveFromPanelList(this);

            if (ActivePERAttack.Lifetime <= 0 || ListMAPAttackTarget.Count > 0)
            {
                Map.ListPERAttack.Remove(ActivePERAttack);
            }
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
            return new ActionPanelInitDelayedAttackMAP(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
