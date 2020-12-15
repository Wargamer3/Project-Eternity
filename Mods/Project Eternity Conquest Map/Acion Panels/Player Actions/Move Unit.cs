using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMoveUnit : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;
        private List<Vector3> ListMVChoice;

        public ActionPanelMoveUnit(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Player Move Unit", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            ListMVChoice = Map.GetMVChoice(ActiveUnit);
            Map.LastPosition = Map.CursorPosition;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(ListMVChoice, Color.White);

            Map.CursorControl();

            if ((InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased()) &&
                ListMVChoice.Contains(Map.CursorPosition))//If the cursor is in the possible move list.
            {
                //Movement initialisation.
                Map.MovementAnimation.Add(ActiveUnit.X, ActiveUnit.Y, ActiveUnit.Components);
                //Move the Unit to the cursor position
                ActiveUnit.SetPosition(Map.CursorPosition);
                ListMVChoice.Clear();
                AddToPanelListAndSelect(new ActionPanelPlayerUnitSelected(Map, ActiveUnit));
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
