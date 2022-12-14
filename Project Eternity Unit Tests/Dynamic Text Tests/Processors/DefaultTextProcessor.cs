using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class DefaultTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public DefaultTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override DynamicTextPart ParseText(string Text)
        {
            return new FontlessText(Owner, Text, string.Empty);
        }
    }
}
