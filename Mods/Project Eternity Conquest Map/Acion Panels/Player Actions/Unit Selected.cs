using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerUnitSelected : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelPlayerUnitSelected(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Player Unit Selected", Map)
        {
            this.ActiveUnit = ActiveUnit;
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

            if (Map.GetTerrain(ActiveUnit.Components).TerrainTypeIndex >= 13)
            {
                if (Map.GetTerrain(ActiveUnit.Components).CapturedPlayerIndex != Map.ActivePlayerIndex)
                    AddChoiceToCurrentPanel(new ActionPanelCapture(Map, ActiveUnit));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            NavigateThroughNextChoices(Map.sndSelection, Map.sndConfirm);
        }

        protected override void OnCancelPanel()
        {
            //Move the Unit to the cursor position
            ActiveUnit.SetPosition(Map.LastPosition);

            Map.sndCancel.Play();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
