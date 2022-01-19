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
        private Vector3 LastCusorMVPosition;
        private Squad ActiveSquad;
        private bool IsPostAttack;

        private List<MovementAlgorithmTile> ListMVChoice;
        private List<Vector3> ListMVPoints;
        private List<MovementAlgorithmTile> ListMovedOverTerrain;
        private List<Vector3> ListMovedOverPoint;

        public ActionPanelMovePart1(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
        }

        public ActionPanelMovePart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, Vector3 LastPosition, Vector3 LastCameraPosition, bool IsPostAttack = false)
            : base(PanelName, Map, !IsPostAttack)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.LastPosition = LastPosition;
            this.LastCameraPosition = LastCameraPosition;
            this.IsPostAttack = IsPostAttack;

            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ActiveSquad.SetPosition(LastPosition);
            Map.CursorPosition = LastPosition;
            Map.CursorPositionVisible = Map.CursorPosition;
            LastCusorMVPosition = LastPosition;

            Map.CameraPosition = LastCameraPosition;

            ListMVChoice = Map.GetMVChoice(ActiveSquad);
            ListMVPoints = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTerrain in ListMVChoice)
            {
                ListMVPoints.Add(new Vector3(ActiveTerrain.Position.X, ActiveTerrain.Position.Y, ActiveTerrain.LayerIndex));
            }
            ListMovedOverTerrain.Add(Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z));
            ListMovedOverPoint.Add(ActiveSquad.Position);
            ActiveSquad.CurrentLeader.CurrentAttack = null;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.LayerManager.AddDrawablePath(ListMovedOverTerrain);

            Map.CursorControl(ActiveInputManager);//Move the cursor

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (CheckIfUnitCanMove())
                {
                    ListNextChoice.Clear();

                    AddToPanelListAndSelect(new ActionPanelMovePart2(Map, ActivePlayerIndex, ActiveSquadIndex, IsPostAttack, ListMovedOverPoint));

                    Map.sndConfirm.Play();
                }
            }
            else if (Map.CursorPosition == ActiveSquad.Position)
            {
                ListMovedOverTerrain.Clear();
                ListMovedOverPoint.Clear();
                ListMovedOverTerrain.Add(Map.GetTerrain(ActiveSquad.X, ActiveSquad.Y, (int)ActiveSquad.Z));
                ListMovedOverPoint.Add(ActiveSquad.Position);
                LastCusorMVPosition = Map.CursorPosition;
            }
            else if (ListMVPoints.Contains(Map.CursorPosition) && Map.CursorPosition != LastCusorMVPosition)
            {
                if (Math.Abs(LastCusorMVPosition.X - Map.CursorPosition.X) + Math.Abs(LastCusorMVPosition.Y - Map.CursorPosition.Y) > 1)
                {
                    ComputeNewHoverPath();
                }
                else if (ListMovedOverPoint.Contains(Map.CursorPosition))
                {
                    RemoveHoverChoice();
                }
                else
                {
                    AddHoverChoice();
                }

                LastCusorMVPosition = Map.CursorPosition;
            }
        }

        private void AddHoverChoice()
        {
            int MaxMV = Map.GetSquadMaxMovement(ActiveSquad);//Maximum distance you can reach.

            MaxMV += ActiveSquad.CurrentLeader.Boosts.MovementModifier;

            MovementAlgorithmTile LastTerrin = Map.GetTerrain(ActiveSquad);
            float CurrentMVCost = 0;
            for (int T = 1; T < ListMovedOverTerrain.Count; T++)
            {
                MovementAlgorithmTile ActiveTerrain = ListMovedOverTerrain[T];
                CurrentMVCost += Map.Pathfinder.GetMVCost(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, LastTerrin, ActiveTerrain);
            }

            MovementAlgorithmTile NextTerrain = Map.GetTerrain(Map.CursorPosition.X, Map.CursorPosition.Y, (int)Map.CursorPosition.Z);
            CurrentMVCost += Map.Pathfinder.GetMVCost(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, LastTerrin, NextTerrain);

            if (CurrentMVCost <= MaxMV)
            {
                ListMovedOverTerrain.Add(NextTerrain);
                ListMovedOverPoint.Add(Map.CursorPosition);
            }
            else
            {
                ComputeNewHoverPath();
            }
        }

        private void RemoveHoverChoice()
        {
            bool RemovePoint = false;

            for (int P = 0; P < ListMovedOverPoint.Count; ++P)
            {
                if (RemovePoint)
                {
                    ListMovedOverPoint.RemoveAt(P);
                    ListMovedOverTerrain.RemoveAt(P);
                    --P;
                }
                else if (Map.CursorPosition == ListMovedOverPoint[P])
                {
                    RemovePoint = true;
                }
            }
        }

        private void ComputeNewHoverPath()
        {
            ListMovedOverTerrain.Clear();
            ListMovedOverPoint.Clear();

            MovementAlgorithmTile CurrentTerrain = Map.GetTerrain(Map.CursorPosition.X, Map.CursorPosition.Y, (int)Map.CursorPosition.Z);

            do
            {
                if (!ListMovedOverTerrain.Contains(CurrentTerrain))
                {
                    ListMovedOverTerrain.Add(CurrentTerrain);
                    ListMovedOverPoint.Add(new Vector3(CurrentTerrain.Position.X, CurrentTerrain.Position.Y, CurrentTerrain.LayerIndex));
                    if (CurrentTerrain.Position == ActiveSquad.Position)
                    {
                        CurrentTerrain = null;
                    }
                    else
                    {
                        CurrentTerrain = CurrentTerrain.ParentReal;
                    }
                }
                else
                {
                    CurrentTerrain = null;
                }
            }
            while (CurrentTerrain != null);

            ListMovedOverTerrain.Reverse();
            ListMovedOverPoint.Reverse();
        }

        private bool CheckIfUnitCanMove()
        {
            if (ListMVPoints.Contains(Map.CursorPosition))
            {
                for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < ActiveSquad.ArrayMapSize.GetLength(0); ++CurrentSquadOffsetX)
                {
                    for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < ActiveSquad.ArrayMapSize.GetLength(1); ++CurrentSquadOffsetY)
                    {
                        float RealX = Map.CursorPosition.X + CurrentSquadOffsetX;
                        float RealY = Map.CursorPosition.Y + CurrentSquadOffsetY;

                        if (!ListMVPoints.Contains(new Vector3((int)RealX, (int)RealY, (int)Map.CursorPosition.Z)))
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
            LastPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadUInt32());
            LastCameraPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendFloat(LastPosition.X);
            BW.AppendFloat(LastPosition.Y);
            BW.AppendInt32((int)LastPosition.Z);
            BW.AppendFloat(LastCameraPosition.X);
            BW.AppendFloat(LastCameraPosition.Y);
            BW.AppendFloat(LastCameraPosition.Z);
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
