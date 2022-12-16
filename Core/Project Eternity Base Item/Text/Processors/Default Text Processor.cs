using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Item
{
    public class DefaultTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;

        public DefaultTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            return new RegularText(Owner, string.Empty);
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
        }

        public override DynamicTextPart ParseText(string Text)
        {
            return new RegularText(Owner, Fonts, Text, string.Empty);
        }
    }
}
