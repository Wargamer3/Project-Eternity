using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class RegularTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;
        private SpriteFont DefaultFont;

        public RegularTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public RegularTextProcessor(DynamicText Owner, SpriteFont DefaultFont)
        {
            this.Owner = Owner;
            this.DefaultFont = DefaultFont;
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
            if (DefaultFont != null)
            {
                Fonts.fntDefaultFont = DefaultFont;
            }
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Text:"))
            {
                return new RegularText(Owner, Fonts, string.Empty, "Text:");
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Text:"))
            {
                return new RegularText(Owner, Fonts, Text, "Text:");
            }

            return null;
        }
    }
}
