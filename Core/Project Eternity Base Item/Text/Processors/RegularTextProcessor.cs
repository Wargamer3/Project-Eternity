using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Item
{
    public class RegularTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;

        public RegularTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
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
