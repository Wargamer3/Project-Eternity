using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAttackMAP : ActionPanelConquest
    {
        private const string PanelName = "AttackMAP";

        private readonly int ActivePlayerIndex;
        private readonly int ActiveUnitIndex;
        private readonly List<Vector3> ListMVHoverPoints;
        private readonly Attack CurrentAttack;

        public ActionPanelAttackMAP(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAP(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            CurrentAttack = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex].CurrentAttack;
        }

        public override void OnSelect()
        {
            if (CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
            {
                if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Spread)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPSpread(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Direction)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPDirection(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Targeted)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPTargeted(Map, ActivePlayerIndex, ActiveUnitIndex, ListMVHoverPoints));
                }

                RemoveFromPanelList(this);
            }
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
            return new ActionPanelAttackMAP(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
