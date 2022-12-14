using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class ImageText
    {
        public string OriginalText;
        public Texture2D sprImage;

        public int Draw(CustomSpriteBatch g, Vector2 Position)
        {
            return sprImage.Width;
        }
    }
}
