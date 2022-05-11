using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
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

            for (int L = 0; L < LayerCount; ++L)
            {
                ListLayer.Add(new MapLayer(Map, BR, L));
            }

            if (Map.CameraType == "2D")
            {
                LayerHolderDrawable = new DeathmatchMap2DHolder(Map, Map.LayerManager);
            }
            else
            {
                LayerHolderDrawable = new Map3DDrawable(Map, this, GameScreen.GraphicsDevice);
            }
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

            if ((KeyboardHelper.KeyHold(Keys.LeftControl) || KeyboardHelper.KeyHold(Keys.RightControl)) && KeyboardHelper.KeyPressed(Keys.K))
            {
                LayerHolderDrawable = new Map3DDrawable(Map, this, GameScreen.GraphicsDevice);
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
                LayerHolderDrawable = new DeathmatchMap2DHolder(Map, Map.LayerManager);
            }
        }

        public override void TogglePreview(bool UsePreview)
        {

            if (!UsePreview || Map.CameraType == "2D")
            {
                LayerHolderDrawable = new DeathmatchMap2DHolder(Map, Map.LayerManager);
            }
            else
            {
                LayerHolderDrawable = new Map3DDrawable(Map, this, GameScreen.GraphicsDevice);
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

        public override void AddDamageNumber(string Damage, Vector3 Position)
        {
            LayerHolderDrawable.AddDamageNumber(Damage, Position);
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
