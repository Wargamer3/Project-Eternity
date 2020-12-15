using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart1 : ActionPanelDeathmatch
    {
        private readonly Vector3 LastPosition;
        private readonly Vector3 LastCameraPosition;
        private readonly Squad ActiveSquad;
        private readonly int ActivePlayerIndex;
        private readonly bool IsPostAttack;

        private List<Vector3> ListMVChoice;

        public ActionPanelMovePart1(DeathmatchMap Map, Vector3 LastPosition, Vector3 LastCameraPosition, Squad ActiveSquad, int ActivePlayerIndex, bool IsPostAttack = false)
            : base("Move", Map, !IsPostAttack)
        {
            this.LastPosition = LastPosition;
            this.LastCameraPosition = LastCameraPosition;
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.IsPostAttack = IsPostAttack;
        }

        public override void OnSelect()
        {
            ActiveSquad.SetPosition(LastPosition);
            Map.CursorPosition = LastPosition;
            Map.CursorPositionVisible = Map.CursorPosition;

            Map.CameraPosition = LastCameraPosition;

            //Get the possible moves.
            ListMVChoice = Map.GetMVChoice(ActiveSquad);
            ActiveSquad.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.ListLayer[Map.ActiveLayerIndex].LayerGrid.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.CursorControl();//Move the cursor
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (MouseHelper.InputLeftButtonReleased())
                {
                    if (MouseHelper.MouseStateCurrent.X < 0 || MouseHelper.MouseStateCurrent.X > Constants.Width ||
                        MouseHelper.MouseStateCurrent.Y < 0 || MouseHelper.MouseStateCurrent.Y > Constants.Height)
                        return;
                }
                if (ListMVChoice.Contains(Map.CursorPosition))
                {
                    ListNextChoice.Clear();

                    AddToPanelListAndSelect(new ActionPanelMovePart2(Map, ActiveSquad, ActivePlayerIndex, IsPostAttack));

                    Map.sndConfirm.Play();
                }
            }
        }

        protected override void OnCancelPanel()
        {
            Map.CursorPosition = LastPosition;
            Map.CursorPositionVisible = Map.CursorPosition;

            Map.CameraPosition = LastCameraPosition;
            ListMVChoice.Clear();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
