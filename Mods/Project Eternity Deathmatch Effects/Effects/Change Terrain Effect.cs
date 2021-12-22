using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.TilesetOriginSelector;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class ChangeTerrainEffect : DeathmatchEffect
    {
        public static string Name = "Change Terrain";

        private ChangeTerrainAttribute TerrainAttribute;
        private Texture2D sprTileset;

        public ChangeTerrainEffect()
            : base(Name, false)
        {
            TerrainAttribute = new ChangeTerrainAttribute(string.Empty, new Point(32, 32), new Rectangle(0, 0, TileSize.X, TileSize.Y));
        }

        public ChangeTerrainEffect(DeathmatchParams Params)
            : base(Name, false, Params)
        {
            TerrainAttribute = new ChangeTerrainAttribute(string.Empty, new Point(32, 32), new Rectangle(0, 0, TileSize.X, TileSize.Y));
        }

        protected override void Load(BinaryReader BR)
        {
            TerrainAttribute.Tileset = BR.ReadString();
            TerrainAttribute.TileSize = new Point(BR.ReadInt32(), BR.ReadInt32());
            TerrainAttribute.Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());

            if (Params != null)
            {
                sprTileset = Params.LocalContext.Map.Content.Load<Texture2D>("Maps/Tilesets/" + TerrainAttribute.Tileset);
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(TerrainAttribute.Tileset);
            BW.Write(TerrainAttribute.TileSize.X);
            BW.Write(TerrainAttribute.TileSize.Y);
            BW.Write(TerrainAttribute.Origin.X);
            BW.Write(TerrainAttribute.Origin.Y);
            BW.Write(TerrainAttribute.Origin.Width);
            BW.Write(TerrainAttribute.Origin.Height);
        }

        protected override string DoExecuteEffect()
        {
            if (!Params.LocalContext.Map.ListTileSet.Contains(sprTileset))
            {
                Params.LocalContext.Map.ListTileSet.Add(sprTileset);
            }

            int TilesetIndex = Params.LocalContext.Map.ListTileSet.IndexOf(sprTileset);

            MapLayer ActiveLayer = Params.LocalContext.Map.ListLayer[0];
            foreach (Vector3 ActivePosition in Params.LocalContext.ArrayAttackPosition)
            {
                int X = (int)ActivePosition.X;
                int Y = (int)ActivePosition.Y;
                DrawableTile ActiveTile = ActiveLayer.OriginalLayerGrid.GetTile(X, Y);
                ActiveTile.Origin = TerrainAttribute.Origin;
                ActiveTile.TilesetIndex = TilesetIndex;
                ActiveLayer.OriginalLayerGrid.ReplaceTile(X, Y, ActiveTile);
            }

            ActiveLayer.ResetGrid();

            return TerrainAttribute.Tileset + " (" + TerrainAttribute.Origin.X + ", " + TerrainAttribute.Origin.Y + ")";
        }

        protected override void ReactivateEffect()
        {
            //Don't change terrain on reactivation
        }

        protected override BaseEffect DoCopy()
        {
            ChangeTerrainEffect NewEffect = new ChangeTerrainEffect(Params);

            NewEffect.TerrainAttribute = TerrainAttribute;
            NewEffect.sprTileset = sprTileset;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeTerrainEffect NewEffect = (ChangeTerrainEffect)Copy;

            TerrainAttribute = NewEffect.TerrainAttribute;
            sprTileset = NewEffect.sprTileset;
        }

        #region Properties

        [Editor(typeof(TilesetSelector), typeof(UITypeEditor)),
        CategoryAttribute("Tileset"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Tileset
        {
            get
            {
                return TerrainAttribute.Tileset;
            }
            set
            {
                TerrainAttribute.Tileset = value;
            }
        }
        
        [CategoryAttribute("Tileset"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Point TileSize
        {
            get
            {
                return TerrainAttribute.TileSize;
            }
            set
            {
                TerrainAttribute.TileSize = value;
                TerrainAttribute.Origin.Width = value.X;
                TerrainAttribute.Origin.Height = value.Y;
            }
        }
        
        [Editor(typeof(TilesetOriginSelector), typeof(UITypeEditor)),
        CategoryAttribute("Tileset Origin"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ChangeTerrainAttribute TilesetOrigin
        {
            get
            {
                return TerrainAttribute;
            }
            set
            {
                TerrainAttribute = value;
            }
        }

        #endregion
    }
}
