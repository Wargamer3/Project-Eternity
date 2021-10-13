using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDive : ActionPanelDeathmatch
    {
        private const string PanelName = "Dive";

        private Squad ActiveSquad;

        public ActionPanelDive(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelDive(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentMovement = "Sea";
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
            return new ActionPanelDive(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
