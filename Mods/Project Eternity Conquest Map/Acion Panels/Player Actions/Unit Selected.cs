using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerUnitSelected : ActionPanelConquest
    {
        private const string PanelName = "PlayerUnitSelected";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private UnitConquest ActiveUnit;

        public ActionPanelPlayerUnitSelected(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelPlayerUnitSelected(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public override void OnSelect()
        {
            AddChoiceToCurrentPanel(new ActionPanelWait(Map, ActiveUnit));
            List<Tuple<int, int>> ListSquadFound = Map.CanSquadAttackWeapon1((int)ActiveUnit.X, (int)ActiveUnit.Y, Map.ActivePlayerIndex, ActiveUnit.ArmourType, ActiveUnit.ListAttack[0]);
            if (ListSquadFound != null && ListSquadFound.Count > 0)
            {
                AddChoiceToCurrentPanel(new ActionPanelAttack(Map, ActiveUnit, ListSquadFound, 1));
            }
            else if (ListSquadFound == null || ListSquadFound.Count == 0)
            {
                ListSquadFound = Map.CanSquadAttackWeapon2((int)ActiveUnit.X, (int)ActiveUnit.Y, Map.ActivePlayerIndex, ActiveUnit.ArmourType, ActiveUnit.ListAttack[1]);
                if (ListSquadFound != null && ListSquadFound.Count > 0)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttack(Map, ActiveUnit, ListSquadFound, 2));
                }
            }

            int BuildingIndex = Map.CheckForBuildingPosition(Map.CursorPosition);
            if (BuildingIndex >= 0)
            {
                if (Map.ListBuilding[BuildingIndex].CanBeCaptured && Map.ListBuilding[BuildingIndex].CapturedTeamIndex != Map.ListAllPlayer[ActivePlayerIndex].TeamIndex)
                {
                    AddChoiceToCurrentPanel(new ActionPanelCapture(Map, ActivePlayerIndex, ActiveUnitIndex, BuildingIndex));
                }
                else
                {
                }
            }
        }
        
        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(Map.sndSelection))
            {
            }
            else if (ConfirmNextChoices(Map.sndConfirm))
            {
            }
        }

        protected override void OnCancelPanel()
        {
            //Move the Unit to the cursor position
            ActiveUnit.SetPosition(Map.LastPosition);

            Map.sndCancel.Play();
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerUnitSelected(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
