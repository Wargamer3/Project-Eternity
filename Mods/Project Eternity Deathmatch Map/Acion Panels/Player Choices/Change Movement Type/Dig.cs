using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDig : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelDig(DeathmatchMap Map, Squad ActiveSquad)
            : base("Dig", Map)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
