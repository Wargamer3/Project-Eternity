using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelControlVehicle : ActionPanelDeathmatch
    {
        private const string PanelName = "ControlVehicle";

        private const float CameraHeight = 100;
        private const float CameraDistance = 200;

        private Camera3D Camera;

        private readonly Vehicle ActiveVehicle;

        private float FuelRemaining;
        private float MaxFuel;

        public ActionPanelControlVehicle(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelControlVehicle(DeathmatchMap Map, Vehicle ActiveVehicle)
            : base(PanelName, Map)
        {
            this.ActiveVehicle = ActiveVehicle;
        }

        public override void OnSelect()
        {
            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            Map.Camera3DOverride = Camera;
            MaxFuel = FuelRemaining = 120;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Matrix RotationMatrix = Matrix.CreateFromYawPitchRoll(ActiveVehicle.Yaw - MathHelper.PiOver2, ActiveVehicle.Pitch, ActiveVehicle.Roll);
            Vector3 Forward = Vector3.Transform(Vector3.Forward, RotationMatrix);

            Camera.CameraPosition3D = Vector3.Transform(new Vector3(0, 0, CameraDistance), Matrix.CreateRotationY(ActiveVehicle.Yaw - MathHelper.PiOver2))
                + ActiveVehicle.Position;
            Camera.CameraPosition3D = Vector3.Transform(Camera.CameraPosition3D, Matrix.CreateTranslation(0f, CameraHeight, 0f));
            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, ActiveVehicle.Position, Vector3.Up);

            if (ActiveInputManager.InputUpHold())
            {
                ActiveVehicle.Position += Forward * (float)gameTime.ElapsedGameTime.TotalSeconds  * 100;
                FuelRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds * 15;
            }
            if (ActiveInputManager.InputLeftHold())
            {
                ActiveVehicle.Yaw +=3 * 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (ActiveInputManager.InputRightHold())
            {
                ActiveVehicle.Yaw += 3 * -1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            ActiveVehicle.World = Matrix.CreateTranslation(new Vector3(-ActiveVehicle.sprVehicle.Width / 2, 0, -ActiveVehicle.sprVehicle.Height / 2)) * Matrix.CreateFromYawPitchRoll(ActiveVehicle.Yaw, ActiveVehicle.Pitch, ActiveVehicle.Roll) * Matrix.CreateTranslation(ActiveVehicle.Position);
        }

        private void GetTerrainUnderVehicle()
        {
            MovementAlgorithmTile SpawnTerrain = Map.GetMovementTile((int)ActiveVehicle.Position.X / Map.TileSize.X, (int)ActiveVehicle.Position.Z / Map.TileSize.Y, (int)ActiveVehicle.Position.Y / 32);
            Terrain3D ActiveTerrain3D = SpawnTerrain.DrawableTile.Terrain3DInfo;

            int X = SpawnTerrain.InternalPosition.X;
            int Y = SpawnTerrain.InternalPosition.Y;
            float Z = SpawnTerrain.WorldPosition.Z * Map.LayerHeight + 0.1f;
            Tile3D TerrainToSpawnOn = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X,
                Y * Map.TileSize.Y,
                Z,
                SpawnTerrain.LayerIndex * Map.LayerHeight + 0.1f,
                new Point(ActiveVehicle.sprVehicle.Width, ActiveVehicle.sprVehicle.Height),
                new Point(ActiveVehicle.sprVehicle.Width, ActiveVehicle.sprVehicle.Height),
                new List<Texture2D>() { ActiveVehicle.sprVehicle }, Z, Z, Z, Z, 0)[0];

            ActiveVehicle.Position = new Vector3(SpawnTerrain.WorldPosition.X * Map.TileSize.X, SpawnTerrain.WorldPosition.Z * Map.LayerHeight, SpawnTerrain.WorldPosition.Y * Map.TileSize.Y);

            ActiveVehicle.SetVertex(TerrainToSpawnOn.ArrayVertex, TerrainToSpawnOn.ArrayIndex);
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelControlVehicle(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 240, Constants.Height - 30), 40, 20, Color.Black);
            g.DrawString(Map.fntArial8, "Fuel", new Vector2(Constants.Width - 230, Constants.Height - 27), Color.White);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 200, Constants.Height - 30), 180, 20, Color.Black);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 195, Constants.Height - 26, (int)(170 * (FuelRemaining / MaxFuel)), 12), Color.Green);
        }

        public override string ToString()
        {
            return "Drive";
        }
    }
}
