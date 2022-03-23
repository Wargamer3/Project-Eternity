using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptEditMap : DeathmatchMapScript
        {
            private float _TilesPerSeconds;
            private float _MinSimultaneousTiles;

            private float CurrentTransformingIndex;
            private int RealCurrentTransformingIndex;

            private double TransformationCounter;
            private double TilesTransformationCounter;
            public MapEditorSelector.ChangeTerrainAttribute TerrainAttribute;

            public ScriptEditMap()
                : this(null)
            {
            }

            public ScriptEditMap(DeathmatchMap Map)
                : base(Map, 150, 50, "Edit Map", new string[] { "Change" }, new string[] { "Map Changed" })
            {
                _TilesPerSeconds = 6;
                _MinSimultaneousTiles = 2;
                TerrainAttribute = new MapEditorSelector.ChangeTerrainAttribute(new Point(32, 32));
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                TransformationCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (TransformationCounter >= TilesTransformationCounter)
                {
                    TransformationCounter -= TilesTransformationCounter;

                    MapLayer ActiveLayer = Map.LayerManager.ListLayer[0];

                    for (int T = RealCurrentTransformingIndex; T < TerrainAttribute.ListTerrainChangeLocation.Count && T < RealCurrentTransformingIndex + _MinSimultaneousTiles; T++)
                    {
                        Terrain ActiveTerrain = TerrainAttribute.ListTerrainChangeLocation[T];
                        int PosX = (int)ActiveTerrain.WorldPosition.X;
                        int PosY = (int)ActiveTerrain.WorldPosition.Y;
                        DrawableTile ActiveTile = TerrainAttribute.ListTileChangeLocation[T];
                        ActiveLayer.ArrayTerrain[PosX, PosY] = ActiveTerrain;
                        ActiveLayer.LayerGrid.ReplaceTile(PosX, PosY, ActiveTile);
                    }

                    CurrentTransformingIndex += _MinSimultaneousTiles;
                    RealCurrentTransformingIndex = (int)CurrentTransformingIndex;

                    Map.LayerManager.LayerHolderDrawable.Reset();
                }

                if (RealCurrentTransformingIndex >= TerrainAttribute.ListTerrainChangeLocation.Count)
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
                TerrainAttribute.TileSize.X = BR.ReadInt32();
                TerrainAttribute.TileSize.Y = BR.ReadInt32();

                _TilesPerSeconds = BR.ReadSingle();
                _MinSimultaneousTiles = BR.ReadSingle();

                TilesTransformationCounter = 1 / _TilesPerSeconds * _MinSimultaneousTiles;

                int ListTilesetCount = BR.ReadInt32();
                TerrainAttribute.ListTileset = new List<string>(ListTilesetCount);
                for (int L = 0; L < ListTilesetCount; ++L)
                {
                    TerrainAttribute.ListTileset.Add(BR.ReadString());
                }

                int ListTerrainChangeLocationCount = BR.ReadInt32();
                TerrainAttribute.ListTerrainChangeLocation = new List<Terrain>(ListTerrainChangeLocationCount);
                for (int L = 0; L < ListTerrainChangeLocationCount; ++L)
                {
                    float TileX = BR.ReadSingle();
                    float TileY = BR.ReadSingle();
                    int Tileset = BR.ReadInt32();
                    Rectangle Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TerrainAttribute.TileSize.X, TerrainAttribute.TileSize.Y);
                    TerrainAttribute.ListTerrainChangeLocation.Add(new Terrain(BR, TileX, TileY, 0));
                    TerrainAttribute.ListTileChangeLocation.Add(new DrawableTile(Origin, Tileset));
                }

                if (Map != null)
                {
                    bool TilesetAdded = false;
                    for (int T = 0; T < TerrainAttribute.ListTileset.Count; ++T)
                    {
                        bool AlreadyExist = false;
                        for (int P = 0; P < Map.ListTilesetPreset.Count; ++P)
                        {
                            if (TerrainAttribute.ListTileset[T] == Map.ListTilesetPreset[P].TilesetName)
                            {
                                AlreadyExist = true;
                                break;
                            }
                        }

                        if (!AlreadyExist)
                        {
                            Texture2D sprTileset = Map.Content.Load<Texture2D>("Maps/Tilesets/" + TerrainAttribute.ListTileset[T]);
                            if (!Map.ListTileSet.Contains(sprTileset))
                            {
                                TilesetAdded = true;
                                Map.ListTilesetPreset.Add(new Terrain.TilesetPreset(TerrainAttribute.ListTileset[T], sprTileset.Width, sprTileset.Height, Map.TileSize.X, Map.TileSize.Y, Map.ListTilesetPreset.Count));
                                Map.ListTileSet.Add(sprTileset);
                            }
                        }
                    }

                    if (TilesetAdded)//Recalculate the tileset index
                    {
                        for (int L = 0; L < ListTerrainChangeLocationCount; ++L)
                        {
                            int OriginalTilesetIndex = TerrainAttribute.ListTileChangeLocation[L].TilesetIndex;

                            for (int P = 0; P < Map.ListTilesetPreset.Count; ++P)
                            {
                                if (P != OriginalTilesetIndex && TerrainAttribute.ListTileset[OriginalTilesetIndex] == Map.ListTilesetPreset[P].TilesetName)
                                {
                                    TerrainAttribute.ListTileChangeLocation[L] = new DrawableTile(TerrainAttribute.ListTileChangeLocation[L].Origin, P);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TerrainAttribute.TileSize.X);
                BW.Write(TerrainAttribute.TileSize.Y);

                BW.Write(_TilesPerSeconds);
                BW.Write(_MinSimultaneousTiles);

                BW.Write(TerrainAttribute.ListTileset.Count);
                for (int L = 0; L < TerrainAttribute.ListTileset.Count; ++L)
                {
                    BW.Write(TerrainAttribute.ListTileset[L]);
                }

                BW.Write(TerrainAttribute.ListTerrainChangeLocation.Count);
                for (int L = 0; L < TerrainAttribute.ListTerrainChangeLocation.Count; ++L)
                {
                    BW.Write(TerrainAttribute.ListTerrainChangeLocation[L].WorldPosition.X);
                    BW.Write(TerrainAttribute.ListTerrainChangeLocation[L].WorldPosition.Y);
                    BW.Write(TerrainAttribute.ListTileChangeLocation[L].TilesetIndex);
                    BW.Write(TerrainAttribute.ListTileChangeLocation[L].Origin.X);
                    BW.Write(TerrainAttribute.ListTileChangeLocation[L].Origin.Y);
                    TerrainAttribute.ListTerrainChangeLocation[L].Save(BW);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                ScriptEditMap NewScript = new ScriptEditMap(Map);
                NewScript.TerrainAttribute = TerrainAttribute;
                NewScript.TilesPerSeconds = TilesPerSeconds;
                NewScript.MinSimultaneousTiles = MinSimultaneousTiles;
                NewScript.TilesTransformationCounter = TilesTransformationCounter;
                return NewScript;
            }

            #region Properties

            [Editor(typeof(MapEditorSelector), typeof(UITypeEditor)),
            CategoryAttribute("Terrain change locations"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public MapEditorSelector.ChangeTerrainAttribute MapDestination
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
