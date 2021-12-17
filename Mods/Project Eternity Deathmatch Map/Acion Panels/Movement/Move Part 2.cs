using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart2 : ActionPanelDeathmatch
    {
        private const string PanelName = "Move2";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private bool IsPostAttack;
        private Vector3 CursorPosition;

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

            CursorPosition = Map.CursorPosition;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            if (IsPostAttack)
            {
                AddChoiceToCurrentPanel(new ActionPanelWait(Map, ActiveSquad));
            }
            else
            {
                Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, Map.CursorPosition, Map.ListPlayer[Map.ActivePlayerIndex].Team, false);

                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, ActivePlayerIndex, ActiveSquadIndex, false));
                }

                if (ActiveSquad.CurrentLeader.Boosts.PostMovementModifier.Spirit)
                {
                    AddChoiceToCurrentPanel(new ActionPanelSpirit(Map, ActiveSquad));
                }

                ActiveSquad.CurrentLeader.OnMenuMovement(ActivePlayerIndex, ActiveSquad, Map.ListActionMenuChoice);

                int SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);
                if (SquadIndex >= 0 && Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex] != ActiveSquad)
                {
                    AddChoiceToCurrentPanel(new ActionPanelBoard(Map, Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex], ActiveSquad));
                }
                else
                {
                    AddChoiceToCurrentPanel(new ActionPanelWait(Map, ActiveSquad));
                }

                foreach (ActionPanel OptionalPanel in GetPropPanelsOnUnitStop(ActiveSquad))
                {
                    AddChoiceToCurrentPanel(OptionalPanel);
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

        private List<ActionPanel> GetPropPanelsOnUnitStop(Squad StoppedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            foreach (InteractiveProp ActiveProp in Map.ListLayer[ActiveSquad.LayerIndex].ListProp)
            {
                ListPanel.AddRange(ActiveProp.OnUnitBeforeStop(StoppedUnit, CursorPosition));
            }
            return ListPanel;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);

            if (ActiveInputManager.InputConfirmPressed())
            {//Make sure the mouse is inside the menu.
                AddToPanelListAndSelect(ListNextChoice[ActionMenuCursor]);

                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
            }
            if (ActiveInputManager.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed() && ActiveSquad.CurrentLeader.CanAttack)
            {
                ActionMenuCursor += (ActionMenuCursor < ListNextChoice.Count - 1) ? 1 : 0;

                Map.sndSelection.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            CursorPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
            Map.CursorPosition = CursorPosition;

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
            BW.AppendFloat(CursorPosition.X);
            BW.AppendFloat(CursorPosition.Y);
            BW.AppendFloat(CursorPosition.Z);
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
