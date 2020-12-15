using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerEnemyUnitSelected : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelPlayerEnemyUnitSelected(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Player Enemy Unit Selected", Map)
        {
            this.ActiveUnit = ActiveUnit;
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
