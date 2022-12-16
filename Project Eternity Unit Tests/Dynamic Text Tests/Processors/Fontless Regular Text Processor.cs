using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class FontlessRegularTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public FontlessRegularTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Text:"))
            {
                return new FontlessText(Owner, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Text:"))
            {
                return new FontlessText(Owner, Text, "Text:");
            }

            return null;
        }
    }
}
