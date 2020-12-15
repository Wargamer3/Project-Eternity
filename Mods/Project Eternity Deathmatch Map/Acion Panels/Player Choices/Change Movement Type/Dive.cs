using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDive : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelDive(DeathmatchMap Map, Squad ActiveSquad)
            : base("Dive", Map)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
