using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class ActionPanelPlayerHumanStep : ActionPanelWorldMap
    {
        public ActionPanelPlayerHumanStep(WorldMap Map)
            : base("PlayerHumanStep", Map, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.MoveCursor();

            if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
            else if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                int ConstructionIndex = Map.CheckForConstructionAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);
                if (ConstructionIndex >= 0)
                {
                    AddToPanelListAndSelect(new ActionPanelSelectConstruction(Map, Map.ListPlayer[Map.ActivePlayerIndex].ListConstruction[ConstructionIndex]));
                }
                else
                {
                    int SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);
                    if (SquadIndex >= 0)
                    {
                    }
                    else
                    {
                        AddToPanelListAndSelect(new ActionPanelPlaceNewConstruction(Map));
                    }
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerHumanStep(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
