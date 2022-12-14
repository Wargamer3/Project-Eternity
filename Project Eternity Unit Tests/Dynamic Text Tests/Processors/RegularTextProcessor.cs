using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class RegularTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public RegularTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
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
