using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelStatus : ActionPanelDeathmatch
    {
        private const string PanelName = "Status";

        private Squad ActiveSquad;

        public ActionPanelStatus(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelStatus(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            Map.StatusMenu.OpenStatusMenuScreen(Map.ListPlayer[Map.ActivePlayerIndex], ActiveSquad);

            Map.ListActionMenuChoice.Remove(this);
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
            return new ActionPanelStatus(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
