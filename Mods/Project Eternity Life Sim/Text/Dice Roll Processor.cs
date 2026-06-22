using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class DiceRollPart : DynamicTextPart
    {
        public DiceRollPart(DynamicText Owner, FontsHolder Fonts, string OriginalText)
             : base(Owner, OriginalText, "DiceRoll:")
        {
        }

        public override void OnTextRead(string TextRead)
        {
            OriginalText = TextRead;
        }

        public override Vector2 UpdatePosition()
        {
            ReadTags();

            Vector2 ActivePosition = Position;

            return new Vector2(0, ActivePosition.Y + Owner.LineHeight + 40);
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

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            GameScreen.DrawBox(g, Position + Offset, (int)MaxWidth - 20, 50, Color.White);
            TextHelper.DrawTextMiddleAligned(g, OriginalText, Position + new Vector2(Offset.X + MaxWidth / 2, Offset.Y + 15), Color.White);
        }
    }

    public class DiceRollProcessor : DynamicTextProcessor
    {
        private readonly DynamicText Owner;
        private FontsHolder Fonts;
        private SpriteFont DefaultFont;

        public DiceRollProcessor(DynamicText Owner)
        {
            this.Owner = Owner;
        }

        public DiceRollProcessor(DynamicText Owner, SpriteFont DefaultFont)
        {
            this.Owner = Owner;
            this.DefaultFont = DefaultFont;
        }

        public override void Load(ContentManager Content)
        {
            Fonts = new FontsHolder(Content);
            if (DefaultFont != null)
            {
                Fonts.fntDefaultFont = DefaultFont;
            }
        }

        public override DynamicTextPart GetTextObject(string Prefix)
        {
            if (Prefix.StartsWith("DiceRoll:"))
            {
                return new DiceRollPart(Owner, Fonts, string.Empty);
            }

            return null;
        }

        public override DynamicTextPart ParseText(string Text)
        {
            if (Text.StartsWith("DiceRoll:"))
            {
                return new DiceRollPart(Owner, Fonts, Text);
            }

            return null;
        }
    }
}
