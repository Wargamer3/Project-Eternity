using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class Sprite
    {
        public Texture2D SpriteImage;

        public Rectangle SpriteDestination;
        public Rectangle? SpriteSource;

        public Color SpriteColor;

        public Vector2 SpriteOrigin;

        public SpriteEffects SpriteEffects;

        public Sprite(Texture2D SpriteImage, Rectangle SpriteDestination, Rectangle? SpriteSource = null)
        {
            this.SpriteImage = SpriteImage;

            this.SpriteSource = SpriteSource;

            this.SpriteDestination = SpriteDestination;

            SpriteColor = Color.White;

            SpriteOrigin = Vector2.Zero;

            SpriteEffects = SpriteEffects.None;
        }

        public Sprite(Texture2D SpriteImage, int PositionX, int PositionY, Rectangle? SpriteSource = null)
            : this(SpriteImage, new Rectangle(PositionX, PositionY, SpriteImage.Width, SpriteImage.Height), SpriteSource)
        {
        }

        public Sprite(Texture2D SpriteImage, Vector2 Position, Rectangle? SpriteSource = null)
            : this(SpriteImage, new Rectangle((int)Position.X, (int)Position.Y, SpriteImage.Width, SpriteImage.Height), SpriteSource)
        {
        }
    }
}
