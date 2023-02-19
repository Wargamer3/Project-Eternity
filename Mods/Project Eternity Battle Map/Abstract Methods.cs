using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        public abstract byte[] GetSnapshotData();
        public abstract void Update(double ElapsedSeconds);
        public abstract void RemoveOnlinePlayer(string PlayerID, IOnlineConnection ActivePlayer);
        public abstract void Load(byte[] ArrayGameData);
        public abstract GameScreen GetMultiplayerScreen();

        public abstract void AddPlatform(BattleMapPlatform NewPlatform);

        public abstract void SharePlayer(BattleMapPlayer SharedPlayer, bool IsLocal);

        public void AddLocalPlayer(OnlinePlayerBase NewPlayer)
        {
            DoAddLocalPlayer(NewPlayer);
        }

        protected abstract void DoAddLocalPlayer(OnlinePlayerBase NewPlayer);

        public abstract void SetMutators(List<Mutator> ListMutator);

        public abstract void RemoveUnit(int PlayerIndex, UnitMapComponent UnitToRemove);

        public abstract void AddUnit(int PlayerIndex, UnitMapComponent UnitToAdd, MovementAlgorithmTile NewPosition);

        public abstract void ReplaceTile(int X, int Y, int LayerIndex, DrawableTile ActiveTile);

        public abstract MovementAlgorithmTile GetNextLayerIndex(MovementAlgorithmTile CurrentPosition, int NextX, int NextY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility);

        public abstract MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex);

        public abstract List<MovementAlgorithmTile> GetSpawnLocations(int Team);

        public abstract void Save(string FilePath);

        public abstract void SaveTemporaryMap();

        public abstract BattleMap LoadTemporaryMap(BinaryReader BR);

        public abstract BattleMap GetNewMap(GameModeInfo GameInfo, string ParamsID);

        public abstract string GetMapType();

        public abstract Dictionary<string, GameModeInfo> GetAvailableGameModes();

        public virtual void Resize(int Width, int Height)
        {
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Width, Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;

            fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

            MapRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        public abstract void SetWorld(Matrix World);

        public abstract bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement);

        public abstract Dictionary<string, ActionPanel> GetOnlineActionPanel();
    }
}
