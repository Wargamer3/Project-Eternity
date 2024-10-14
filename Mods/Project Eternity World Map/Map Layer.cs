using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class SubMapLayer : BaseMapLayer, ISubMapLayer
    {
        //Only used to display a grid, should never have movement logic in it.
        public override MovementAlgorithmTile GetTile(int X, int Y)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return " - Sub Layer";
        }
    }

    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public float Depth;

        public Terrain[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private WorldMap Map;

        public MapLayer(WorldMap Map, int LayerIndex)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            ToggleTimer = StartupDelay;
            IsVisible = StartupDelay > 0;

            //Tiles
            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(X, Y, Map.TileSize.X, Map.TileSize.Y, LayerIndex, Map.LayerHeight, Depth);
                }
            }

            ArrayTile = new DrawableTile[Map.MapSize.X, Map.MapSize.Y];

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y), 0);
                }
            }
        }

        public MapLayer(WorldMap Map, BinaryReader BR, int LayerIndex)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();

            StartupDelay = BR.ReadInt32();
            ToggleDelayOn = BR.ReadInt32();
            ToggleDelayOff = BR.ReadInt32();
            Depth = BR.ReadSingle();

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
                    ArrayTerrain[X, Y] = new Terrain(BR, X, Y, Map.TileSize.X, Map.TileSize.Y, LayerIndex, Map.LayerHeight, Depth);
                }
            }

            ArrayTile = new DrawableTile[Map.MapSize.X, Map.MapSize.Y];
            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(BR, Map.TileSize.X, Map.TileSize.Y);
                }
            }
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

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y].Save(BW);
                }
            }
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
                        ToggleTimer = ToggleDelayOn;
                    }
                    else
                    {
                        ToggleTimer = ToggleDelayOff;
                    }
                }
            }
        }

        public override MovementAlgorithmTile GetTile(int X, int Y)
        {
            return ArrayTerrain[X, Y];
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
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
