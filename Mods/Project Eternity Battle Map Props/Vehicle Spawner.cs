using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Vehicle;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class VehicleSpawner : InteractiveProp
    {
        public class VehicleSelector : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVehicles);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(17);
                    }
                }
                return value;
            }
        }

        private BasicEffect PolygonEffect;

        private readonly BattleMap Map;

        private Vehicle VehicleToSpawn;
        private Vehicle LastVehicleSpawned;

        private Tile3D Preview3D;

        private string _VehiclePath;
        private int _TurnsBeforeRespawn;
        private int _MaxNumberOfVehicleSpawned;
        private float _Yaw;
        private float _Pitch;
        private float _Roll;

        public VehicleSpawner(BattleMap Map)
            : base("Vehicle Spawner", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _VehiclePath = string.Empty;
        }

        public override void Load(ContentManager Content)
        {
            PolygonEffect = new BasicEffect(GameScreen.GraphicsDevice);

            PolygonEffect.TextureEnabled = true;
            PolygonEffect.EnableDefaultLighting();

            float aspectRatio = GameScreen.GraphicsDevice.Viewport.Width / (float)GameScreen.GraphicsDevice.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;
        }

        public override void DoLoad(BinaryReader BR)
        {
            _VehiclePath = BR.ReadString();
            _TurnsBeforeRespawn = BR.ReadInt32();
            _MaxNumberOfVehicleSpawned = BR.ReadInt32();
            float Yaw = BR.ReadSingle();
            float Pitch = BR.ReadSingle();
            float Roll = BR.ReadSingle();

            if (!string.IsNullOrEmpty(_VehiclePath))
            {
                VehicleToSpawn = new Vehicle(_VehiclePath, Map.Content);
            }
        }

        private void CreatePreview()
        {
            MovementAlgorithmTile SpawnTerrain = Map.GetMovementTile((int)Position.X, (int)Position.Y, (int)Position.Z);
            Terrain3D ActiveTerrain3D = SpawnTerrain.DrawableTile.Terrain3DInfo;

            int X = SpawnTerrain.InternalPosition.X;
            int Y = SpawnTerrain.InternalPosition.Y;
            float Z = SpawnTerrain.WorldPosition.Z * 32 + 0.1f;
            Preview3D = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X,
                Y * Map.TileSize.Y,
                Z,
                SpawnTerrain.LayerIndex * 32 + 0.1f,
                new Point(VehicleToSpawn.sprVehicle.Width, VehicleToSpawn.sprVehicle.Height),
                new List<Texture2D>() { VehicleToSpawn.sprVehicle }, Z, Z, Z, Z, 0)[0];
        }

        private void SpawnVehicle()
        {
            MovementAlgorithmTile SpawnTerrain = Map.GetMovementTile((int)Position.X, (int)Position.Y, (int)Position.Z);
            Terrain3D ActiveTerrain3D = SpawnTerrain.DrawableTile.Terrain3DInfo;

            int X = SpawnTerrain.InternalPosition.X;
            int Y = SpawnTerrain.InternalPosition.Y;
            float Z = SpawnTerrain.WorldPosition.Z * 32 + 0.1f;
            Tile3D TerrainToSpawnOn = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                0 * Map.TileSize.X,
                0 * Map.TileSize.Y,
                0,
                SpawnTerrain.LayerIndex * 32 + 0.1f,
                new Point(VehicleToSpawn.sprVehicle.Width, VehicleToSpawn.sprVehicle.Height),
                new List<Texture2D>() { VehicleToSpawn.sprVehicle }, Z, Z, Z, Z, 0)[0];

            LastVehicleSpawned = VehicleToSpawn.Copy();

            LastVehicleSpawned.Position = new Vector3(SpawnTerrain.WorldPosition.X * Map.TileSize.X, SpawnTerrain.WorldPosition.Z * 32, SpawnTerrain.WorldPosition.Y * Map.TileSize.Y);

            LastVehicleSpawned.SetVertex(TerrainToSpawnOn.ArrayVertex, TerrainToSpawnOn.ArrayIndex);

            LastVehicleSpawned.World = Matrix.CreateTranslation(new Vector3(-LastVehicleSpawned.sprVehicle.Width / 2, 0, -LastVehicleSpawned.sprVehicle.Height / 2)) * Matrix.CreateFromYawPitchRoll(LastVehicleSpawned.Yaw, LastVehicleSpawned.Pitch, LastVehicleSpawned.Roll) * Matrix.CreateTranslation(LastVehicleSpawned.Position);

            Map.ListVehicle.Add(LastVehicleSpawned);
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_VehiclePath);
            BW.Write(_TurnsBeforeRespawn);
            BW.Write(_MaxNumberOfVehicleSpawned);
            BW.Write(Yaw);
            BW.Write(Pitch);
            BW.Write(Roll);
        }

        public override void Update(GameTime gameTime)
        {
            if (VehicleToSpawn != null && LastVehicleSpawned == null)
            {
                CreatePreview();
                SpawnVehicle();
            }
        }

        public override void OnUnitSelected(ActionPanel PanelOwner, Squad SelectedUnit)
        {
        }

        public override void OnUnitBeforeStop(ActionPanel PanelOwner, Squad StoppedUnit, Vector3 PositionToStopOn)
        {
        }

        public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
        {
        }

        public override void OnUnitStop(Squad StoppedUnit)
        {
        }

        public override void OnBattleEnd(Squad Attacker, Squad Defender)
        {
        }

        public override void OnTurnEnd(int PlayerIndex)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
            float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;
            g.Draw(GameScreen.sprPixel, new Rectangle((int)PosX, (int)PosY, 32, 32), Color.Red);
        }

        public override void Draw3D(GraphicsDevice GraphicsDevice, Matrix View, CustomSpriteBatch g)
        {
            if (Preview3D != null && Map.IsEditor)
            {
                PolygonEffect.View = Unit3D.UnitEffect3D.Parameters["View"].GetValueMatrix();
                PolygonEffect.Texture = VehicleToSpawn.sprVehicle;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                Preview3D.Draw(g.GraphicsDevice);
            }
        }

        protected override InteractiveProp Copy()
        {
            VehicleSpawner NewProp = new VehicleSpawner(Map);

            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), GameScreen.sprPixel, 1);
            NewProp.PolygonEffect = PolygonEffect;

            return NewProp;
        }

        [Editor(typeof(VehicleSelector), typeof(UITypeEditor)),
        CategoryAttribute("Platform"),
        DescriptionAttribute("The vehicle path."),
        DefaultValueAttribute(0)]
        public string VehiclePath
        {
            get
            {
                return _VehiclePath;
            }
            set
            {
                _VehiclePath = value;
                VehicleToSpawn = new Vehicle(_VehiclePath, Map.Content);
                CreatePreview();
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("Number of Turns Before Respawn."),
        DefaultValueAttribute(0)]
        public int TurnBeforeRespawn
        {
            get
            {
                return _TurnsBeforeRespawn;
            }
            set
            {
                _TurnsBeforeRespawn = value;
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("Max Number Of Vehicle Spawned."),
        DefaultValueAttribute(0)]
        public int MaxNumberOfVehicleSpawned
        {
            get
            {
                return _MaxNumberOfVehicleSpawned;
            }
            set
            {
                _MaxNumberOfVehicleSpawned = value;
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The vehicle yaw."),
        DefaultValueAttribute(0)]
        public float Yaw
        {
            get
            {
                return MathHelper.ToDegrees(_Yaw);
            }
            set
            {
                _Yaw = MathHelper.ToRadians(value);
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The vehicle pitch."),
        DefaultValueAttribute(0)]
        public float Pitch
        {
            get
            {
                return MathHelper.ToDegrees(_Pitch);
            }
            set
            {
                _Pitch = MathHelper.ToRadians(value);
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The vehicle roll."),
        DefaultValueAttribute(0)]
        public float Roll
        {
            get
            {
                return MathHelper.ToDegrees(_Roll);
            }
            set
            {
                _Roll = MathHelper.ToRadians(value);
            }
        }
    }
}
