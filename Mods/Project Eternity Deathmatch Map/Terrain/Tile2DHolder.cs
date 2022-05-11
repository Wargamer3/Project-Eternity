using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Tile2DHolder
    {
        private List<Terrain> ListTile3D;
        Texture2D sprTileset;
        Texture2D sprTilesetBumpMap;
        Texture2D sprTilesetHeightMap;//R = Depth, G = Wall Left to right angle, B = Wall Up to down angle where 255 = upward wall
        public Effect WetEffect;
        private bool CanUseEffect;

        public Tile2DHolder(string TilesetName, ContentManager Content, Effect WetEffect)
        {
            ListTile3D = new List<Terrain>();

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
            ListTile3D.Add(NewTile);
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (CanUseEffect)
            {
                g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);
            }
            else
            {
                g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            }

            foreach (var ActiveTile in ListTile3D)
            {
                Color FinalColor = Color.White;
                float FinalHeight = ActiveTile.WorldPosition.Z;

                if (FinalHeight > ActiveTile.Owner.CameraPosition.Z)
                {
                    FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - ActiveTile.Owner.CameraPosition.Z) * 255);
                }

                g.Draw(sprTileset,
                        new Vector2((ActiveTile.InternalPosition.X - ActiveTile.Owner.CameraPosition.X) * ActiveTile.Owner.TileSize.X, (ActiveTile.InternalPosition.Y - ActiveTile.Owner.CameraPosition.Y) * ActiveTile.Owner.TileSize.Y),
                        ActiveTile.DrawableTile.Origin, FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, ActiveTile.LayerDepth);
            }

            g.End();
        }
    }
}
