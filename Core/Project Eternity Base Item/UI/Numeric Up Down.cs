using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class NumericUpDown : TextInput
    {
        private float ButtonX => TextInputPosition.X + TextInputSize.X - 15;
        private float TopButtonY => TextInputPosition.Y - 5;
        private float BottomButtonY => TextInputPosition.Y + TextInputSize.Y / 2;
        private int ButtonWidth => 20;
        private int ButtonHeight => (int)TextInputSize.Y / 2 + 5;

        public TextButton ButtonUp;
        public TextButton ButtonDown;

        public NumericUpDown(SpriteFont fntText, Texture2D sprPixel, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, float Ratio, OnConfirmDelegate OnConfirm = null)
            : base(fntText, sprPixel, sprCursor, TextInputPosition, TextInputSize, OnConfirm, true)
        {
            SetText("0");
        }

        public NumericUpDown(SpriteFont fntText, Texture2D sprPixel, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, OnConfirmDelegate OnConfirm, string DefaultText)
            : base(fntText, sprPixel, sprCursor, TextInputPosition, TextInputSize, OnConfirm, true)
        {
            SetText(DefaultText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ButtonUp.Update(gameTime);
            ButtonDown.Update(gameTime);

            if (MouseHelper.InputLeftButtonPressed())
            {
                if (MouseHelper.MouseStateCurrent.X >= ButtonX && MouseHelper.MouseStateCurrent.Y >= TopButtonY
                && MouseHelper.MouseStateCurrent.X <= ButtonX + ButtonWidth && MouseHelper.MouseStateCurrent.Y <= TopButtonY + ButtonHeight)
                {
                    int InternalValue = int.Parse(Text);
                    SetText(MathHelper.Min(int.MaxValue, InternalValue + 1).ToString());
                    OnConfirm(this, Text);
                }
                else if (MouseHelper.MouseStateCurrent.X >= ButtonX && MouseHelper.MouseStateCurrent.Y >= BottomButtonY
                    && MouseHelper.MouseStateCurrent.X <= ButtonX + ButtonWidth && MouseHelper.MouseStateCurrent.Y <= BottomButtonY + ButtonHeight)
                {
                    int InternalValue = int.Parse(Text);
                    SetText(MathHelper.Max(int.MinValue, InternalValue - 1).ToString());
                    OnConfirm(this, Text);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            ButtonUp.Draw(g);
            ButtonDown.Draw(g);
        }
    }
}
