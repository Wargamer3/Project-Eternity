using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMap
    {
        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public abstract bool CursorControl(PlayerInput ActiveInputManager);
        public abstract Vector3 GetFinalPosition(Vector3 WorldPosition);
        public abstract Tile3D CreateTile3D(int TilesetIndex, Vector3 WorldPosition, Point Origin, Point TileSize, Point TextureSize, float PositionOffset);
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

        public abstract void RemoveUnit(int PlayerIndex, object UnitToRemove);

        public abstract void AddUnit(int PlayerIndex, object UnitToAdd, Vector3 NewPosition);

        public abstract List<Vector3> GetCampaignEnemySpawnLocations();

        public abstract List<Vector3> GetMultiplayerSpawnLocations(int Team);

        public abstract void Save(string FilePath);

        public abstract void SaveTemporaryMap();

        public abstract BattleMap LoadTemporaryMap(BinaryReader BR);

        public abstract BattleMap GetNewMap(GameModeInfo GameInfo, string ParamsID);

        public abstract string GetMapType();

        public abstract Dictionary<string, GameModeInfo> GetAvailableGameModes();

        public virtual void Resize(int Width, int Height)
        {
            try
            {
                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.ScissorRectangle.Width, GraphicsDevice.ScissorRectangle.Height, 0, 0, -1f);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Matrix projectionMatrix = HalfPixelOffset * Projection;

                fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

                MapRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height, false,
                    GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            }
            catch
            {

            }
        }

        public abstract void SetWorld(Matrix World);

        public abstract bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement);

        public abstract Dictionary<string, ActionPanel> GetOnlineActionPanel();
    }
}
