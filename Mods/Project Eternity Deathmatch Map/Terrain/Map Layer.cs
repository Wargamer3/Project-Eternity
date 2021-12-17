using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapLayer : IMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;
        public float Depth { get { return _Depth; } set { _Depth = value; if (OriginalLayerGrid != null) OriginalLayerGrid.Depth = value; } }
        private float _Depth;

        public DrawableGrid LayerGrid;
        public readonly DeathmatchMap2D OriginalLayerGrid;
        public Terrain[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private DeathmatchMap Map;

        public MapLayer(DeathmatchMap Map)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            IsVisible = true;

            //Tiles
            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(X, Y);
                }
            }

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map);
            _Depth = OriginalLayerGrid.Depth;
        }

        public MapLayer(DeathmatchMap Map, BinaryReader BR)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();

            StartupDelay = BR.ReadInt32();
            ToggleDelayOn = BR.ReadInt32();
            ToggleDelayOff = BR.ReadInt32();
            _Depth = BR.ReadSingle();

            if (StartupDelay == 0)
            {
                IsVisible = true;
                ToggleTimer = ToggleDelayOn;
            }
            else
            {
                IsVisible = false;
                ToggleTimer = StartupDelay;
            }

            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(BR, X, Y);
                }
            }

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map, BR);
            OriginalLayerGrid.Depth = Depth;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(StartupDelay);
            BW.Write(ToggleDelayOn);
            BW.Write(ToggleDelayOff);
            BW.Write(Depth);

            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y].Save(BW);
                }
            }

            LayerGrid.Save(BW);
        }

        public void Update(GameTime gameTime)
        {
            if (ToggleDelayOn > 0)
            {
                ToggleTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (ToggleTimer <= 0)
                {
                    IsVisible = !IsVisible;
                    if (IsVisible)
                    {
                        ToggleTimer += ToggleDelayOn;
                    }
                    else
                    {
                        ToggleTimer += ToggleDelayOff;
                    }
                }
            }

            LayerGrid.Update(gameTime);
        }

        public void ResetGrid()
        {
            LayerGrid.Reset();
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (IsVisible)
                LayerGrid.BeginDraw(g);
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (IsVisible)
            {
                LayerGrid.Draw(g);

                if (Map.ShowTerrainType)
                {
                    int IndexOfLayer = Map.ListLayer.IndexOf(this);
                    if (Map.ShowLayerIndex >= 0)
                    {
                        IndexOfLayer = 0;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            switch (ArrayTerrain[X, Y].TerrainTypeIndex)
                            {
                                case 0:
                                    TextColor = Color.DeepSkyBlue;
                                    break;
                                case 1:
                                    TextColor = Color.White;
                                    break;
                                case 2:
                                    TextColor = Color.Navy;
                                    break;
                                case 3:
                                    TextColor = Color.DarkGray;
                                    break;
                                case 4:
                                    TextColor = Color.Red;
                                    break;
                                case 5:
                                    TextColor = Color.Yellow;
                                    break;
                            }
                            TextHelper.DrawText(g, ArrayTerrain[X, Y].TerrainTypeIndex.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }

                if (Map.ShowTerrainHeight)
                {
                    int IndexOfLayer = Map.ListLayer.IndexOf(this);
                    if (Map.ShowLayerIndex >= 0)
                    {
                        IndexOfLayer = 0;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            if (ArrayTerrain[X, Y].Position.Z >= 2)
                            {
                                TextColor = Color.Red;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 1)
                            {
                                TextColor = Color.Orange;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 0.75)
                            {
                                TextColor = Color.Yellow;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 0.5)
                            {
                                TextColor = Color.Green;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z > 0)
                            {
                                TextColor = Color.SkyBlue;
                            }

                            TextHelper.DrawText(g, ArrayTerrain[X, Y].Position.Z.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {

        }

        public override string ToString()
        {
            return "Layer";
        }
    }
}
