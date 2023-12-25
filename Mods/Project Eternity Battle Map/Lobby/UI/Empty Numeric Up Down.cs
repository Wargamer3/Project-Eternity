using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EmptyNumericUpDown : TextInput
    {
        private float ButtonX => TextInputPosition.X + TextInputSize.X - 15;
        private float TopButtonY => TextInputPosition.Y - 5;
        private float BottomButtonY => TextInputPosition.Y + TextInputSize.Y / 2;
        private int ButtonWidth => 20;
        private int ButtonHeight => (int)TextInputSize.Y / 2 + 5;

        public EmptyNumericUpDown(SpriteFont fntText, Texture2D sprPixel, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, OnConfirmDelegate OnConfirm = null)
            : base(fntText, sprPixel, sprCursor, TextInputPosition, TextInputSize, OnConfirm, true)
        {
            SetText("0");
        }

        public EmptyNumericUpDown(SpriteFont fntText, Texture2D sprPixel, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, OnConfirmDelegate OnConfirm, string DefaultText)
            : base(fntText, sprPixel, sprCursor, TextInputPosition, TextInputSize, OnConfirm, true)
        {
            SetText(DefaultText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
            GameScreen.DrawEmptyBox(g, new Vector2(TextInputPosition.X - 5, TextInputPosition.Y - 5), (int)TextInputSize.X + 10, (int)TextInputSize.Y + 10);
            GameScreen.DrawBox(g, new Vector2(ButtonX, TopButtonY), ButtonWidth, ButtonHeight, Color.White);
            GameScreen.DrawBox(g, new Vector2(ButtonX, BottomButtonY), ButtonWidth, ButtonHeight, Color.White);

            if (MouseHelper.MouseStateCurrent.X >= ButtonX && MouseHelper.MouseStateCurrent.Y >= TopButtonY
                && MouseHelper.MouseStateCurrent.X <= ButtonX + ButtonWidth && MouseHelper.MouseStateCurrent.Y <= TopButtonY + ButtonHeight)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)ButtonX, (int)TopButtonY, ButtonWidth, ButtonHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
            else if (MouseHelper.MouseStateCurrent.X >= ButtonX && MouseHelper.MouseStateCurrent.Y >= BottomButtonY
                && MouseHelper.MouseStateCurrent.X <= ButtonX + ButtonWidth && MouseHelper.MouseStateCurrent.Y <= BottomButtonY + ButtonHeight)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle((int)ButtonX, (int)BottomButtonY, ButtonWidth, ButtonHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
            }

            base.Draw(g);
        }
    }
}
