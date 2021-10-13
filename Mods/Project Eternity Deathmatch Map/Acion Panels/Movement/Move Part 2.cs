using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart2 : ActionPanelDeathmatch
    {
        private const string PanelName = "Move2";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private bool IsPostAttack;

        public ActionPanelMovePart2(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovePart2(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, bool IsPostAttack)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.IsPostAttack = IsPostAttack;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
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
                    ListNextChoice.Add(new ActionPanelAttackPart1(Map, ActivePlayerIndex, ActiveSquadIndex, false));
                }

                if (ActiveSquad.CurrentLeader.Boosts.PostMovementModifier.Spirit)
                {
                    ListNextChoice.Add(new ActionPanelSpirit(Map, ActiveSquad));
                }

                ActiveSquad.CurrentLeader.OnMenuMovement(ActivePlayerIndex, ActiveSquad, Map.ListActionMenuChoice);

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

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            Map.CursorPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

            //Movement initialisation.
            Map.MovementAnimation.Add(ActiveSquad.X, ActiveSquad.Y, ActiveSquad);

            //Move the Unit to the cursor position
            ActiveSquad.SetPosition(Map.CursorPosition);

            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);
            BW.AppendFloat(Map.CursorPosition.Z);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMovePart2(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
