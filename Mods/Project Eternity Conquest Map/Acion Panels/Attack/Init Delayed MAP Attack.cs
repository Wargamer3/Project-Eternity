using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelInitDelayedAttackMAP : ActionPanelConquest
    {
        private const string PanelName = "InitAttackMAP";

        private readonly UnitConquest ActiveUnit;
        private int ActivePlayerIndex;
        private readonly DelayedAttack ActiveDelayedAttack;

        public ActionPanelInitDelayedAttackMAP(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelInitDelayedAttackMAP(ConquestMap Map, int ActivePlayerIndex, DelayedAttack ActiveDelayedAttack)
            : base(PanelName, Map)
        {
            ActiveUnit = ActiveDelayedAttack.Owner;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveDelayedAttack = ActiveDelayedAttack;
        }

        public override void OnSelect()
        {
            if (ActiveDelayedAttack.ActiveAttack.ExplosionOption.ExplosionRadius > 0)
            {
                Map.AttackWithExplosion(ActivePlayerIndex, ActiveDelayedAttack.Owner, ActiveDelayedAttack.ActiveAttack, ActiveDelayedAttack.ListAttackPosition[0].WorldPosition);
            }

            ActionPanelUseMAPAttack.AttackWithMAPAttack(Map, ActivePlayerIndex, Map.ListPlayer[ActivePlayerIndex].ListUnit.IndexOf(ActiveUnit), ActiveDelayedAttack.ActiveAttack,
                new System.Collections.Generic.List<Vector3>(),
                Map.GetEnemies(ActiveUnit.CurrentAttack.MAPAttributes.FriendlyFire, ActiveDelayedAttack.ListAttackPosition));

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
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
