using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

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
        private List<Vector3> ListMVHoverChoice;

        public ActionPanelMovePart1(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListMVHoverChoice = new List<Vector3>();
        }

        public ActionPanelMovePart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, Vector3 LastPosition, Vector3 LastCameraPosition, bool IsPostAttack = false)
            : base(PanelName, Map, !IsPostAttack)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.LastPosition = LastPosition;
            this.LastCameraPosition = LastCameraPosition;
            this.IsPostAttack = IsPostAttack;

            ListMVHoverChoice = new List<Vector3>();
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
            Map.ListLayer[(int)ActiveSquad.Position.Z].LayerGrid.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.CursorControl();//Move the cursor
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (CheckIfUnitCanMove())
                {
                    ListNextChoice.Clear();

                    AddToPanelListAndSelect(new ActionPanelMovePart2(Map, ActivePlayerIndex, ActiveSquadIndex, IsPostAttack));

                    Map.sndConfirm.Play();
                }
            }
        }

        private void AddHoverChoice()
        {
            if (ListMVHoverChoice.Count == 0 && LastPosition != Map.CursorPosition)
            {
                float DiffX = Math.Abs(LastPosition.X - Map.CursorPosition.X);
                float DiffY = Math.Abs(LastPosition.Y - Map.CursorPosition.Y);
                float CurrentZ = LastPosition.Z;

                for (int X = 0; X <= DiffX; ++X)
                {
                    for (int Y = 0; Y <= DiffX; ++Y)
                    {
                        CurrentZ = Map.GetTerrain(0, 0, 0).Position.Z;
                        ListMVHoverChoice.Add(GetMVChoice(X, Y));
                    }
                }
            }
            else
            {

            }
        }

        private Vector3 GetMVChoice(float X, float Y)
        {
            string TerrainType = Map.GetTerrainType(X, Y, 0);
            foreach (Vector3 ActiveMVChoice in ListMVChoice)
            {
                if (ActiveMVChoice.X == X && ActiveMVChoice.Y == Y)
                {
                    return ActiveMVChoice;
                }
            }

            return Vector3.Zero;
        }

        private bool CheckIfUnitCanMove()
        {
            if (ListMVChoice.Contains(Map.CursorPosition))
            {
                for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < ActiveSquad.ArrayMapSize.GetLength(0); ++CurrentSquadOffsetX)
                {
                    for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < ActiveSquad.ArrayMapSize.GetLength(1); ++CurrentSquadOffsetY)
                    {
                        float RealX = Map.CursorPosition.X + CurrentSquadOffsetX;
                        float RealY = Map.CursorPosition.Y + CurrentSquadOffsetY;

                        if (!ListMVChoice.Contains(new Vector3((int)RealX, (int)RealY, (int)ActiveSquad.Position.Z)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
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
