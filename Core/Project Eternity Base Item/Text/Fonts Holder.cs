using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class FontsHolder
    {
        public SpriteFont fntDefaultFont;
        public SpriteFont fntArial12;
        public SpriteFont fntArial16;
        public ContentManager Content;

        public FontsHolder(ContentManager Content)
        {
            fntDefaultFont = fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntArial16 = Content.Load<SpriteFont>("Fonts/Arial16");
        }
    }
}
