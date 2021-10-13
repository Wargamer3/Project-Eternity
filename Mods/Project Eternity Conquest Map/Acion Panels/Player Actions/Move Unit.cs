using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMoveUnit : ActionPanelConquest
    {
        private const string PanelName = "PlayerMoveUnit";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private UnitConquest ActiveUnit;
        private List<Vector3> ListMVChoice;

        public ActionPanelMoveUnit(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelMoveUnit(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
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
                AddToPanelListAndSelect(new ActionPanelPlayerUnitSelected(Map, ActivePlayerIndex, ActiveUnitIndex));
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            Map.CursorPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];

            ListMVChoice = Map.GetMVChoice(ActiveUnit);
            Map.LastPosition = Map.CursorPosition;
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);
            BW.AppendFloat(Map.CursorPosition.Z);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMoveUnit(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
