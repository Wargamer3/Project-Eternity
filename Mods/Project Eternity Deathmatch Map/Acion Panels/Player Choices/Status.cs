using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelStatus : ActionPanelDeathmatch
    {
        Squad ActiveSquad;

        public ActionPanelStatus(DeathmatchMap Map, Squad ActiveSquad)
            : base("Status", Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            Map.StatusMenu.OpenStatusMenuScreen(ActiveSquad);

            Map.ListActionMenuChoice.Remove(this);
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
