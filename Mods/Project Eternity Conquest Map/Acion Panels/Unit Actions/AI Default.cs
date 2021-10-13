using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
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

                //Capture building on self
                if (UnitTerrain.CapturedPlayerIndex != Map.ActivePlayerIndex)
                {
                    AddToPanelListAndSelect(new ActionPanelCapture(Map, Map.ActivePlayerIndex, U));
                }
                else
                {
                    AddToPanelListAndSelect(new ActionPanelAIAttack(Map, Map.ActivePlayerIndex, U));
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
