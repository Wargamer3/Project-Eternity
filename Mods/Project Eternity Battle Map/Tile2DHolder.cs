using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.BattleMapScreen.Terrain;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Tile2DHolder
    {
        private List<Rectangle> ListTile2D;
        private List<Vector3> ListTileWorldPosition;
        private Texture2D sprTileset;
        private int AnimationFrames;
        private Texture2D sprTilesetBumpMap;
        private Texture2D sprTilesetHeightMap;//R = Depth, G = Wall Left to right angle, B = Wall Up to down angle where 255 = upward wall
        public Effect WetEffect;
        private bool CanUseEffect;
        private double TimeElapsed;

        public Tile2DHolder(int AnimationFrames, Texture2D sprTileset)
        {
            this.sprTileset = sprTileset;
            this.AnimationFrames = AnimationFrames;

            ListTile2D = new List<Rectangle>();
            ListTileWorldPosition = new List<Vector3>();

            if (WetEffect != null)
            {
                this.WetEffect = WetEffect.Clone();
                CanUseEffect = true;
            }
            else
            {
                CanUseEffect = false;
            }
        }

        public Tile2DHolder(int AnimationFrames, string TilesetName,  ContentManager Content, Effect WetEffect)
        {
            this.AnimationFrames = AnimationFrames;
            ListTile2D = new List<Rectangle>();
            ListTileWorldPosition = new List<Vector3>();

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

        public Tile2DHolder(int AnimationFrames, ContentManager Content, Effect WetEffect, string TilesetName)
        {
            this.AnimationFrames = AnimationFrames;
            ListTile2D = new List<Rectangle>();
            ListTileWorldPosition = new List<Vector3>();

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
                sprTileset = Content.Load<Texture2D>("Maps/" + TilesetName);

                if (CanUseEffect && File.Exists("Content/Maps/" + TilesetName + " NormalMap.xnb"))
                {
                    sprTilesetBumpMap = Content.Load<Texture2D>("Maps/" + TilesetName + " NormalMap");
                    this.WetEffect.Parameters["NormalMap"].SetValue(sprTilesetBumpMap);
                }
                else
                {
                    CanUseEffect = false;
                }

                if (CanUseEffect && File.Exists("Content/Maps/" + TilesetName + " HeightMap.xnb"))
                {
                    sprTilesetHeightMap = Content.Load<Texture2D>("Maps/" + TilesetName + " HeightMap");
                    this.WetEffect.Parameters["HeightMap"].SetValue(sprTilesetHeightMap);
                }
                else
                {
                    CanUseEffect = false;
                }
            }
        }

        public void AddTile(Rectangle TileOrigin, Vector3 TileWorldPosition)
        {
            ListTile2D.Add(TileOrigin);
            ListTileWorldPosition.Add(TileWorldPosition);
        }

        public void Update(GameTime gameTime)
        {
            if (AnimationFrames > 0)
            {
                TimeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Draw(CustomSpriteBatch g, BattleMap Owner, float LayerDepth)
        {
            if (CanUseEffect)
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);
            }
            else
            {
                g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            }

            int FinalOffsetX = 0;

            if (AnimationFrames > 0)
            {
                int OffsetX = sprTileset.Width / AnimationFrames;
                FinalOffsetX = ((int)(TimeElapsed * 2) % AnimationFrames) * OffsetX;
            }

            for (int T = 0; T < ListTile2D.Count; T++)
            {
                Rectangle ActiveTile = ListTile2D[T];
                Vector3 ActiveTileWorldPosition = ListTileWorldPosition[T];

                Color FinalColor = Color.White;
                float FinalHeight = ActiveTileWorldPosition.Z - 1;

                if (FinalHeight > Owner.Camera2DPosition.Z && !Owner.IsEditor)
                {
                    FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - Owner.Camera2DPosition.Z) * 255);
                }

                g.Draw(sprTileset,
                        new Vector2(ActiveTileWorldPosition.X - Owner.Camera2DPosition.X, ActiveTileWorldPosition.Y - Owner.Camera2DPosition.Y),
                        new Rectangle((ActiveTile.X + FinalOffsetX) % sprTileset.Width, ActiveTile.Y, ActiveTile.Width, ActiveTile.Height), FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth);

            }

            g.End();
        }

        public void Draw(CustomSpriteBatch g, Vector2 Offset, BattleMap Owner, float LayerDepth)
        {
            if (CanUseEffect)
            {
                g.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, WetEffect);
            }
            else
            {
                g.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            }

            for (int T = 0; T < ListTile2D.Count; T++)
            {
                Rectangle ActiveTile = ListTile2D[T];
                Vector3 ActiveTileWorldPosition = ListTileWorldPosition[T];

                Color FinalColor = Color.White;
                float FinalHeight = ActiveTileWorldPosition.Z - 1;

                if (FinalHeight > Owner.Camera2DPosition.Z && !Owner.IsEditor)
                {
                    FinalColor.A = (byte)Math.Min(255, 255 - (FinalHeight - Owner.Camera2DPosition.Z) * 255);
                }

                g.Draw(sprTileset,
                        new Vector2(ActiveTileWorldPosition.X - Owner.Camera2DPosition.X + Offset.X, ActiveTileWorldPosition.Y - Owner.Camera2DPosition.Y + Offset.Y),
                        new Rectangle(ActiveTile.X, ActiveTile.Y, ActiveTile.Width, ActiveTile.Height), FinalColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth);
            }

            g.End();
        }

        public void Clear()
        {
            ListTile2D.Clear();
            ListTileWorldPosition.Clear();
        }
    }
}
