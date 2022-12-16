using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class ImagelessIconProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public ImagelessIconProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Icon:"))
            {
                return new ImagelessIconPart(Owner, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Icon:"))
            {
                return new ImagelessIconPart(Owner, Text);
            }
            return null;
        }
    }
}
