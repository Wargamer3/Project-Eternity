using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAP : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private readonly int ActivePlayerIndex;
        private readonly Attack CurrentAttack;

        public ActionPanelAttackMAP(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex)
            : base("AttackMAP", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            CurrentAttack = ActiveSquad.CurrentLeader.CurrentAttack;
        }

        public override void OnSelect()
        {
            if (CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
            {
                if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Spread)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPSpread(Map, ActiveSquad, ActivePlayerIndex));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Direction)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPDirection(Map, ActiveSquad, ActivePlayerIndex));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Targeted)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPTargeted(Map, ActiveSquad));
                }
                RemoveFromPanelList(this);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
