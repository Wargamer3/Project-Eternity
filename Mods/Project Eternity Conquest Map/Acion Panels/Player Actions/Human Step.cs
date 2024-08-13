using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerHumanStep : ActionPanelConquest
    {
        public ActionPanelPlayerHumanStep(ConquestMap Map)
            : base("Player Default", Map)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.CursorControl();
            if (MouseHelper.InputLeftButtonReleased())
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
                        if (P == Map.ActivePlayerIndex)//Ally.
                        {
                            AddToPanelListAndSelect(new ActionPanelMoveUnit(Map, P, CursorSelect));
                        }
                        else//Enemy.
                        {
                            AddToPanelListAndSelect(new ActionPanelPlayerEnemyUnitSelected(Map));
                        }
                    }
                }
                //No unit, search for captured building.
                if (ActiveUnit == null && Map.CursorPosition.X >= 0 && Map.CursorPosition.X < Map.MapSize.X
                    && Map.CursorPosition.Y >= 0 && Map.CursorPosition.Y < Map.MapSize.Y)
                {
                    if (Map.GetTerrain((int)Map.CursorPosition.X, (int)Map.CursorPosition.Y, 0).TerrainTypeIndex >= 13 &&
                        Map.GetTerrain((int)Map.CursorPosition.X, (int)Map.CursorPosition.Y, 0).CapturedPlayerIndex == Map.ActivePlayerIndex)
                    {
                        AddToPanelListAndSelect(new ActionPanelPlayerBuildingUnitSelected(Map));
                    }
                }

                Map.sndConfirm.Play();
            }
        }

        protected override void OnCancelPanel()
        {
            AddToPanelListAndSelect(new ActionPanelPlayerMainMenu(Map));
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
