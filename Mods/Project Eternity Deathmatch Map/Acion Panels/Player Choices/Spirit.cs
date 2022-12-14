using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelSpirit : ActionPanelDeathmatch
    {
        private const string PanelName = "Spirit";

        private Squad ActiveSquad;

        public ActionPanelSpirit(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelSpirit(DeathmatchMap Map, Squad ActiveSquad)
            : base(PanelName, Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            Map.SpiritMenu.InitSpiritScreen(ActiveSquad);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            //Player has exited the menu.
            if (!Map.SpiritMenu.Alive)
            {
                Map.CursorPosition = ActiveSquad.Position;
                CancelPanel();
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpirit(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
