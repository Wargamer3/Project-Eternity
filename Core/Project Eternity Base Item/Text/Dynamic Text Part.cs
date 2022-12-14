using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public abstract class DynamicTextPart
    {
        public readonly DynamicText Owner;
        public readonly string OriginalText;
        public readonly Dictionary<string, string> DicSubTag;

        public Vector2 Position;
        public float MaxWidth;
        public float MaxHeight;

        public DynamicTextPart(DynamicText Owner, string OriginalText, string Prefix)
        {
            this.Owner = Owner;
            DicSubTag = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(Prefix))
            {
                this.OriginalText = ProcessSubTags(OriginalText.Substring(Prefix.Length, OriginalText.Length - Prefix.Length));
            }
            else
            {
                this.OriginalText = OriginalText;
            }
        }

        public string ProcessSubTags(string Text)
        {
            while (Text[0] == '{')
            {
                int ReadIndex = 1;
                while (ReadIndex < Text.Length)
                {
                    string SubTag = Text.Substring(ReadIndex, 1);
                    ++ReadIndex;

                    if (SubTag == "}")
                    {
                        SubTag = Text.Substring(1, ReadIndex - 2);
                        string[] ArraySubTagPart = SubTag.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        DicSubTag.Add(ArraySubTagPart[0], ArraySubTagPart[1]);

                        Text = Text.Substring(ReadIndex, Text.Length - ReadIndex);
                        break;
                    }
                }
            }

            return Text;
        }

        protected float GetStartingXPositionOnLine(Vector2 ActivePosition)
        {
            Rectangle LineCollisionBox = new Rectangle((int)ActivePosition.X, (int)ActivePosition.Y, 1, (int)Owner.LineHeight);

            foreach (Rectangle ActiveObstacle in Owner.ListObstacle)
            {
                if (LineCollisionBox.Intersects(ActiveObstacle))
                {
                    return ActiveObstacle.Right;
                }
            }

            return ActivePosition.X;
        }

        protected float GetRemainingSpaceOnLine(Vector2 ActivePosition)
        {
            Rectangle LineCollisionBox = new Rectangle((int)ActivePosition.X, (int)ActivePosition.Y, (int)Owner.TextMaxWidthInPixel, (int)Owner.LineHeight);

            foreach (Rectangle ActiveObstacle in Owner.ListObstacle)
            {
                if (LineCollisionBox.Intersects(ActiveObstacle))
                {
                    return ActiveObstacle.X - ActivePosition.X;
                }
            }

            return MaxWidth - ActivePosition.X;
        }

        //Retun end position
        public abstract Vector2 UpdatePosition();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(CustomSpriteBatch g, Vector2 Offset);
    }
}
