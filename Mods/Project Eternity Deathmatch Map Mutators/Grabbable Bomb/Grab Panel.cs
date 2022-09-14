using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackGrab : ActionPanelDeathmatch
    {
        private const string PanelName = "Grab";

        private Squad ActiveSquad;

        public ActionPanelAttackGrab(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackGrab(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelThrowBomb(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
