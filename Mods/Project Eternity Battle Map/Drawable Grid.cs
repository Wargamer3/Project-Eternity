using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct DrawableTile
    {
        public Rectangle Origin;//X, Y origin from at which the tile is located in the TileSet.
        public int Tileset;

        public DrawableTile(BinaryReader BR, int TileWidth, int TileHeight)
        {
            Tileset = BR.ReadInt32();
            Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TileWidth, TileHeight);
        }

        public DrawableTile(Rectangle Origin, int Tileset)
        {
            this.Origin = Origin;
            this.Tileset = Tileset;
        }

        public DrawableTile(DrawableTile TilePreset) : this()
        {
            this.Origin = TilePreset.Origin;
            this.Tileset = TilePreset.Tileset;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Tileset);
            BW.Write(Origin.X);
            BW.Write(Origin.Y);
        }
    }

    public interface DrawableGrid
    {
        void Save(BinaryWriter BW);
        void Load(BinaryReader BR);
        void Update(GameTime gameTime);
        void RemoveTileset(int TilesetIndex);
        void AddDrawablePoints(List<Vector3> ListPoint, Color PointColor);
        void BeginDraw(CustomSpriteBatch g);
        void Draw(CustomSpriteBatch g);
        void Reset();
    }

    public abstract class Map2D : DrawableGrid
    {
        private BattleMap Map;
        private Dictionary<Color, List<Vector3>> DicDrawablePointPerColor;

        protected Point MapSize { get { return Map.MapSize; } }

        protected Point TileSize { get { return Map.TileSize; } }

        protected Vector3 CameraPosition { get { return Map.CameraPosition; } }

        private DrawableTile[,] ArrayTile;

        public float Depth;

        public Map2D(BattleMap Map)
        {
            this.Map = Map;
            DicDrawablePointPerColor = new Dictionary<Color, List<Vector3>>();
            Depth = 1f;
        }

        public void Save(BinaryWriter BW)
        {
            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y].Save(BW);
                }
            }
        }

        public void Load(BinaryReader BR)
        {
            ArrayTile = new DrawableTile[MapSize.X, MapSize.Y];

            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(BR, TileSize.X, TileSize.Y);
                }
            }
        }

        public DrawableTile GetTile(int X, int Y)
        {
            return ArrayTile[X, Y];
        }
        
        public void ReplaceTile(int X, int Y, DrawableTile ReplacementTile)
        {
            this.ArrayTile[X, Y] = ReplacementTile;
        }

        public void ReplaceGrid(DrawableTile[,] ReplacementGrid)
        {
            ArrayTile = ReplacementGrid;
        }

        public void ReplaceForegrounds(List<AnimationBackground> ListForegrounds)
        {
            ListForegrounds.Clear();
            ListForegrounds.AddRange(ListForegrounds);
        }

        public void Update(GameTime gameTime)
        {
            DicDrawablePointPerColor.Clear();
        }

        public void RemoveTileset(int TilesetIndex)
        {
            for (int X = ArrayTile.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = ArrayTile.GetLength(1) - 1; Y >= 0; --Y)
                {
                    if (ArrayTile[X, Y].Tileset > TilesetIndex)
                    {
                        --ArrayTile[X, Y].Tileset;
                    }
                }
            }
        }

        public void AddDrawablePoints(List<Vector3> ListPoint, Color PointColor)
        {
            DicDrawablePointPerColor.Add(PointColor, ListPoint);
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            for (int X = ArrayTile.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = ArrayTile.GetLength(1) - 1; Y >= 0; --Y)
                {
                    g.Draw(Map.ListTileSet[ArrayTile[X, Y].Tileset],
                        new Vector2((X - CameraPosition.X) * TileSize.X, (Y - CameraPosition.Y) * TileSize.Y), 
                        ArrayTile[X, Y].Origin, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, Depth);
                }
            }

            if (Map.ShowUnits)
            {
                DrawDrawablePoints(g);

                DrawPlayers(g);

                DrawCursor(g);
            }
        }
        
        public abstract void DrawPlayers(CustomSpriteBatch g);
        
        public void DrawUnitMap(CustomSpriteBatch g, Color PlayerColor, UnitMapComponent ActiveSquad, bool IsGreyed)
        {
            //If it's dead, don't draw it.
            if (!ActiveSquad.IsActive)
                return;

            float PosZ = ActiveSquad.Z;

            if (Map.MovementAnimation.Contains(ActiveSquad))
            {
                int IndexOfUnit = Map.MovementAnimation.IndexOf(ActiveSquad);
                float PosX = (Map.MovementAnimation.ListPosX[IndexOfUnit] - CameraPosition.X) * TileSize.X;
                float PosY = (Map.MovementAnimation.ListPosY[IndexOfUnit] - CameraPosition.Y) * TileSize.Y;

                if (ActiveSquad.IsFlying)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }

                ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
                g.End();
                g.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
                g.End();
                g.Begin();
            }
            else
            {
                Color UnitColor;
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                    UnitColor = PlayerColor;
                else
                    UnitColor = Color.White;

                float PosX = (ActiveSquad.X - CameraPosition.X) * TileSize.X;
                float PosY = (ActiveSquad.Y - CameraPosition.Y) * TileSize.Y;

                if (ActiveSquad.IsFlying)
                {
                    g.Draw(Map.sprUnitHover, new Vector2(PosX, PosY), Color.White);
                    PosY -= 7;
                }
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.NonColoredWithBorder)
                {
                    Vector2 TextureRealSize = new Vector2(ActiveSquad.Width, ActiveSquad.Height);
                    Vector2 TextureOuputSize = new Vector2(TextureRealSize.X + 2, TextureRealSize.Y + 2);

                    Vector2 PixelSize = new Vector2(1 / TextureOuputSize.X, 1 / TextureOuputSize.Y);
                    Vector2 TextureScale = TextureOuputSize / TextureRealSize;

                    Map.fxOutline.Parameters["TextureScale"].SetValue(TextureScale);
                    Map.fxOutline.Parameters["OffsetScale"].SetValue(PixelSize * TextureScale);

                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxOutline);

                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX - 1, PosY - 1, PosZ), (int)TextureOuputSize.X, (int)TextureOuputSize.Y, PlayerColor);
                    g.End();
                    g.Begin();
                }
                //Unit can't move, grayed.
                if (IsGreyed)
                {
                    g.End();
                    g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, Map.fxGrayscale);

                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);

                    g.End();
                    g.Begin();

                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), Color.FromNonPremultiplied(UnitColor.R, UnitColor.G, UnitColor.B, 140));
                }
                else
                {
                    ActiveSquad.Draw2DOnMap(g, new Vector3(PosX, PosY, PosZ), UnitColor);
                }

                ActiveSquad.DrawExtraOnMap(g, new Vector3(PosX, PosY, PosZ), Color.White);
            }
        }

        private void DrawCursor(CustomSpriteBatch g)
        {
            //Draw cursor.
            g.Draw(Map.sprCursor, new Vector2((Map.CursorPositionVisible.X - CameraPosition.X) * TileSize.X, (Map.CursorPositionVisible.Y - CameraPosition.Y) * TileSize.Y), Color.White);
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            foreach (KeyValuePair<Color, List<Vector3>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
                foreach (Vector3 DrawablePoint in DrawablePointPerColor.Value)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)(DrawablePoint.X - CameraPosition.X) * TileSize.X, (int)(DrawablePoint.Y - CameraPosition.Y) * TileSize.Y, TileSize.X, TileSize.Y), DrawablePointPerColor.Key);
                }
            }
        }

        public void Reset()
        {
            //Nothing to do.
        }
    }
}
