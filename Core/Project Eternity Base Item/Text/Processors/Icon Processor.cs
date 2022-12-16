using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Item
{
    public class IconProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private ImagesHolder Images;

        public IconProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public override void Load(ContentManager Content)
        {
            Images = new ImagesHolder(Content);
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("Icon:"))
            {
                return new IconPart(Owner, Images, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("Icon:"))
            {
                return new IconPart(Owner, Images, Text);
            }

            return null;
        }
    }
}
