using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelWait : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelWait(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Wait", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveUnit);
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
