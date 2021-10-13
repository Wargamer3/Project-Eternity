using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public class ActionPanelLand : ActionPanelDeathmatch
    {
        private const string PanelName = "Land";

        private Squad ActiveSquad;

        public ActionPanelLand(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelLand(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentMovement = "Land";
            ActiveSquad.IsFlying = false;
            RemoveFromPanelList(this);
            ReplaceChoiceInCurrentPanel(new ActionPanelFly(Map, ActiveSquad), typeof(ActionPanelLand));
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
            return new ActionPanelLand(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
