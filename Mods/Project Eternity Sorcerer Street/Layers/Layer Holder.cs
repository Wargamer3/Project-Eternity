﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class LayerHolderSorcererStreet : LayerHolder
    {
        public List<MapLayer> ListLayer;

        private readonly SorcererStreetMap Map;
        private readonly TileInformationPopupManager TileInformationManager;

        public LayerHolderSorcererStreet(SorcererStreetMap Map)
        {
            this.Map = Map;
            ListLayer = new List<MapLayer>();
            LayerHolderDrawable = new Map2DDrawable(Map, Map.LayerManager);
            TileInformationManager = new TileInformationPopupManager(Map, this);
            TileInformationManager.Load(Map.Content);
        }

        public LayerHolderSorcererStreet(SorcererStreetMap Map, BinaryReader BR)
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
                LayerHolderDrawable = new Map2DDrawable(Map, Map.LayerManager);
            }
            else
            {
                LayerHolderDrawable = new Map3DDrawable(Map, this, GameScreen.GraphicsDevice);
            }

            TileInformationManager = new TileInformationPopupManager(Map, this);
            TileInformationManager.Load(Map.Content);
        }

        public void CursorMoved()
        {
            LayerHolderDrawable.CursorMoved();
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
            TileInformationManager.Update(gameTime);

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
                LayerHolderDrawable = new Map2DDrawable(Map, Map.LayerManager);
            }
        }

        public override void TogglePreview(bool UsePreview)
        {
            if (!UsePreview || Map.CameraType == "2D")
            {
                LayerHolderDrawable = new Map2DDrawable(Map, Map.LayerManager);
            }
            else
            {
                LayerHolderDrawable = new Map3DDrawable(Map, this, GameScreen.GraphicsDevice);
            }
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

        public void UnitMoved(int PlayerIndex)
        {
            LayerHolderDrawable.UnitMoved(PlayerIndex);
        }

        public void UnitKilled(int PlayerIndex)
        {
            LayerHolderDrawable.UnitKilled(PlayerIndex);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.Draw(g);
            TileInformationManager.Draw(g);
        }

        public override void EndDraw(CustomSpriteBatch g)
        {
            LayerHolderDrawable.EndDraw(g);
        }
    }
}
