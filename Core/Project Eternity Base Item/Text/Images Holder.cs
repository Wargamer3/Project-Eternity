using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class ImagesHolder
    {
        public Texture2D sprDefault;
        public ContentManager Content;
        public Dictionary<string, Texture2D> DicSprite;

        public ImagesHolder(ContentManager Content)
        {
            DicSprite = new Dictionary<string, Texture2D>();
            sprDefault = Content.Load<Texture2D>("Deathmatch/Units/Default");
        }
    }
}
