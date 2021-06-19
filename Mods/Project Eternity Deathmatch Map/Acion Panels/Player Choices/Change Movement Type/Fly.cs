using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelFly : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;

        public ActionPanelFly(DeathmatchMap Map, Squad ActiveSquad)
            : base("Fly", Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
            ActiveSquad.CurrentMovement = UnitStats.TerrainAir;
            ActiveSquad.IsFlying = true;
            RemoveFromPanelList(this);
            ReplaceChoiceInCurrentPanel(new ActionPanelLand(Map, ActiveSquad), typeof(ActionPanelFly));
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
