using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlatform
    {
        private BattleMap PlatformMap;
        public BattleMap Map => PlatformMap;

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
            Map.Camera3D = Parent.Camera3D;

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

            PlatformMap.SetWorld(ToOrigin * YawMatrix * PitchMatrix * RollMatrix * ToFinalPosition);
        }

        public void AddLocalPlayer(BattleMapPlayer ActivePlayer)
        {
            PlatformMap.AddLocalPlayer(ActivePlayer);
        }

        public MovementAlgorithmTile FindTileFromLocalPosition(int X, int Y, int Z)
        {
            return PlatformMap.GetMovementTile(X, Y, Z);
        }
        
        public List<MovementAlgorithmTile> GetCampaignEnemySpawnLocations()
        {
            return PlatformMap.GetCampaignEnemySpawnLocations();
        }

        public List<MovementAlgorithmTile> GetMultiplayerSpawnLocations(int Team)
        {
            return PlatformMap.GetMultiplayerSpawnLocations(Team);
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
