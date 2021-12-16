using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.TilesetOriginSelector;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptChangeTerrain : DeathmatchMapScript
        {
            private ChangeTerrainAttribute TerrainAttribute;
            private List<Vector2> ListTerrainChangeLocation;
            private Texture2D sprTileset;
            private float _TilesPerSeconds;
            private float _MinSimultaneousTiles;

            private float CurrentTransformingIndex;
            private int RealCurrentTransformingIndex;

            private double TransformationCounter;
            private double TilesTransformationCounter;

            public ScriptChangeTerrain()
                : this(null)
            {
            }

            public ScriptChangeTerrain(DeathmatchMap Map)
                : base(Map, 150, 50, "Change Terrain", new string[] { "Change" }, new string[] { "Terrain Changed" })
            {
                TerrainAttribute = new ChangeTerrainAttribute(string.Empty, new Point(32, 32), new Rectangle(0, 0, 32, 32));

                _TilesPerSeconds = 6;
                _MinSimultaneousTiles = 2;

                ListTerrainChangeLocation = new List<Vector2>();
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!Map.ListTileSet.Contains(sprTileset))
                {
                    Map.ListTileSet.Add(sprTileset);
                }

                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                TransformationCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (TransformationCounter >= TilesTransformationCounter)
                {
                    TransformationCounter -= TilesTransformationCounter;

                    int TilesetIndex = Map.ListTileSet.IndexOf(sprTileset);

                    MapLayer ActiveLayer = Map.ListLayer[0];

                    for (int T = RealCurrentTransformingIndex; T < ListTerrainChangeLocation.Count && T < RealCurrentTransformingIndex + _MinSimultaneousTiles; T++)
                    {
                        Vector2 ActivePosition = ListTerrainChangeLocation[T];
                        int X = (int)ActivePosition.X;
                        int Y = (int)ActivePosition.Y;
                        DrawableTile ActiveTile = ActiveLayer.OriginalLayerGrid.GetTile(X, Y);
                        ActiveTile.Origin = TerrainAttribute.Origin;
                        ActiveTile.Tileset = TilesetIndex;
                        ActiveLayer.OriginalLayerGrid.ReplaceTile(X, Y, ActiveTile);
                    }

                    CurrentTransformingIndex += _MinSimultaneousTiles;
                    RealCurrentTransformingIndex = (int)CurrentTransformingIndex;

                    ActiveLayer.ResetGrid();
                }

                if (RealCurrentTransformingIndex >= ListTerrainChangeLocation.Count)
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TerrainAttribute.Tileset = BR.ReadString();
                TerrainAttribute.TileSize = new Point(BR.ReadInt32(), BR.ReadInt32());
                TerrainAttribute.Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TerrainAttribute.TileSize.X, TerrainAttribute.TileSize.Y);

                _TilesPerSeconds = BR.ReadSingle();
                _MinSimultaneousTiles = BR.ReadSingle();

                TilesTransformationCounter = 1 / _TilesPerSeconds * _MinSimultaneousTiles;

                int ListTerrainChangeLocationCount = BR.ReadInt32();
                ListTerrainChangeLocation = new List<Vector2>(ListTerrainChangeLocationCount);
                for (int L = 0; L < ListTerrainChangeLocationCount; ++L)
                {
                    ListTerrainChangeLocation.Add(new Vector2(BR.ReadSingle(), BR.ReadSingle()));
                }

                if (Map != null)
                {
                    sprTileset = Map.Content.Load<Texture2D>("Maps/Tilesets/" + TerrainAttribute.Tileset);
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TerrainAttribute.Tileset);
                BW.Write(TerrainAttribute.TileSize.X);
                BW.Write(TerrainAttribute.TileSize.Y);
                BW.Write(TerrainAttribute.Origin.X);
                BW.Write(TerrainAttribute.Origin.Y);

                BW.Write(_TilesPerSeconds);
                BW.Write(_MinSimultaneousTiles);

                BW.Write(ListTerrainChangeLocation.Count);
                for (int L = 0; L < ListTerrainChangeLocation.Count; ++L)
                {
                    BW.Write(ListTerrainChangeLocation[L].X);
                    BW.Write(ListTerrainChangeLocation[L].Y);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                ScriptChangeTerrain NewScript = new ScriptChangeTerrain(Map);
                NewScript.Tileset = Tileset;
                NewScript.TileSize = TileSize;
                NewScript.TilesetOrigin = TilesetOrigin;
                NewScript.MapDestination.AddRange(MapDestination);
                NewScript.TilesPerSeconds = TilesPerSeconds;
                NewScript.MinSimultaneousTiles = MinSimultaneousTiles;
                NewScript.TilesTransformationCounter = TilesTransformationCounter;
                NewScript.sprTileset = sprTileset;
                return NewScript;
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

            [Editor(typeof(MapDestinationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Terrain change locations"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public List<Vector2> MapDestination
            {
                get
                {
                    return ListTerrainChangeLocation;
                }
                set
                {
                    ListTerrainChangeLocation = value;
                }
            }

            [CategoryAttribute("Tileset"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public float TilesPerSeconds
            {
                get
                {
                    return _TilesPerSeconds;
                }
                set
                {
                    _TilesPerSeconds = value;
                }
            }

            [CategoryAttribute("Tileset"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public float MinSimultaneousTiles
            {
                get
                {
                    return _MinSimultaneousTiles;
                }
                set
                {
                    _MinSimultaneousTiles = value;
                }
            }

            #endregion
        }
    }
}
