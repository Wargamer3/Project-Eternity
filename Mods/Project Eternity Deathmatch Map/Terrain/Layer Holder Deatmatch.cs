using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class LayerHolderDeathmatch : LayerHolder
    {
        public List<MapLayer> ListLayer;

        private readonly DeathmatchMap Map;

        public LayerHolderDeathmatch(DeathmatchMap Map)
        {
            this.Map = Map;
            ListLayer = new List<MapLayer>();
        }

        public LayerHolderDeathmatch(DeathmatchMap Map, BinaryReader BR)
        {
            this.Map = Map;
            int LayerCount = BR.ReadInt32();
            ListLayer = new List<MapLayer>(LayerCount);

            for (int i = 0; i < LayerCount; ++i)
            {
                ListLayer.Add(new MapLayer(Map, BR));
            }

            LayerHolderDrawable = new DeathmatchMap2DHolder(Map);
        }

        public override BaseMapLayer this[int i]
        {
            get
            {
                return ListLayer[i];
            }
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(ListLayer.Count);
            foreach (MapLayer ActiveLayer in ListLayer)
            {
                ActiveLayer.Save(BW);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (MapLayer ActiveMapLayer in ListLayer)
            {
                ActiveMapLayer.Update(gameTime);
            }

            LayerHolderDrawable.Update(gameTime);

            if (KeyboardHelper.KeyPressed(Keys.Q))
            {
                do
                {
                    Map.CursorPosition.Z += 1f;
                }
                while (Map.CursorPosition.Z < ListLayer.Count
                && ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex
                && !KeyboardHelper.KeyHold(Keys.LeftAlt));

                if (Map.CursorPosition.Z >= ListLayer.Count)
                {
                    Map.CursorPosition.Z = ListLayer.Count - 1;
                }
            }

            if (KeyboardHelper.KeyPressed(Keys.E))
            {
                do
                {
                    Map.CursorPosition.Z -= 1f;
                }
                while (Map.CursorPosition.Z >= 0
                && ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex
                && !KeyboardHelper.KeyHold(Keys.LeftAlt));

                if (Map.CursorPosition.Z < 0)
                {
                    Map.CursorPosition.Z = 0;
                }
            }

            if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.K))
            {
                LayerHolderDrawable = new Map3DDrawable(Map, GameScreen.GraphicsDevice);
            }
            if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.L))
            {
                //ListLayer[0].LayerGrid = new CubeMap3D(Map, 0, ListLayer[0], GameScreen.GraphicsDevice);
            }
            if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.O))
            {
                //ListLayer[0].LayerGrid = new SphericalMap3D(Map, 0, ListLayer[0], GameScreen.GraphicsDevice);
            }
            if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.U))
            {
                LayerHolderDrawable = new DeathmatchMap2DHolder(Map);
            }
        }

        public override void TogglePreview(bool UsePreview)
        {
            if (!UsePreview)
            {
                LayerHolderDrawable = new DeathmatchMap2DHolder(Map);
            }
            else if (Map.ListTileSet.Count > 0)
            {
                LayerHolderDrawable = new Map3DDrawable(Map, GameScreen.GraphicsDevice);
            }
        }

        public override DrawableTile GetTile(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].LayerGrid.GetTile(X, Y);
        }

        public override void AddDrawablePath(List<MovementAlgorithmTile> ListPoint)
        {
            LayerHolderDrawable.AddDrawablePath(ListPoint);
        }

        public override void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            LayerHolderDrawable.AddDrawablePoints(ListPoint, PointColor);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.Draw(g);
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.EndDraw(g);
        }
    }
}
