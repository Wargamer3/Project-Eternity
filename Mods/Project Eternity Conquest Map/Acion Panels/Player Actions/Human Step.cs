using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerHumanStep : ActionPanelConquest
    {
        private int ActivePlayerIndex;

        public ActionPanelPlayerHumanStep(ConquestMap Map, int ActivePlayerIndex)
            : base("Player Default", Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorControl(ActiveInputManager);
            if (ActiveInputManager.InputConfirmPressed())
            {
                UnitConquest ActiveUnit = null;
                //Loop through the players to find a Unit to control.
                for (int P = 0; P < Map.ListPlayer.Count && ActiveUnit == null; P++)
                {
                    //Find if a Unit is under the cursor.
                    int CursorSelect = Map.CheckForUnitAtPosition(P, Map.CursorPosition, Vector3.Zero);

                    //If one was found.
                    if (CursorSelect >= 0)
                    {
                        ActiveUnit = Map.ListPlayer[P].ListUnit[CursorSelect];
                        if (P == ActivePlayerIndex)//Ally.
                        {
                            AddToPanelListAndSelect(new ActionPanelMoveUnit(Map, P, CursorSelect, Map.GetTerrainUnderCursor(), ActiveUnit.Components.Direction, Map.Camera2DPosition));
                        }
                        else//Enemy.
                        {
                            AddToPanelListAndSelect(new ActionPanelPlayerEnemyUnitSelected(Map));
                        }
                    }
                }
                //No unit, search for captured building.
                if (ActiveUnit == null)
                {
                    int BuildingIndex = Map.CheckForBuildingPosition(Map.CursorPosition);

                    if (BuildingIndex >= 0)
                    {
                        if (Map.ListBuilding[BuildingIndex].CanBeCaptured && Map.ListBuilding[BuildingIndex].CapturedTeamIndex == Map.ListAllPlayer[ActivePlayerIndex].TeamIndex)
                        {
                            AddToPanelListAndSelect(new ActionPanelPlayerBuildingUnitSelected(Map, ActivePlayerIndex, BuildingIndex));
                        }
                        else
                        {
                        }
                    }
                }

                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                AddToPanelListAndSelect(new ActionPanelPlayerMainMenu(Map));
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
            return new ActionPanelPlayerHumanStep(Map, ActivePlayerIndex);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
