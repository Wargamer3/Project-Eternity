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

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PlatformSpawner : InteractiveProp
    {
        public class MapSelector : UITypeEditor
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
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMaps);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(13);
                    }
                }
                return value;
            }
        }

        private readonly BattleMap Map;

        private BattleMapPlatform Platform;
        private BattleMap PlatformMap;

        private string _MapPath;

        public PlatformSpawner(BattleMap Map)
            : base("Platform", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;

            _MapPath = string.Empty;
            Platform = new BattleMapPlatform();
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _MapPath = BR.ReadString();
            float Yaw = BR.ReadSingle();
            float Pitch = BR.ReadSingle();
            float Roll = BR.ReadSingle();

            LoadPlatformMap();

            Platform.Yaw = Yaw;
            Platform.Pitch = Pitch;
            Platform.Roll = Roll;

            Map.AddPlatform(Platform);
        }

        private void LoadPlatformMap()
        {
            if (BattleMap.DicBattmeMapType.Count == 0)
            {
                BattleMap.LoadMapTypes();
            }

            string MapType = _MapPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
            BattleMap NewMap = BattleMap.DicBattmeMapType[MapType].GetNewMap("Platform");

            NewMap.BattleMapPath = _MapPath.Remove(0, MapType.Length + 1);
            NewMap.ListGameScreen = Map.ListGameScreen;

            NewMap.Load();
            NewMap.Init();
            NewMap.TogglePreview(true);
            NewMap.IsEditor = Map.IsEditor;
            Platform.SetMap(NewMap, Map);
            Platform.Position = Position;
            PlatformMap = NewMap;
        }

        public override void DoSave(BinaryWriter BW)
        {
            BW.Write(_MapPath);
            BW.Write(Yaw);
            BW.Write(Pitch);
            BW.Write(Roll);
        }

        public override void Update(GameTime gameTime)
        {
            if (PlatformMap == null)
            {
                return;
            }

            /*Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            Platform.Yaw = MathHelper.ToRadians(Rotation);

            float CenterX = Map.MapSize.X * Map.TileSize.X / 2;
            float CenterY = Map.MapSize.Y * Map.TileSize.Y / 2;
            float LengthX = CenterX * (float)Math.Sin(Platform.Yaw);
            float LengthY = CenterY * (float)Math.Cos(Platform.Yaw);

            Platform.Position = new Vector3(CenterX + LengthX, 32, CenterY + LengthY);*/
            Platform.Position = new Vector3(Position.X * Map.TileSize.X + Map.TileSize.X / 2, Position.Z, Position.Y * Map.TileSize.Y  + Map.TileSize.Y / 2);
            Platform.UpdateWorld();
        }

        public override List<ActionPanel> OnUnitSelected(Squad SelectedUnit)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
        }

        public override List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn)
        {
            List<ActionPanel> ListPanel = new List<ActionPanel>();

            return ListPanel;
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

        public override void Draw3D(GraphicsDevice GraphicsDevice, CustomSpriteBatch g)
        {
        }

        protected override InteractiveProp Copy()
        {
            PlatformSpawner NewProp = new PlatformSpawner(Map);

            NewProp.Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D"), GameScreen.sprPixel, 1);

            return NewProp;
        }

        [Editor(typeof(MapSelector), typeof(UITypeEditor)),
        CategoryAttribute("Platform"),
        DescriptionAttribute("The Weapon path."),
        DefaultValueAttribute(0)]
        public string MapPath
        {
            get
            {
                return _MapPath;
            }
            set
            {
                _MapPath = value;
                LoadPlatformMap();
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The platform yaw."),
        DefaultValueAttribute(0)]
        public float Yaw
        {
            get
            {
                return MathHelper.ToDegrees(Platform.Yaw);
            }
            set
            {
                Platform.Yaw = MathHelper.ToRadians(value);
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The platform pitch."),
        DefaultValueAttribute(0)]
        public float Pitch
        {
            get
            {
                return MathHelper.ToDegrees(Platform.Pitch);
            }
            set
            {
                Platform.Pitch = MathHelper.ToRadians(value);
            }
        }

        [CategoryAttribute("Platform"),
        DescriptionAttribute("The platform roll."),
        DefaultValueAttribute(0)]
        public float Roll
        {
            get
            {
                return MathHelper.ToDegrees(Platform.Roll);
            }
            set
            {
                Platform.Roll = MathHelper.ToRadians(value);
            }
        }
    }
}
