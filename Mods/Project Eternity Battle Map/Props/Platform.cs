using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlatform
    {
        private BattleMap PlatformMap;

        public AIContainer PlatformAI;
        public Vector3 Position;
        public float Yaw;
        public float Pitch;
        public float Roll;
        private float OffsetX;
        private float OffsetY;

        public void SetMap(BattleMap Map, BattleMap Parent)
        {
            this.PlatformMap = Map;
            Map.IsAPlatform = true;
            Map.Camera = Parent.Camera;

            OffsetX = PlatformMap.MapSize.X * PlatformMap.TileSize.X / 2;
            OffsetY = PlatformMap.MapSize.Y * PlatformMap.TileSize.Y / 2;
        }

        public void UpdateWorld()
        {
            Matrix ToOrigin = Matrix.CreateTranslation(-new Vector3(OffsetX, 0, OffsetY));

            Matrix YawMatrix = Matrix.CreateRotationY(Yaw);
            Matrix PitchMatrix = Matrix.CreateRotationX(Pitch);
            Matrix RollMatrix = Matrix.CreateRotationZ(Roll);

            Matrix ToFinalPosition = Matrix.CreateTranslation(new Vector3(Position.X + OffsetX - PlatformMap.TileSize.X / 2, Position.Y, Position.Z + OffsetY - PlatformMap.TileSize.Y / 2));

            PlatformMap.SetWorld(ToOrigin * YawMatrix * PitchMatrix * RollMatrix * ToFinalPosition, Position);
        }

        public MovementAlgorithmTile FindTileFromGlobalPosition(int X, int Y, int Z)
        {
            int FinalX = X - (int)Position.X / PlatformMap.TileSize.X;
            int FinalY = Y - (int)Position.Z / PlatformMap.TileSize.Y;
            int FinalZ = Z - (int)Position.Y / 32;

            return PlatformMap.GetMovementTile(FinalX, FinalY, FinalZ);
        }

        public void Update(GameTime gameTime)
        {
            PlatformMap.Update(gameTime);
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            PlatformMap.BeginDraw(g);
        }

        public void Draw(CustomSpriteBatch g)
        {
            PlatformMap.Draw(g);
        }
    }
}
