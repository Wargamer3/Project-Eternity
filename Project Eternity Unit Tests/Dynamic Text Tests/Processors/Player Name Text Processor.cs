using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class PlayerNameTextProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;

        public PlayerNameTextProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Player:"))
            {
                return new PlayerNamePart(Owner, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Player:"))
            {
                return new PlayerNamePart(Owner, "Player:");
            }

            return null;
        }
    }
}
