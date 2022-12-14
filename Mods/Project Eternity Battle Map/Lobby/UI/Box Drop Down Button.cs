using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DropDownButton: IUIElement
    {
        public enum ButtonStates { Idle, Disabled, Hover, Open }

        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick();
        private readonly OnClick OnClickDelegate;

        private readonly Rectangle ButtonCollsionBox;
        private readonly SpriteFont fntText;
        private string SelectedItem;
        private string[] Choices;

        public ButtonStates ButtonState;

        public bool IsIdle { get { return ButtonState == ButtonStates.Idle  ; } }
        public bool IsDisabled { get { return ButtonState == ButtonStates.Disabled; } }
        public bool IsHover { get { return ButtonState == ButtonStates.Hover; } }
        public bool IsOpen { get { return ButtonState == ButtonStates.Open; } }

        public DropDownButton(Rectangle ButtonCollsionBox, SpriteFont fntText, string Text, string[] Choices, OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.ButtonCollsionBox = ButtonCollsionBox;
            this.fntText = fntText;
            this.SelectedItem = Text;
            this.Choices = Choices;
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;
        }

        public void ChangeChoices(string[] Choices)
        {
            this.Choices = Choices;
        }

        public void Update(GameTime gameTime)
        {
            if (IsDisabled)
            {
                return;
            }

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (ButtonState != ButtonStates.Open)
                {
                    if (ButtonCollsionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                    {
                        if (!IsHover && !IsOpen)
                        {
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
                        Unselect();
                    }
                }
                else
                {
                    if (MouseHelper.InputLeftButtonPressed())
                    {
                        if (MouseHelper.MouseStateCurrent.X >= ButtonCollsionBox.X
                            && MouseHelper.MouseStateCurrent.X <= ButtonCollsionBox.X + ButtonCollsionBox.Width
                            && MouseHelper.MouseStateCurrent.Y >= ButtonCollsionBox.Y + ButtonCollsionBox.Height
                            && MouseHelper.MouseStateCurrent.Y < ButtonCollsionBox.Y + ButtonCollsionBox.Height * (Choices.Length + 1))
                        {
                            SelectedItem = Choices[(MouseHelper.MouseStateCurrent.Y - ButtonCollsionBox.Y - ButtonCollsionBox.Height) / ButtonCollsionBox.Height];
                            if (OnClickDelegate != null)
                            {
                                OnClickDelegate();
                            }
                        }

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
            ButtonState = ButtonStates.Open;
        }

        public void Check()
        {
            ButtonState = ButtonStates.Open;
        }

        public void Unselect()
        {
            ButtonState = ButtonStates.Idle;
        }

        public void Uncheck()
        {
            ButtonState = ButtonStates.Idle;
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
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.White);
            }
            else if (ButtonState == ButtonStates.Disabled)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Black);
            }
            else if (ButtonState == ButtonStates.Open)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Black);

                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y + ButtonCollsionBox.Height),
                    ButtonCollsionBox.Width, ButtonCollsionBox.Height * Choices.Length, Color.White);

                for (int C = 0; C < Choices.Length; ++C)
                {
                    g.DrawStringVerticallyAligned(fntText, Choices[C], new Vector2(ButtonCollsionBox.X + 5,
                        ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2 + ButtonCollsionBox.Height + ButtonCollsionBox.Height * C), Color.White);

                    if (MouseHelper.MouseStateCurrent.X >= ButtonCollsionBox.X
                        && MouseHelper.MouseStateCurrent.X <= ButtonCollsionBox.X + ButtonCollsionBox.Width
                        && MouseHelper.MouseStateCurrent.Y >= ButtonCollsionBox.Y + ButtonCollsionBox.Height + ButtonCollsionBox.Height * C
                        && MouseHelper.MouseStateCurrent.Y < ButtonCollsionBox.Y + ButtonCollsionBox.Height + ButtonCollsionBox.Height * (C + 1))
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle(ButtonCollsionBox.X + 4,
                            ButtonCollsionBox.Y + 4 + ButtonCollsionBox.Height + ButtonCollsionBox.Height * C,
                            ButtonCollsionBox.Width - 8, ButtonCollsionBox.Height - 8), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                }

            }
            else if (ButtonState == ButtonStates.Hover)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Gray);
            }

            g.DrawStringVerticallyAligned(fntText, SelectedItem, new Vector2(ButtonCollsionBox.X + 5,
                ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2), Color.White);
        }
    }
}
