﻿using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelInitDelayedAttackMAP : ActionPanelDeathmatch
    {
        private const string PanelName = "InitAttackMAP";

        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private readonly DelayedAttack ActiveDelayedAttack;

        public ActionPanelInitDelayedAttackMAP(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelInitDelayedAttackMAP(DeathmatchMap Map, int ActivePlayerIndex, DelayedAttack ActiveDelayedAttack)
            : base(PanelName, Map)
        {
            ActiveSquad = ActiveDelayedAttack.Owner;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveDelayedAttack = ActiveDelayedAttack;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.AttackWithMAPAttack(ActivePlayerIndex, Map.ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad), Map.GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack, ActiveDelayedAttack.ListAttackPosition));
            RemoveFromPanelList(this);
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