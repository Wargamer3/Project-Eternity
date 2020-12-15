using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelDeploy : ActionPanelDeathmatch
    {
        UnitMapComponent TransportUnit;

        public ActionPanelDeploy(DeathmatchMap Map, UnitMapComponent TransportUnit)
            : base("Deploy", Map)
        {
            this.TransportUnit = TransportUnit;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
