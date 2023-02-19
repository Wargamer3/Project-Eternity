using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class DefaultTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;
        private SpriteFont DefaultFont;

        public DefaultTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public DefaultTextProcessor(DynamicText Owner, SpriteFont DefaultFont)
        {
            this.Owner = Owner;
            this.DefaultFont = DefaultFont;
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            return new RegularText(Owner, string.Empty);
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
            if (DefaultFont != null)
            {
                Fonts.fntDefaultFont = DefaultFont;
            }
        }

        public override DynamicTextPart ParseText(string Text)
        {
            return new RegularText(Owner, Fonts, Text, string.Empty);
        }
    }
}
