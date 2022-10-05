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
                sprTileset = Params.Map.Content.Load<Texture2D>("Maps/Tilesets/" + TerrainAttribute.Tileset);
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
            if (!Params.Map.ListTileSet.Contains(sprTileset))
            {
                Params.Map.ListTileSet.Add(sprTileset);
            }

            int TilesetIndex = Params.Map.ListTileSet.IndexOf(sprTileset);

            foreach (MovementAlgorithmTile ActivePosition in LocalContext.ArrayAttackPosition)
            {
                int X = ActivePosition.InternalPosition.X;
                int Y = ActivePosition.InternalPosition.Y;
                DrawableTile ActiveTile = ActivePosition.Owner.GetMovementTile(X, Y, ActivePosition.LayerIndex).DrawableTile;
                ActiveTile.Origin = TerrainAttribute.Origin;
                ActiveTile.TilesetIndex = TilesetIndex;
                ActivePosition.Owner.ReplaceTile(X, Y, ActivePosition.LayerIndex, ActiveTile);
            }

            Params.Map.LayerManager.LayerHolderDrawable.Reset();

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
