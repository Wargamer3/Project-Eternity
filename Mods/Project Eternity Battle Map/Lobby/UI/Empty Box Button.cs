using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EmptyBoxButton : IUIElement
    {
        public enum ButtonStates { Idle, Disabled, Hover, Checked }

        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick();
        private readonly OnClick OnClickDelegate;

        private readonly Rectangle ButtonCollsionBox;
        private readonly string Text;
        private SpriteFont fntText;

        private double HoverProgression;
        public bool CanBeChecked;

        public ButtonStates ButtonState;

        public bool IsIdle { get { return ButtonState == ButtonStates.Idle  ; } }
        public bool IsDisabled { get { return ButtonState == ButtonStates.Disabled; } }
        public bool IsHover { get { return ButtonState == ButtonStates.Hover; } }
        public bool IsChecked { get { return ButtonState == ButtonStates.Checked; } }

        public EmptyBoxButton(Rectangle ButtonCollsionBox, SpriteFont fntText, string Text, OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.ButtonCollsionBox = ButtonCollsionBox;
            this.fntText = fntText;
            this.Text = Text;
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;
        }

        public void Update(GameTime gameTime)
        {
            if (IsDisabled)
            {
                return;
            }

            if (ButtonState == ButtonStates.Hover)
            {
                HoverProgression += gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (ButtonCollsionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    if (!IsHover && ((CanBeChecked && !IsChecked) || (!CanBeChecked)))
                    {
                        HoverProgression = 0;

                        Hover();
                    }

                    if (MouseHelper.InputLeftButtonPressed())
                    {
                        Select();
                        if (OnClickDelegate != null)
                        {
                            OnClickDelegate();
                        }
                    }
                }
                else
                {
                    if (!CanBeChecked || !IsChecked)
                    {
                        Unselect();
                    }
                }
            }
        }

        public void Hover()
        {
            ButtonState = ButtonStates.Hover;
            if (OnOverDelegate != null)
            {
                OnOverDelegate();
            }
        }

        public void Select()
        {
            ButtonState = ButtonStates.Checked;
        }

        public void Unselect()
        {
            if (!IsDisabled)
            {
                ButtonState = ButtonStates.Idle;
            }
        }

        public void Disable()
        {
            ButtonState = ButtonStates.Disabled;
        }

        public void Enable()
        {
            ButtonState = ButtonStates.Idle;
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (ButtonState == ButtonStates.Idle)
            {
                Vector2 TextPosition = new Vector2(ButtonCollsionBox.X + ButtonCollsionBox.Width / 2,
                    ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2);

                Vector2 TextSize = fntText.MeasureString(Text);

                g.Draw(GameScreen.sprPixel, new Rectangle((int)(TextPosition.X - TextSize.X / 2), (int)(TextPosition.Y - TextSize.Y / 2), (int)TextSize.X, (int)TextSize.Y), Lobby.BackgroundColor);

                Color NewBackgroundColor = Color.FromNonPremultiplied((int)(Lobby.BackgroundColor.R * 0.9f), (int)(Lobby.BackgroundColor.G * 0.9f), (int)(Lobby.BackgroundColor.B * 0.9f), 125);
                g.Draw(GameScreen.sprPixel, ButtonCollsionBox, NewBackgroundColor);

                GameScreen.DrawEmptyBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, 2, 0);

                g.DrawString(fntText, Text, TextPosition, Color.White, 0, new Vector2((int)TextSize.X / 2, (int)TextSize.Y / 2), 1, SpriteEffects.None, 0);
            }
            else if (ButtonState == ButtonStates.Disabled)
            {
                GameScreen.DrawEmptyBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height);
                g.Draw(GameScreen.sprPixel, new Rectangle(), Color.FromNonPremultiplied(127, 127, 127, 127));

                g.DrawStringCenteredBackground(fntText, Text, new Vector2(ButtonCollsionBox.X + ButtonCollsionBox.Width / 2,
                    ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2), Color.White, GameScreen.sprPixel, Lobby.BackgroundColor);
            }
            else if (ButtonState == ButtonStates.Checked)
            {
                Vector2 TextPosition = new Vector2(ButtonCollsionBox.X + ButtonCollsionBox.Width / 2,
                    ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2);

                Vector2 TextSize = fntText.MeasureString(Text);

                g.Draw(GameScreen.sprPixel, new Rectangle((int)(TextPosition.X - TextSize.X / 2), (int)(TextPosition.Y - TextSize.Y / 2), (int)TextSize.X, (int)TextSize.Y), Lobby.BackgroundColor);

                Color NewBackgroundColor = Color.FromNonPremultiplied((int)(Lobby.BackgroundColor.R * 0.9f), (int)(Lobby.BackgroundColor.G * 0.9f), (int)(Lobby.BackgroundColor.B * 0.9f), 200);
                g.Draw(GameScreen.sprPixel, ButtonCollsionBox, NewBackgroundColor);

                GameScreen.DrawEmptyBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, 5, 0);

                g.DrawString(fntText, Text, TextPosition, Color.White, 0, new Vector2((int)TextSize.X / 2, (int)TextSize.Y / 2), 1, SpriteEffects.None, 0);
            }
            else if (ButtonState == ButtonStates.Hover)
            {
                Vector2 TextPosition = new Vector2(ButtonCollsionBox.X + ButtonCollsionBox.Width / 2,
                    ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2);

                Vector2 TextSize = fntText.MeasureString(Text);

                g.Draw(GameScreen.sprPixel, new Rectangle((int)(TextPosition.X - TextSize.X / 2), (int)(TextPosition.Y - TextSize.Y / 2), (int)TextSize.X, (int)TextSize.Y), Lobby.BackgroundColor);

                Color NewBackgroundColor = Color.FromNonPremultiplied((int)(Lobby.BackgroundColor.R * 0.9f), (int)(Lobby.BackgroundColor.G * 0.9f), (int)(Lobby.BackgroundColor.B * 0.9f), 160);
                g.Draw(GameScreen.sprPixel, ButtonCollsionBox, NewBackgroundColor);

                GameScreen.DrawEmptyBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, 5, HoverProgression);

                g.DrawString(fntText, Text, TextPosition, Color.White, 0, new Vector2((int)TextSize.X / 2, (int)TextSize.Y / 2), 1, SpriteEffects.None, 0);
            }
        }
    }
}
