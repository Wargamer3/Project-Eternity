using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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

        public void PreloadImage(string Tag, string Path)
        {
            Images.DicSprite.Add(Tag, Images.Content.Load<Texture2D>(Path));
        }

        public void PreloadImage(string Tag, Texture2D Sprite)
        {
            Images.DicSprite.Add(Tag, Sprite);
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
