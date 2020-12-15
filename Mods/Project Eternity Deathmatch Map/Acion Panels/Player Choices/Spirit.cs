using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelSpirit : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelSpirit(DeathmatchMap Map, Squad ActiveSquad)
            : base("Spirit", Map)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
