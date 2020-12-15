using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public class ActionPanelLand : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelLand(DeathmatchMap Map, Squad ActiveSquad)
            : base("Land", Map)
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
