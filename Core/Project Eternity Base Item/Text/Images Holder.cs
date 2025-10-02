
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class ImagesHolder
    {
        public Texture2D sprBoss;
        public ContentManager Content;

        public ImagesHolder(ContentManager Content)
        {
            sprBoss = Content.Load<Texture2D>("Deathmatch/Units/Default");
        }
    }
}
