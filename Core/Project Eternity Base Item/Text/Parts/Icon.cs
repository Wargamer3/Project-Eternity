using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class IconPart : DynamicTextPart
    {
        public Texture2D sprIcon;
        private ImagesHolder Images;
        public Vector2 ImagePosition;

        public IconPart(DynamicText Owner, ImagesHolder Images, string OriginalText)
             : base(Owner, OriginalText, "Icon:")
        {
            this.Images = Images;
            if (Images != null)
            {
                sprIcon = Images.sprBoss;
            }
        }

        public override void OnTextRead(string TextRead)
        {
            if (Images != null)
            {
                sprIcon = Images.sprBoss;
            }
        }

        public override Vector2 UpdatePosition()
        {
            ReadTags();

            Vector2 ActivePosition = Position;

            float ImageWidth = GetImageSize();
            ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
            ImagePosition = ActivePosition;
            float RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);

            if (ImageWidth > Owner.TextMaxWidthInPixel)
            {
                return new Vector2(Position.X + ImageWidth, Position.Y);
            }

            while (RemainingSpaceOnLine < ImageWidth)
            {
                ActivePosition.Y += Owner.LineHeight;
                ActivePosition.X = 0;
                ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                ImagePosition = ActivePosition;
                RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);
            }

            return new Vector2(ActivePosition.X + ImageWidth, ActivePosition.Y);
        }

        private void ReadTags()
        {
            if (DicSubTag.ContainsKey("MaxWidth"))
            {
                MaxWidth = float.Parse(DicSubTag["MaxWidth"]);
            }
            else if (Parent != null)
            {
                MaxWidth = Parent.MaxWidth;
            }
            else
            {
                MaxWidth = Owner.TextMaxWidthInPixel;
            }
        }

        public virtual float GetImageSize()
        {
            return sprIcon.Width;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            g.Draw(sprIcon, ImagePosition, Color.White);
        }
    }
}
