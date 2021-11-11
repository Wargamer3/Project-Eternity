using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BoxButton : IUIElement
    {
        public enum ButtonStates { Idle, Disabled, Hover, Checked }

        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick();
        private readonly OnClick OnClickDelegate;

        private readonly Rectangle ButtonCollsionBox;
        private readonly string Text;
        private SpriteFont fntText;

        public bool CanBeChecked;

        public ButtonStates ButtonState;

        public bool IsIdle { get { return ButtonState == ButtonStates.Idle  ; } }
        public bool IsDisabled { get { return ButtonState == ButtonStates.Disabled; } }
        public bool IsHover { get { return ButtonState == ButtonStates.Hover; } }
        public bool IsChecked { get { return ButtonState == ButtonStates.Checked; } }

        public BoxButton(Rectangle ButtonCollsionBox, SpriteFont fntText, string Text, OnOver OnOverDelegate, OnClick OnClickDelegate)
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

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (ButtonCollsionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    if (!IsHover && ((CanBeChecked && !IsChecked) || (!CanBeChecked)))
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
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.White);
            }
            else if (ButtonState == ButtonStates.Disabled)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Black);
            }
            else if (ButtonState == ButtonStates.Checked)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Black);
            }
            else if (ButtonState == ButtonStates.Hover)
            {
                GameScreen.DrawBox(g, new Vector2(ButtonCollsionBox.X, ButtonCollsionBox.Y), ButtonCollsionBox.Width, ButtonCollsionBox.Height, Color.Gray);
            }

            g.DrawStringCentered(fntText, Text, new Vector2(ButtonCollsionBox.X + ButtonCollsionBox.Width / 2,
                ButtonCollsionBox.Y + ButtonCollsionBox.Height / 2), Color.White);
        }
    }
}
