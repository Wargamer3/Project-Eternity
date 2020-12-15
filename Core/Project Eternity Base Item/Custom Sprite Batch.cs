﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core
{
    public class CustomSpriteBatch
    {
        public Matrix Scale;
        private SpriteBatch g;

        public GraphicsDevice GraphicsDevice { get { return g.GraphicsDevice; } }

        public CustomSpriteBatch(SpriteBatch g)
        {
            this.g = g;
            Scale = Matrix.CreateScale(1);
        }

        public void Begin()
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Scale);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState)
        {
            g.Begin(sortMode, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Scale);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            g.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, null, Scale);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            g.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Scale);
        }

        public void Begin(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            g.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, Scale * transformMatrix);
        }

        public void BeginUnscaled(SpriteSortMode sortMode, BlendState blendState)
        {
            g.Begin(sortMode, blendState);
        }

        public void BeginUnscaled(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect)
        {
            g.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            g.Draw(texture, position, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            g.Draw(texture, position, sourceRectangle, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            g.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            g.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            g.Draw(texture, destinationRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            g.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            g.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            g.DrawString(spriteFont, text, position, color);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDept)
        {
            g.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDept);
        }

        public void DrawStringMiddleAligned(SpriteFont spriteFont, string Text, Vector2 Position, Color TextColor)
        {
            int TextWidth = (int)spriteFont.MeasureString(Text).X / 2;
            DrawString(spriteFont, Text, Position, TextColor, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 0);
        }

        public void DrawStringRightAligned(SpriteFont spriteFont, string Text, Vector2 Position, Color TextColor)
        {
            int TextWidth = (int)spriteFont.MeasureString(Text).X;
            DrawString(spriteFont, Text, Position, TextColor, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 0);
        }

        public void DrawLine(Texture2D sprPixel, Vector2 StartPos, Vector2 EndPos, Color ActiveColor, int width = 1)
        {
            Vector2 Line = StartPos - EndPos;

            Rectangle LineSize = new Rectangle((int)StartPos.X, (int)StartPos.Y, (int)Line.Length() + width, width);
            Line.Normalize();

            //Get line angle.
            float angle = (float)System.Math.Acos(Vector2.Dot(Line, -Vector2.UnitX));

            if (StartPos.Y > EndPos.Y)
                angle = MathHelper.TwoPi - angle;

            Draw(sprPixel, LineSize, null, ActiveColor, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void End()
        {
            g.End();
        }
    }
}
