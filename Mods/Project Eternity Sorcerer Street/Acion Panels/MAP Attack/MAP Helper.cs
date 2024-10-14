using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAttackMAP : ActionPanelSorcererStreet
    {
        private const string PanelName = "AttackMAP";

        private readonly int ActivePlayerIndex;
        private readonly int ActiveSquadIndex;
        public MAPAttackAttributes MAPAttributes;
        private readonly List<Vector3> ListMVHoverPoints;

        public ActionPanelAttackMAP(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAP(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;
        }

        public override void OnSelect()
        {
            if (MAPAttributes.Property == WeaponMAPProperties.Spread)
            {
                AddToPanelListAndSelect(new ActionPanelAttackMAPSpread(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
            }
            else if (MAPAttributes.Property == WeaponMAPProperties.Direction)
            {
                AddToPanelListAndSelect(new ActionPanelAttackMAPDirection(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
            }
            else if (MAPAttributes.Property == WeaponMAPProperties.Targeted)
            {
                AddToPanelListAndSelect(new ActionPanelAttackMAPTargeted(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
            }

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
            return new ActionPanelAttackMAP(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
