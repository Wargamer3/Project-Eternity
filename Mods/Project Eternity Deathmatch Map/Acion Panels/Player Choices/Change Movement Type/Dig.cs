using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDig : ActionPanelDeathmatch
    {
        private const string PanelName = "Dig";

        private Squad ActiveSquad;

        public ActionPanelDig(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelDig(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentMovement = "Underground";
            ActiveSquad.IsFlying = false;
            RemoveFromPanelList(this);
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
            return new ActionPanelDig(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
