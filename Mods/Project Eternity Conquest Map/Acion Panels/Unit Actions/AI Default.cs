using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAIDefault : ActionPanelConquest
    {
        public ActionPanelAIDefault(ConquestMap Map)
            : base("AI Default", Map)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            int UnitsNotUpdatedCount = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count;

            for (int U = 0; U < Map.ListPlayer[Map.ActivePlayerIndex].ListUnit.Count; U++)
            {
                UnitConquest ActiveUnit = Map.ListPlayer[Map.ActivePlayerIndex].ListUnit[U];
                TerrainConquest UnitTerrain = Map.GetTerrain(ActiveUnit.Components);

                if (!ActiveUnit.CanMove)
                {
                    --UnitsNotUpdatedCount;
                    continue;
                }

                //Capture building on self
                if (UnitTerrain.CapturedPlayerIndex != Map.ActivePlayerIndex)
                {
                    AddToPanelListAndSelect(new ActionPanelCapture(Map, ActiveUnit));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelAIAttack(Map, ActiveUnit));
                }
            }

            if (UnitsNotUpdatedCount == 0)
            {
                Map.OnNewPhase();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
