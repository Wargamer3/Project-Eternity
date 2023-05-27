using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Tile2DHolder
    {
        private List<Terrain> ListTile2D;
        Texture2D sprTileset;
        Texture2D sprTilesetBumpMap;
        Texture2D sprTilesetHeightMap;//R = Depth, G = Wall Left to right angle, B = Wall Up to down angle where 255 = upward wall
        public Effect WetEffect;
        private bool CanUseEffect;

        public Tile2DHolder(string TilesetName, ContentManager Content, Effect WetEffect)
        {
            ListTile2D = new List<Terrain>();

            if (WetEffect != null)
            {
                this.WetEffect = WetEffect.Clone();
                CanUseEffect = true;
            }
            else
            {
                CanUseEffect = false;
            }

            if (Content != null)
            {
                sprTileset = Content.Load<Texture2D>("Maps/Tilesets/" + TilesetName);

                if (CanUseEffect && File.Exists("Content/Maps/Tilesets/" + TilesetName + " NormalMap.xnb"))
                {
                    sprTilesetBumpMap = Content.Load<Texture2D>("Maps/Tilesets/" + TilesetName + " NormalMap");
                    this.WetEffect.Parameters["NormalMap"].SetValue(sprTilesetBumpMap);
                }
                else
                {
                    CanUseEffect = false;
                }

                if (CanUseEffect && File.Exists("Content/Maps/Tilesets/" + TilesetName + " HeightMap.xnb"))
                {
                    sprTilesetHeightMap = Content.Load<Texture2D>("Maps/Tilesets/" + TilesetName + " HeightMap");
                    this.WetEffect.Parameters["HeightMap"].SetValue(sprTilesetHeightMap);
                }
                else
                {
                    CanUseEffect = false;
                }
            }
        }

        public void AddTile(Terrain NewTile)
        {
            ListTile2D.Add(NewTile);
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (CanUseEffect)
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);
            }
            else
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            }

            foreach (Terrain ActiveTile in ListTile2D)
            {
                Color FinalColor = Color.White;
                float FinalHeight = ActiveTile.WorldPosition.Z - 1;

                if (FinalHeight > ActiveTile.Owner.Camera2DPosition.Z && !ActiveTile.Owner.IsEditor)
                {
                    FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - ActiveTile.Owner.Camera2DPosition.Z) * 255);
                }

                g.Draw(sprTileset,
                        new Vector2((ActiveTile.InternalPosition.X - ActiveTile.Owner.Camera2DPosition.X) * ActiveTile.Owner.TileSize.X, (ActiveTile.InternalPosition.Y - ActiveTile.Owner.Camera2DPosition.Y) * ActiveTile.Owner.TileSize.Y),
                        ActiveTile.DrawableTile.Origin, FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, ActiveTile.LayerDepth);
            }

            g.End();
        }

        public void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            if (CanUseEffect)
            {
                g.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);
            }
            else
            {
                g.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            }

            foreach (Terrain ActiveTile in ListTile2D)
            {
                Color FinalColor = Color.White;
                float FinalHeight = ActiveTile.WorldPosition.Z;

                if (FinalHeight > ActiveTile.Owner.Camera2DPosition.Z)
                {
                    FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - ActiveTile.Owner.Camera2DPosition.Z) * 255);
                }

                g.Draw(sprTileset,
                        new Vector2((ActiveTile.InternalPosition.X - ActiveTile.Owner.Camera2DPosition.X) * ActiveTile.Owner.TileSize.X, (ActiveTile.InternalPosition.Y - ActiveTile.Owner.Camera2DPosition.Y) * ActiveTile.Owner.TileSize.Y) + Offset,
                        ActiveTile.DrawableTile.Origin, FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, ActiveTile.LayerDepth);
            }

            g.End();
        }
    }
}
