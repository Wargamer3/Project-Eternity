using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelInitDelayedAttackMAP : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private int ActivePlayerIndex;
        private readonly DelayedAttack ActiveDelayedAttack;

        public ActionPanelInitDelayedAttackMAP(DeathmatchMap Map, int ActivePlayerIndex, DelayedAttack ActiveDelayedAttack)
            : base("Init Attack MAP", Map)
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
            Map.AttackWithMAPAttack(ActiveSquad, ActivePlayerIndex, Map.GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack, ActiveDelayedAttack.ListAttackPosition));
            RemoveFromPanelList(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
