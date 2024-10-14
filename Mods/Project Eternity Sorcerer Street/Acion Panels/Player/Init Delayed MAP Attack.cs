using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelInitDelayedAttackMAP : ActionPanelSorcererStreet
    {
        private const string PanelName = "InitAttackMAP";

        private readonly SorcererStreetUnit ActiveUnit;
        private int ActivePlayerIndex;
        private readonly DelayedAttack ActiveDelayedAttack;

        public ActionPanelInitDelayedAttackMAP(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelInitDelayedAttackMAP(SorcererStreetMap Map, int ActivePlayerIndex, DelayedAttack ActiveDelayedAttack)
            : base(PanelName, Map)
        {
            ActiveUnit = ActiveDelayedAttack.Owner;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveDelayedAttack = ActiveDelayedAttack;
        }

        public override void OnSelect()
        {
            if (ActiveDelayedAttack.ExplosionOption.ExplosionRadius > 0)
            {
                Map.AttackWithExplosion(ActivePlayerIndex, ActiveDelayedAttack.Owner, ActiveDelayedAttack.ExplosionOption, ActiveDelayedAttack.ListAttackPosition[0].WorldPosition);
            }

            Map.AttackDirectly(ActivePlayerIndex, new System.Collections.Generic.List<Vector3>(),
                Map.GetEnemies(ActiveDelayedAttack.MAPAttributes.FriendlyFire, ActiveDelayedAttack.ListAttackPosition));

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
