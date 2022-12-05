﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using static ProjectEternity.Core.Units.UnitMapComponent;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelMovementPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "MovePlayer";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int Movement;
        private TerrainSorcererStreet NextTerrain;
        private ActionPanelChooseDirection DirectionPicker;

        public ActionPanelMovementPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovementPhase(SorcererStreetMap Map, int ActivePlayerIndex, int Movement)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Movement = Movement;
        }

        public override void OnSelect()
        {
            PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, ActivePlayer.GamePiece.Direction == Directions.None);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (DirectionPicker != null && DirectionPicker.ChosenTerrain != null)
            {
                NextTerrain = DirectionPicker.ChosenTerrain;
                MoveToNextTerrain();
                DirectionPicker = null;
            }

            if (Map.MovementAnimation.Count == 0)
            {
                if (Movement > 0)
                {
                    PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
                }
                else
                {
                    RemoveFromPanelList(this);
                }
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            if (DirectionPicker != null && DirectionPicker.ChosenTerrain != null)
            {
                NextTerrain = DirectionPicker.ChosenTerrain;
                MoveToNextTerrain();
                DirectionPicker = null;
            }

            if (Map.MovementAnimation.Count == 0)
            {
                if (Movement > 0)
                {
                    PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
                }
                else
                {
                    RemoveFromPanelList(this);
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            ActivePlayer.GamePiece.Direction = (Directions)BR.ReadByte();
            ActivePlayer.GamePiece.SetPosition(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            NextTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            Movement = BR.ReadInt32();
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            ByteReader BR = new ByteReader(ArrayUpdateData);
            ActivePlayer.GamePiece.Direction = (Directions)BR.ReadByte();
            ActivePlayer.GamePiece.SetPosition(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            NextTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            Movement = BR.ReadInt32();
            BR.Clear();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendByte((byte)ActivePlayer.GamePiece.Direction);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.X);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.Y);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.Z);
            if (NextTerrain == null)
            {
                BW.AppendFloat(ActivePlayer.GamePiece.Position.X);
                BW.AppendFloat(ActivePlayer.GamePiece.Position.Y);
                BW.AppendFloat(ActivePlayer.GamePiece.Position.Z);
            }
            else
            {
                BW.AppendFloat(NextTerrain.InternalPosition.X);
                BW.AppendFloat(NextTerrain.InternalPosition.Y);
                BW.AppendFloat(NextTerrain.LayerIndex);
            }
            BW.AppendInt32(Movement);
        }

        public override byte[] DoWriteUpdate()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendByte((byte)ActivePlayer.GamePiece.Direction);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.X);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.Y);
            BW.AppendFloat(ActivePlayer.GamePiece.Position.Z);
            if (NextTerrain == null)
            {
                BW.AppendFloat(ActivePlayer.GamePiece.Position.X);
                BW.AppendFloat(ActivePlayer.GamePiece.Position.Y);
                BW.AppendFloat(ActivePlayer.GamePiece.Position.Z);
            }
            else
            {
                BW.AppendFloat(NextTerrain.InternalPosition.X);
                BW.AppendFloat(NextTerrain.InternalPosition.Y);
                BW.AppendFloat(NextTerrain.LayerIndex);
            }
            BW.AppendInt32(Movement);

            byte[] ArrayUpdateData = BW.GetBytes();
            BW.ClearWriteBuffer();

            return ArrayUpdateData;
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMovementPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw remaining movement count in the upper left corner
            GameScreen.DrawBox(g, new Vector2(30, 30), 50, 50, Color.Black);
            g.DrawString(Map.fntArial12, Movement.ToString(), new Vector2(37, 35), Color.White);
        }

        private void MoveToNextTerrain()
        {
            Vector3 FinalPosition = NextTerrain.WorldPosition;
            Map.MovementAnimation.Add(ActivePlayer.GamePiece, ActivePlayer.GamePiece.Position, FinalPosition);
            ActivePlayer.GamePiece.SetPosition(FinalPosition);

            --Movement;
            NextTerrain.OnReached(Map, ActivePlayerIndex, Movement);
            Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
        }

        private void PrepareToMoveToNextTerrain(Vector3 PlayerPosition, int LayerIndex, bool AllowDirectionChange)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = GetNextTerrains(Map, (int)PlayerPosition.X, (int)PlayerPosition.Y, LayerIndex, ActivePlayer.GamePiece.Direction);

            if (DicNextTerrain.Count == 1)
            {
                KeyValuePair<Directions, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
                ActivePlayer.GamePiece.Direction = NextTerrainHolder.Key;
                NextTerrain = NextTerrainHolder.Value;
                MoveToNextTerrain();
            }
            else if (!AllowDirectionChange)
            {
                if (DicNextTerrain.ContainsKey(ActivePlayer.GamePiece.Direction))
                {
                    NextTerrain = DicNextTerrain[ActivePlayer.GamePiece.Direction];
                    MoveToNextTerrain();
                }
                else
                {
                    KeyValuePair<Directions, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
                    ActivePlayer.GamePiece.Direction = NextTerrainHolder.Key;
                    NextTerrain = NextTerrainHolder.Value;
                    MoveToNextTerrain();
                }
            }
            else if (DicNextTerrain.Count > 1)
            {
                NextTerrain = null;
                DirectionPicker = new ActionPanelChooseDirection(Map, ActivePlayerIndex, Movement, DicNextTerrain);
                AddToPanelListAndSelect(DirectionPicker);
            }
            else
            {
                KeyValuePair<Directions, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
                ActivePlayer.GamePiece.Direction = NextTerrainHolder.Key;
                NextTerrain = NextTerrainHolder.Value;
                MoveToNextTerrain();
            }
        }

        private static Dictionary<Directions, TerrainSorcererStreet> GetNextTerrains(SorcererStreetMap Map, int ActiveTerrainX, int ActiveTerrainY, int LayerIndex, Directions PlayerDirection)
        {
            Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain = new Dictionary<Directions, TerrainSorcererStreet>();

            if (ActiveTerrainY - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Up, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY - 1, LayerIndex));
            }
            if (ActiveTerrainY + 1 < Map.MapSize.Y)
            {
                DicNextTerrain.Add(Directions.Down, Map.GetTerrain(ActiveTerrainX, ActiveTerrainY + 1, LayerIndex));
            }
            if (ActiveTerrainX - 1 >= 0)
            {
                DicNextTerrain.Add(Directions.Left, Map.GetTerrain(ActiveTerrainX - 1, ActiveTerrainY, LayerIndex));
            }
            if (ActiveTerrainX + 1 < Map.MapSize.X)
            {
                DicNextTerrain.Add(Directions.Right, Map.GetTerrain(ActiveTerrainX + 1, ActiveTerrainY, LayerIndex));
            }

            Directions[] TerrainDirections = DicNextTerrain.Keys.ToArray();
            foreach (Directions ActiveDirection in TerrainDirections)
            {
                if (PlayerDirection == Directions.Left && ActiveDirection == Directions.Right)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Right && ActiveDirection == Directions.Left)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Up && ActiveDirection == Directions.Down)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == Directions.Down && ActiveDirection == Directions.Up)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (DicNextTerrain[ActiveDirection].TerrainTypeIndex == 0)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
            }

            return DicNextTerrain;
        }
    }
}
