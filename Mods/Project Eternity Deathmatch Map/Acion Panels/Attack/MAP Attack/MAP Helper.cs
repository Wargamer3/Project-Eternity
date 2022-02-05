using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackMAP : ActionPanelDeathmatch
    {
        private const string PanelName = "AttackMAP";

        private readonly int ActivePlayerIndex;
        private readonly int ActiveSquadIndex;
        private readonly List<Vector3> ListMVHoverPoints;
        private readonly Attack CurrentAttack;

        public ActionPanelAttackMAP(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAP(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
            CurrentAttack = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex].CurrentLeader.CurrentAttack;
        }

        public override void OnSelect()
        {
            if (CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
            {
                if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Spread)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPSpread(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Direction)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPDirection(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                }
                else if (CurrentAttack.MAPAttributes.Property == WeaponMAPProperties.Targeted)
                {
                    AddToPanelListAndSelect(new ActionPanelAttackMAPTargeted(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
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
