using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class FontlessDefaultTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public FontlessDefaultTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            return new FontlessText(Owner, string.Empty);
        }

        public override DynamicTextPart ParseText(string Text)
        {
            return new FontlessText(Owner, Text, string.Empty);
        }
    }
}
