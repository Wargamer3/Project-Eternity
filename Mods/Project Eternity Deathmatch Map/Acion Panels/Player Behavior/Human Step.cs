using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelPlayerHumanStep : ActionPanelDeathmatch
    {
        BattlePreviewer BattlePreview;

        public ActionPanelPlayerHumanStep(DeathmatchMap Map)
            : base("PlayerHumanStep", Map, false)
        {
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            bool CursorMoved = Map.UpdateMapNavigation();
            if (CursorMoved)
            {
                BattlePreview = null;
            }
            //Loop through the players to find a Unit to control.
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                if (CursorSelect >= 0)
                {
                    if (BattlePreview == null)
                    {
                        BattlePreview = new BattlePreviewer(Map, Map.ListPlayer[P].ListSquad[CursorSelect], null);
                    }
                    BattlePreview.UpdateUnitDisplay();
                }
            }

            if (InputHelper.InputCancelPressed())
            {
                AddToPanelListAndSelect(Map.BattleMapMenu);

                Map.ActiveSquadIndex = -1;
                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (MouseHelper.InputLeftButtonReleased())
                {
                    if (MouseHelper.MouseStateCurrent.X < 0 || MouseHelper.MouseStateCurrent.X > Constants.Width ||
                        MouseHelper.MouseStateCurrent.Y < 0 || MouseHelper.MouseStateCurrent.Y > Constants.Height)
                        return;
                }

                Map.ActiveSquadIndex = -1;
                Map.TargetSquadIndex = -1;
                bool UnitFound = false;
                //Loop through the players to find a Unit to control.
                for (int P = 0; P < Map.ListPlayer.Count && (Map.ActiveSquadIndex < 0 && Map.TargetSquadIndex < 0); P++)
                {
                    //Find if a current player Unit is under the cursor.
                    int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                    #region Unit found

                    if (CursorSelect >= 0)
                    {
                        UnitFound = true;
                        ActionPanelMainMenu NewActionMenu = new ActionPanelMainMenu(Map, Map.ListPlayer[P].ListSquad[CursorSelect], P);
                        AddToPanelList(NewActionMenu);

                        if (P == Map.ActivePlayerIndex && Map.ListPlayer[P].ListSquad[CursorSelect].IsPlayerControlled)//Player controlled Squad.
                        {
                            NewActionMenu.OnSelect();
                        }
                        else//Enemy.
                        {
                            NewActionMenu.AddChoiceToCurrentPanel(new ActionPanelStatus(Map, Map.ListPlayer[P].ListSquad[CursorSelect]));
                        }
                    }

                    #endregion
                }

                //You select the tile under the cursor.
                if (!UnitFound)
                {
                    AddToPanelListAndSelect(new ActionPanelTileStatus(Map));
                }

                Map.sndConfirm.Play();
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //If the action menu is open.
            if (BattlePreview != null)
            {
                BattlePreview.DrawDisplayUnit(g);
            }
        }
    }
}
