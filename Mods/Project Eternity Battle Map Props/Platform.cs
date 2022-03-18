using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
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

        private string _MapPath;
        private float Rotation;

        public PlatformSpawner(BattleMap Map)
            : base("Platform", PropCategories.Interactive, new bool[,] { { true } }, false)
        {
            this.Map = Map;
            Platform = new BattleMapPlatform();

            _MapPath = string.Empty;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override void DoLoad(BinaryReader BR)
        {
            _MapPath = BR.ReadString();
            Yaw = BR.ReadSingle();
            Pitch = BR.ReadSingle();
            Roll = BR.ReadSingle();
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
            Platform.PlatformMap = NewMap;
            Map.AddPlatform(Platform);
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
            Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            float Rad = MathHelper.ToRadians(Rotation);
            Matrix ToOrigin = Matrix.CreateTranslation(-new Vector3(Platform.PlatformMap.MapSize.X * Platform.PlatformMap.TileSize.X, 0, Platform.PlatformMap.MapSize.Y * Platform.PlatformMap.TileSize.Y));
            Matrix Rot = Matrix.CreateRotationY(Rad);

            float CenterX = Map.MapSize.X * Map.TileSize.X / 2;
            float CenterY = Map.MapSize.Y * Map.TileSize.Y / 2;
            float LengthX = CenterX * (float)Math.Sin(Rad);
            float LengthY = CenterY * (float)Math.Cos(Rad);
            Platform.PlatformMap.SetWorld(ToOrigin * Rot
                * Matrix.CreateTranslation(new Vector3(CenterX + LengthX,
                32,
                CenterY + LengthY)));
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
        CategoryAttribute("Spawner"),
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
            }
        }

        [CategoryAttribute("Spawner"),
        DescriptionAttribute("The Weapon path."),
        DefaultValueAttribute(0)]
        public float Yaw
        {
            get
            {
                return Platform.Yaw;
            }
            set
            {
                Platform.Yaw = value;
            }
        }

        [CategoryAttribute("Spawner"),
        DescriptionAttribute("The Weapon path."),
        DefaultValueAttribute(0)]
        public float Pitch
        {
            get
            {
                return Platform.Pitch;
            }
            set
            {
                Platform.Pitch = value;
            }
        }

        [CategoryAttribute("Spawner"),
        DescriptionAttribute("The Weapon path."),
        DefaultValueAttribute(0)]
        public float Roll
        {
            get
            {
                return Platform.Roll;
            }
            set
            {
                Platform.Roll = value;
            }
        }
    }
}
