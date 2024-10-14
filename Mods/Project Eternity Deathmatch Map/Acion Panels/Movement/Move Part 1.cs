using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart1 : ActionPanelDeathmatch
    {
        private const string PanelName = "Move";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private MovementAlgorithmTile LastPosition;
        private float LastDirection;
        private Vector3 LastCameraPosition;
        private MovementAlgorithmTile LastCusorMVPosition;
        private Squad ActiveSquad;
        private bool IsPostAttack;

        private List<MovementAlgorithmTile> ListMVChoice;
        private List<MovementAlgorithmTile> ListMovedOverTerrain;
        private List<Vector3> ListMovedOverPoint;
        private bool HasFocus;

        public ActionPanelMovePart1(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
        }

        public ActionPanelMovePart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, MovementAlgorithmTile LastPosition, float LastDirection, Vector3 LastCameraPosition, bool IsPostAttack = false)
            : base(PanelName, Map, !IsPostAttack)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.LastPosition = LastPosition;
            this.LastDirection = LastDirection;
            this.LastCameraPosition = LastCameraPosition;
            this.IsPostAttack = IsPostAttack;

            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            HasFocus = true;
        }

        public override void OnSelect()
        {
            ActiveSquad.SetPosition(LastPosition.WorldPosition + new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0));
            ActiveSquad.Direction = LastDirection;
            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;
            LastCusorMVPosition = LastPosition;

            Map.Camera2DPosition = LastCameraPosition;

            ListMVChoice = Map.GetMVChoice(ActiveSquad, (DeathmatchMap)LastPosition.Owner);
            ListMovedOverTerrain = new List<MovementAlgorithmTile>();
            ListMovedOverPoint = new List<Vector3>();
            ListMovedOverTerrain.Add(Map.GetTerrain(ActiveSquad.Position));
            ListMovedOverPoint.Add(ActiveSquad.Position);
            ActiveSquad.CurrentLeader.CurrentAttack = null;
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

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                if (CheckIfUnitCanMove())
                {
                    HasFocus = false;
                    ListNextChoice.Clear();

                    AddToPanelListAndSelect(new ActionPanelMovePart2(Map, ActivePlayerIndex, ActiveSquadIndex, IsPostAttack, ListMovedOverPoint));

                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else if (Map.CursorPosition == ActiveSquad.Position)
            {
                ListMovedOverTerrain.Clear();
                ListMovedOverPoint.Clear();
                ListMovedOverTerrain.Add(Map.GetTerrain(ActiveSquad.Position));
                ListMovedOverPoint.Add(ActiveSquad.Position);
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
            int MaxMV = Map.GetSquadMaxMovement(ActiveSquad);//Maximum distance you can reach.

            MovementAlgorithmTile LastTerrin = Map.GetTerrain(ActiveSquad.Position);
            float CurrentMVCost = 0;
            for (int T = 1; T < ListMovedOverTerrain.Count; T++)
            {
                MovementAlgorithmTile ActiveTerrain = ListMovedOverTerrain[T];
                CurrentMVCost += Map.Pathfinder.GetMVCost(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, LastTerrin, ActiveTerrain);
            }

            MovementAlgorithmTile NextTerrain = Map.GetTerrain(Map.CursorPosition);
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

            MovementAlgorithmTile CurrentTerrain = Map.GetTerrain(Map.CursorPosition);

            do
            {
                if (!ListMovedOverTerrain.Contains(CurrentTerrain))
                {
                    ListMovedOverTerrain.Add(CurrentTerrain);
                    ListMovedOverPoint.Add(new Vector3(CurrentTerrain.WorldPosition.X, CurrentTerrain.WorldPosition.Y, CurrentTerrain.LayerIndex));
                    if (CurrentTerrain.WorldPosition == ActiveSquad.Position)
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
                for (int CurrentSquadOffsetX = 0; CurrentSquadOffsetX < ActiveSquad.ArrayMapSize.GetLength(0); ++CurrentSquadOffsetX)
                {
                    for (int CurrentSquadOffsetY = 0; CurrentSquadOffsetY < ActiveSquad.ArrayMapSize.GetLength(1); ++CurrentSquadOffsetY)
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
            ActiveSquadIndex = BR.ReadInt32();
            LastPosition = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            LastDirection = BR.ReadFloat();
            LastCameraPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

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
            BW.AppendInt32(ActiveSquadIndex);
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
            return new ActionPanelMovePart1(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
