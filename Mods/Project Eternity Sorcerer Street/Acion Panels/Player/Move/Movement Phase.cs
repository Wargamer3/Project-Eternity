using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
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
        private bool Backward;
        private float RotationValue;
        private TerrainSorcererStreet NextTerrain;
        private ActionPanelChooseDirection DirectionPicker;

        public ActionPanelMovementPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovementPhase(SorcererStreetMap Map, int ActivePlayerIndex, int Movement, bool Backward)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Movement = Movement;
            this.Backward = Backward;
        }

        public override void OnSelect()
        {
            PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, ActivePlayer.GamePiece.Direction == DirectionNone);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            RotationValue += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (DirectionPicker != null && DirectionPicker.ChosenTerrain != null)
            {
                if (Backward)
                {
                    Map.MovementAnimation.DicMovingMapUnitByNextPosition.Clear();
                    Map.MovementAnimation.DicMovingMapUnitByPosition.Clear();
                    PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
                    DirectionPicker = null;
                    return;
                }
                NextTerrain = DirectionPicker.ChosenTerrain;
                MoveToNextTerrain();
                DirectionPicker = null;
            }

            if (Map.MovementAnimation.Count == 0)
            {
                if (Movement <= 1)
                {
                    RemoveFromPanelList(this);
                }

                PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            RotationValue += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (DirectionPicker != null && DirectionPicker.ChosenTerrain != null)
            {
                NextTerrain = DirectionPicker.ChosenTerrain;
                MoveToNextTerrain();
                DirectionPicker = null;
            }

            if (Map.MovementAnimation.Count == 0)
            {
                if (Movement <= 1)
                {
                    RemoveFromPanelList(this);
                }

                PrepareToMoveToNextTerrain(ActivePlayer.GamePiece.Position, (int)ActivePlayer.GamePiece.Position.Z, true);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            ActivePlayer.GamePiece.Direction = BR.ReadFloat();
            ActivePlayer.GamePiece.SetPosition(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            NextTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            Movement = BR.ReadInt32();
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            ByteReader BR = new ByteReader(ArrayUpdateData);
            ActivePlayer.GamePiece.Direction = BR.ReadFloat();
            ActivePlayer.GamePiece.SetPosition(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            NextTerrain = Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            Movement = BR.ReadInt32();
            BR.Clear();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendFloat(ActivePlayer.GamePiece.Direction);
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
                BW.AppendFloat(NextTerrain.GridPosition.X);
                BW.AppendFloat(NextTerrain.GridPosition.Y);
                BW.AppendFloat(NextTerrain.LayerIndex);
            }
            BW.AppendInt32(Movement);
        }

        public override byte[] DoWriteUpdate()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendFloat(ActivePlayer.GamePiece.Direction);
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
                BW.AppendFloat(NextTerrain.GridPosition.X);
                BW.AppendFloat(NextTerrain.GridPosition.Y);
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
            MenuHelper.DrawDiceHolder(g, new Vector2(Constants.Width / 8, Constants.Height / 4), Movement);
        }

        private void MoveToNextTerrain()
        {
            Vector3 FinalPosition = NextTerrain.WorldPosition + new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0);

            Map.MovementAnimation.Add(ActivePlayer.GamePiece, ActivePlayer.GamePiece.Position, FinalPosition);

            ActivePlayer.GamePiece.SetPosition(FinalPosition);

            if (NextTerrain.PlayerOwner == ActivePlayer)
            {
                Map.ListPassedTerrain.Add(NextTerrain);
            }

            if (NextTerrain.DefendingCreature != null
                && NextTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).ForceStop)
            {
                Movement = 0;
            }

            --Movement;
            NextTerrain.OnReached(Map, ActivePlayerIndex, Movement);

            if (Movement <= 0 && ListActionMenuChoice.HasMainPanel)
            {
                RemoveFromPanelList(this);
            }

            if (Map.OnlineClient != null)
            {
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
        }

        private void PrepareToMoveToNextTerrain(Vector3 PlayerPosition, int LayerIndex, bool AllowDirectionChange)
        {
            if (Movement <= 0)
            {
                NextTerrain = Map.GetTerrain(PlayerPosition);
                MoveToNextTerrain();
                return;
            }

            Dictionary<float, TerrainSorcererStreet> DicNextTerrain = GetNextTerrains(Map, PlayerPosition, ActivePlayer.GamePiece.Direction, Backward);

            if (DicNextTerrain.Count == 1)
            {
                KeyValuePair<float, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
                ActivePlayer.GamePiece.Direction = NextTerrainHolder.Key;
                if (Backward)
                {
                    ActivePlayer.GamePiece.Direction = (ActivePlayer.GamePiece.Direction + 180) % 360;
                }
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
                    KeyValuePair<float, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
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
                KeyValuePair<float, TerrainSorcererStreet> NextTerrainHolder = DicNextTerrain.First();
                ActivePlayer.GamePiece.Direction = NextTerrainHolder.Key;
                NextTerrain = NextTerrainHolder.Value;
                MoveToNextTerrain();
            }
        }

        private static Dictionary<float, TerrainSorcererStreet> GetNextTerrains(SorcererStreetMap Map, Vector3 CurrentPosition, float PlayerDirection, bool Backward)
        {
            if (Backward && PlayerDirection != DirectionNone)
            {
                PlayerDirection = (PlayerDirection + 180) % 360;
            }
            Dictionary<float, TerrainSorcererStreet> DicNextTerrain = new Dictionary<float, TerrainSorcererStreet>();

            if (Map.IsInsideMap(new Vector3(CurrentPosition.X, CurrentPosition.Y - Map.TileSize.Y, CurrentPosition.Z)))
            {
                DicNextTerrain.Add(DirectionUp, Map.GetTerrain(new Vector3(CurrentPosition.X, CurrentPosition.Y - Map.TileSize.Y, CurrentPosition.Z)));
            }
            if (Map.IsInsideMap(new Vector3(CurrentPosition.X, CurrentPosition.Y + Map.TileSize.Y, CurrentPosition.Z)))
            {
                DicNextTerrain.Add(DirectionDown, Map.GetTerrain(new Vector3(CurrentPosition.X, CurrentPosition.Y + Map.TileSize.Y, CurrentPosition.Z)));
            }
            if (Map.IsInsideMap(new Vector3(CurrentPosition.X - Map.TileSize.X, CurrentPosition.Y, CurrentPosition.Z)))
            {
                DicNextTerrain.Add(DirectionLeft, Map.GetTerrain(new Vector3(CurrentPosition.X - Map.TileSize.X, CurrentPosition.Y, CurrentPosition.Z)));
            }
            if (Map.IsInsideMap(new Vector3(CurrentPosition.X + Map.TileSize.X, CurrentPosition.Y, CurrentPosition.Z)))
            {
                DicNextTerrain.Add(DirectionRight, Map.GetTerrain(new Vector3(CurrentPosition.X + Map.TileSize.X, CurrentPosition.Y, CurrentPosition.Z)));
            }

            float[] TerrainDirections = DicNextTerrain.Keys.ToArray();
            foreach (float ActiveDirection in TerrainDirections)
            {
                if (PlayerDirection == DirectionLeft && ActiveDirection == DirectionRight)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == DirectionRight && ActiveDirection == DirectionLeft)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == DirectionUp && ActiveDirection == DirectionDown)
                {
                    DicNextTerrain.Remove(ActiveDirection);
                }
                else if (PlayerDirection == DirectionDown && ActiveDirection == DirectionUp)
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
