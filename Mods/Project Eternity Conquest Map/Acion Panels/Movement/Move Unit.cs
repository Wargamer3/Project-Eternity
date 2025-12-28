using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelMoveUnit : ActionPanelConquest
    {
        private const string PanelName = "PlayerMoveUnit";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private UnitConquest ActiveUnit;

        private bool HasFocus;

        private MovementAlgorithmTile LastPosition;
        private float LastDirection;
        private Vector3 LastCameraPosition;
        private MovementAlgorithmTile LastCusorMVPosition;

        private List<MovementAlgorithmTile> ListMVChoice;
        private List<MovementAlgorithmTile> ListMovedOverTerrain;
        private List<Vector3> ListMovedOverPoint;

        public ActionPanelMoveUnit(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelMoveUnit(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, MovementAlgorithmTile LastPosition, float LastDirection, Vector3 LastCameraPosition)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.LastPosition = LastPosition;
            this.LastDirection = LastDirection;
            this.LastCameraPosition = LastCameraPosition;

            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            HasFocus = true;
        }

        public override void OnSelect()
        {
            ActiveUnit.SetPosition(LastPosition.WorldPosition + new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0));
            ActiveUnit.Components.Direction = LastDirection;
            Map.CursorPosition = ActiveUnit.Position;
            Map.CursorPositionVisible = Map.CursorPosition;
            LastCusorMVPosition = LastPosition;

            Map.Camera2DPosition = LastCameraPosition;

            ListMVChoice = Map.GetMVChoice(ActiveUnit, (ConquestMap)LastPosition.Owner);
            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverTerrain.Add(Map.GetTerrain(ActiveUnit.Position));
            ListMovedOverPoint = new List<Vector3>();
            ListMovedOverPoint.Add(ActiveUnit.Position);
            ActiveUnit.CurrentAttack = null;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HasFocus = true;

            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.LayerManager.AddDrawablePath(ListMovedOverTerrain);

            if (Map.CursorControl(ActiveInputManager))
            {
                foreach (TeleportPoint ActiveTeleport in Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == Map.CursorPosition.X && ActiveTeleport.Position.Y == Map.CursorPosition.Y)
                    {
                        Map.CursorPosition.X = ActiveTeleport.OtherMapEntryPoint.X;
                        Map.CursorPosition.Y = ActiveTeleport.OtherMapEntryPoint.Y;
                        Map.CursorPosition.Z = ActiveTeleport.OtherMapEntryLayer;
                        break;
                    }
                }
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (CheckIfUnitCanMove())
                {
                    HasFocus = false;

                    //Movement initialisation.
                    Map.MovementAnimation.Add(ActiveUnit.Components, ActiveUnit.Components.Position, Map.CursorPosition);
                    //Move the Unit to the cursor position
                    ActiveUnit.SetPosition(Map.CursorPosition);
                    ListMVChoice.Clear();
                    AddToPanelListAndSelect(new ActionPanelPlayerUnitSelected(Map, ActivePlayerIndex, ActiveUnitIndex));
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else if (Map.CursorPosition == ActiveUnit.Position)
            {
                ListMovedOverTerrain.Clear();
                ListMovedOverPoint.Clear();
                ListMovedOverTerrain.Add(Map.GetTerrain(ActiveUnit.Position));
                ListMovedOverPoint.Add(ActiveUnit.Position);
                LastCusorMVPosition = LastPosition.Owner.CursorTerrain;
            }
            else if (ListMVChoice.Contains(LastPosition.Owner.CursorTerrain) && LastPosition.Owner.CursorTerrain != LastCusorMVPosition)
            {
                if (Math.Abs(LastCusorMVPosition.GridPosition.X - LastPosition.Owner.CursorPosition.X / LastPosition.Owner.TileSize.X) + Math.Abs(LastCusorMVPosition.GridPosition.Y - LastPosition.Owner.CursorPosition.Y / LastPosition.Owner.TileSize.Y) > 1)
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

                LastCusorMVPosition = LastPosition.Owner.CursorTerrain;
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListMVChoice, Color.FromNonPremultiplied(0, 128, 0, 190));
            Map.LayerManager.AddDrawablePath(ListMovedOverTerrain);
        }

        private void AddHoverChoice()
        {
            int MaxMV = Map.GetSquadMaxMovement(ActiveUnit);//Maximum distance you can reach.

            MovementAlgorithmTile LastTerrin = Map.GetTerrain(ActiveUnit.Position);
            float CurrentMVCost = 0;
            for (int T = 1; T < ListMovedOverTerrain.Count; T++)
            {
                MovementAlgorithmTile ActiveTerrain = ListMovedOverTerrain[T];
                CurrentMVCost += Map.Pathfinder.GetMVCost(ActiveUnit.Components, ActiveUnit.UnitStat, LastTerrin, ActiveTerrain);
            }

            MovementAlgorithmTile NextTerrain = Map.GetTerrain(Map.CursorPosition);
            CurrentMVCost += Map.Pathfinder.GetMVCost(ActiveUnit.Components, ActiveUnit.UnitStat, LastTerrin, NextTerrain);

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

            MovementAlgorithmTile CurrentTerrain = Map.GetTerrain(Map.CursorPosition);

            do
            {
                if (!ListMovedOverTerrain.Contains(CurrentTerrain))
                {
                    ListMovedOverTerrain.Add(CurrentTerrain);
                    ListMovedOverPoint.Add(new Vector3(CurrentTerrain.WorldPosition.X, CurrentTerrain.WorldPosition.Y, CurrentTerrain.LayerIndex));
                    if (CurrentTerrain.WorldPosition == ActiveUnit.Position)
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
            if (ListMVChoice.Contains(Map.CursorTerrain))
            {
                for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < ActiveUnit.Components.ArrayMapSize.GetLength(0); ++CurrentSquadOffsetX)
                {
                    for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < ActiveUnit.Components.ArrayMapSize.GetLength(1); ++CurrentSquadOffsetY)
                    {
                        if (!ListMVChoice.Contains(Map.GetTerrainUnderCursor()))
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
            Map.CursorPosition = LastPosition.WorldPosition + new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0);
            Map.CursorPositionVisible = Map.CursorPosition;

            Map.Camera2DPosition = LastCameraPosition;
            ListMVChoice.Clear();
        }

        public override void DoRead(ByteReader BR)
        {
            HasFocus = BR.ReadBoolean();
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            LastPosition = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            LastDirection = BR.ReadFloat();
            LastCameraPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];

            if (HasFocus)
            {
                OnSelect();
            }
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            ByteReader BR = new ByteReader(ArrayUpdateData);
            Map.CursorPosition.X = BR.ReadFloat();
            Map.CursorPosition.Y = BR.ReadFloat();
            BR.Clear();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendBoolean(HasFocus);
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
            BW.AppendFloat(LastPosition.WorldPosition.X);
            BW.AppendFloat(LastPosition.WorldPosition.Y);
            BW.AppendFloat(LastPosition.WorldPosition.Z);
            BW.AppendFloat(LastDirection);
            BW.AppendFloat(LastCameraPosition.X);
            BW.AppendFloat(LastCameraPosition.Y);
            BW.AppendFloat(LastCameraPosition.Z);
        }

        public override byte[] DoWriteUpdate()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendFloat(Map.CursorPosition.X);
            BW.AppendFloat(Map.CursorPosition.Y);

            byte[] ArrayUpdateData = BW.GetBytes();
            BW.ClearWriteBuffer();

            return ArrayUpdateData;
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
