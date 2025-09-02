using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAIDefault : ActionPanelConquest
    {
        private const string PanelName = "AI Default";

        public ActionPanelAIDefault(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIDefault(Map);
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

                int BuildingIndex = Map.CheckForBuildingPosition(ActiveUnit.Components.Position);
                //Capture building on self
                if (Map.ListBuilding[BuildingIndex].CapturedTeamIndex != Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex)
                {
                    AddToPanelListAndSelect(new ActionPanelCapture(Map, Map.ActivePlayerIndex, U, BuildingIndex));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelAIAttackBehavior(Map, Map.ActivePlayerIndex, U));
                }
            }

            if (UnitsNotUpdatedCount == 0)
            {
                RemoveAllActionPanels();
                ActionPanelPhaseChange EndPhase = new ActionPanelPhaseChange(Map);
                EndPhase.ActiveSelect = true;
                ListActionMenuChoice.AddToPanelListAndSelect(EndPhase);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
