using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Item
{
    public class IconProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private ImagesHolder Fonts;

        public IconProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new ImagesHolder(Content);
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Icon:"))
            {
                return new IconPart(Owner, Fonts, Text);
            }

            return null;
        }
    }
}
