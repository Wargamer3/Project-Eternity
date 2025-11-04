using System;
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
                sprIcon = Images.sprDefault;
            }
        }

        public override void OnTextRead(string TextRead)
        {
            if (Images != null)
            {
                if (!Images.DicSprite.TryGetValue(TextRead, out sprIcon))
                {
                    sprIcon = Images.sprDefault;
                }
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

            if (DicSubTag.ContainsKey("Wave"))
            {
                Wave = true;
            }
            else if (Parent != null)
            {
                Wave = Parent.Wave;
            }
            else
            {
                Wave = false;
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
            float YOffset = 0;
            if (Wave)
            {
                YOffset = (float)Math.Sin(DynamicText.CurrentDrawnCharacterIndex + (DynamicText.AnimationProgression++ / 10f)) * 10;
            }

            g.Draw(sprIcon, new Vector2(ImagePosition.X + Offset.X, ImagePosition.Y + Offset.Y + YOffset), Color.White);
        }
    }
}
