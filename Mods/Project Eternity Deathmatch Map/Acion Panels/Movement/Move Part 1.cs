using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart1 : ActionPanelDeathmatch
    {
        private const string PanelName = "Move";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Vector3 LastPosition;
        private Vector3 LastCameraPosition;
        private Squad ActiveSquad;
        private bool IsPostAttack;

        private List<Vector3> ListMVChoice;

        public ActionPanelMovePart1(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovePart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, Vector3 LastPosition, Vector3 LastCameraPosition, bool IsPostAttack = false)
            : base(PanelName, Map, !IsPostAttack)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.LastPosition = LastPosition;
            this.LastCameraPosition = LastCameraPosition;
            this.IsPostAttack = IsPostAttack;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
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
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListMVChoice.Contains(Map.CursorPosition))
                {
                    ListNextChoice.Clear();

                    AddToPanelListAndSelect(new ActionPanelMovePart2(Map, ActivePlayerIndex, ActiveSquadIndex, IsPostAttack));

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

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            LastPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), 0);
            LastCameraPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), 0);
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendFloat(LastPosition.X);
            BW.AppendFloat(LastPosition.Y);
            BW.AppendFloat(LastCameraPosition.X);
            BW.AppendFloat(LastCameraPosition.Y);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMovePart1(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
