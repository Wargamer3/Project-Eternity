using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart2 : ActionPanelDeathmatch
    {
        private readonly Squad ActiveSquad;
        private readonly int ActivePlayerIndex;
        private readonly bool IsPostAttack;

        public ActionPanelMovePart2(DeathmatchMap Map, Squad ActiveSquad, int ActivePlayerIndex, bool IsPostAttack)
            : base("Move2", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.IsPostAttack = IsPostAttack;
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            if (IsPostAttack)
            {
                ListNextChoice.Add(new ActionPanelWait(Map, ActiveSquad));
            }
            else
            {
                Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, Map.CursorPosition, Map.ListPlayer[Map.ActivePlayerIndex].Team, false);

                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    ListNextChoice.Add(new ActionPanelAttackPart1(false,
                        ActiveSquad, ActivePlayerIndex, Map));
                }

                if (ActiveSquad.CurrentLeader.Boosts.PostMovementModifier.Spirit)
                {
                    ListNextChoice.Add(new ActionPanelSpirit(Map, ActiveSquad));
                }

                ActiveSquad.CurrentLeader.OnMenuMovement(ActiveSquad, Map.ListActionMenuChoice);

                int SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);
                if (SquadIndex >= 0 && Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex] != ActiveSquad)
                {
                    ListNextChoice.Add(new ActionPanelBoard(Map, Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex], ActiveSquad));
                }
                else
                {
                    ListNextChoice.Add(new ActionPanelWait(Map, ActiveSquad));
                }

                new ActionPanelRepair(Map, this, ActiveSquad).OnSelect();
            }

            //Movement initialisation.
            Map.MovementAnimation.Add(ActiveSquad.X, ActiveSquad.Y, ActiveSquad);

            //Move the Unit to the cursor position
            ActiveSquad.SetPosition(Map.CursorPosition);

            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {//Make sure the mouse is inside the menu.
                if (MouseHelper.InputLeftButtonReleased())
                {
                }
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);

                Map.sndConfirm.Play();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
            }
            if (InputHelper.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;

                Map.sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed() && ActiveSquad.CurrentLeader.CanAttack)
            {
                ActionMenuCursor += (ActionMenuCursor < ListNextChoice.Count - 1) ? 1 : 0;

                Map.sndSelection.Play();
            }
            else if (MouseHelper.MouseMoved())
            {
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
