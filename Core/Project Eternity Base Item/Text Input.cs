using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class TextInput
    {
        public delegate void OnConfirmDelegate(string InputMessage);

        private Texture2D sprCursor;
        private SpriteFont fntText;

        private Vector2 TextInputPosition;
        private Vector2 TextInputSize;

        private OnConfirmDelegate OnConfirm;

        private bool IsActive;
        private bool DigitsOnly;

        public string Text { get; private set; }
        private string TextVisible;//Croped message that fit in the text box.

        private int MessageStartIndex;//Index of the starting letter of the message to draw.
        private int MessageCursorIndex;//Index of the typing cursor in the message.
        private float MessageCursorPosition;//X position of the cursor, based on its index.
        private double BlinkingTimer;
        private bool IsCursorVisible;

        private Keys[] LastPressedKeys;

        public TextInput(SpriteFont fntText, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, OnConfirmDelegate OnConfirm = null, bool DigitsOnly = false)
        {
            this.fntText = fntText;
            this.sprCursor = sprCursor;
            this.TextInputPosition = TextInputPosition;
            this.TextInputSize = TextInputSize;
            this.DigitsOnly = DigitsOnly;

            if (OnConfirm == null)
            {
                this.OnConfirm = EreaseText;
            }
            else
            {
                this.OnConfirm = OnConfirm;
            }

            Text = string.Empty;
            TextVisible = string.Empty;
            LastPressedKeys = new Keys[0];
            BlinkingTimer = 0d;
            IsCursorVisible = true;
        }

        public void Update(GameTime gameTime)
        {
            if (MouseHelper.MouseStateCurrent.X >= TextInputPosition.X && MouseHelper.MouseStateCurrent.Y >= TextInputPosition.Y
                && MouseHelper.MouseStateCurrent.X <= TextInputPosition.X + TextInputSize.X && MouseHelper.MouseStateCurrent.Y <= TextInputPosition.Y + TextInputSize.Y
                && MouseHelper.InputLeftButtonPressed())
            {
                IsActive = true;
                IsCursorVisible = true;
                BlinkingTimer = 0d;
            }
            else if (MouseHelper.InputLeftButtonPressed())
            {
                IsActive = false;
            }

            if (IsActive)
            {
                BlinkingTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (BlinkingTimer > 1d)
                {
                    BlinkingTimer -= 1d;
                    IsCursorVisible = !IsCursorVisible;
                }

                ReadKeys();
            }
        }

        public void SetText(string NewText)
        {
            Text = NewText;
            TextVisible = Text;
            MessageCursorIndex = Text.Length;
            UpdateMessengerCursor();
        }

        private void ReadKeys()
        {
            Keys[] PressedKeys = KeyboardHelper.KeyPressed();

            foreach (Keys ActiveKey in PressedKeys)
            {
                if (!LastPressedKeys.Contains(ActiveKey))
                {
                    switch (ActiveKey)
                    {
                        case Keys.LeftShift:
                        case Keys.RightShift:
                        case Keys.LeftControl:
                        case Keys.RightControl:
                        case Keys.LeftAlt:
                        case Keys.RightAlt:
                        case Keys.PrintScreen:
                        case Keys.Up:
                        case Keys.Down:
                        case Keys.Escape:
                            break;

                        case Keys.OemComma:
                            Text += "'";
                            TextVisible = Text;
                            MessageCursorIndex++;
                            UpdateMessengerCursor();
                            break;

                        case Keys.Enter:
                            OnConfirm(Text);
                            break;

                        case Keys.Right:
                            if (MessageCursorIndex < Text.Length)
                            {
                                MessageCursorIndex++;
                                UpdateMessengerCursor();
                            }
                            break;

                        case Keys.Left:
                            if (MessageCursorIndex > 0)
                            {
                                MessageCursorIndex--;
                                UpdateMessengerCursor();
                            }
                            break;

                        case Keys.Space:
                            Text += " ";
                            TextVisible = Text;
                            MessageCursorIndex++;
                            UpdateMessengerCursor();
                            break;

                        case Keys.Back:
                            if (MessageCursorIndex > 0)
                            {
                                Text = Text.Remove(--MessageCursorIndex, 1);
                                UpdateMessengerCursor();
                            }
                            break;

                        case Keys.Delete:
                            if (MessageCursorIndex < Text.Length)
                            {
                                Text = Text.Remove(MessageCursorIndex, 1);
                                UpdateMessengerCursor();
                            }
                            break;

                        case Keys.D0:
                        case Keys.D1:
                        case Keys.D2:
                        case Keys.D3:
                        case Keys.D4:
                        case Keys.D5:
                        case Keys.D6:
                        case Keys.D7:
                        case Keys.D8:
                        case Keys.D9:
                        case Keys.NumPad0:
                        case Keys.NumPad1:
                        case Keys.NumPad2:
                        case Keys.NumPad3:
                        case Keys.NumPad4:
                        case Keys.NumPad5:
                        case Keys.NumPad6:
                        case Keys.NumPad7:
                        case Keys.NumPad8:
                        case Keys.NumPad9:
                            Text += ActiveKey.ToString().Replace("D", "").Replace("NumPad", "");
                            TextVisible = Text;
                            MessageCursorIndex++;
                            UpdateMessengerCursor();
                            break;

                        default:
                            if (DigitsOnly)
                            {
                                break;
                            }

                            if (KeyboardHelper.KeyHold(Keys.LeftShift) || KeyboardHelper.KeyHold(Keys.RightShift))
                            {
                                Text += ActiveKey.ToString();
                            }
                            else
                            {
                                Text += ActiveKey.ToString().ToLower();
                            }
                            TextVisible = Text;
                            MessageCursorIndex++;
                            UpdateMessengerCursor();
                            break;
                    }
                }
            }

            LastPressedKeys = PressedKeys;
        }

        private void UpdateMessengerCursor()
        {
            if (MessageCursorIndex < MessageStartIndex)
            {
                while (MessageCursorIndex < MessageStartIndex)
                {
                    MessageStartIndex--;
                }

                TextVisible = Text.Substring(MessageStartIndex);

                while (fntText.MeasureString(TextVisible).X > TextInputSize.X)
                {
                    TextVisible = TextVisible.Substring(0, TextVisible.Length - 1);
                }
            }

            MessageCursorPosition = fntText.MeasureString(Text.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;

            //Crop the text so it fits in the message box.
            if (MessageCursorPosition > TextInputSize.X)
            {
                TextVisible = Text.Substring(MessageStartIndex);

                while ((TextVisible.Length + MessageStartIndex) > MessageCursorIndex)
                {
                    TextVisible = TextVisible.Substring(0, TextVisible.Length - 1);
                }

                while (fntText.MeasureString(TextVisible).X > TextInputSize.X)
                {
                    TextVisible = TextVisible.Substring(1, TextVisible.Length - 1);
                    MessageStartIndex++;
                }

                //Update the real cursor position.
                if (MessageCursorIndex - MessageStartIndex >= TextVisible.Length)
                {
                    MessageCursorPosition = fntText.MeasureString(TextVisible).X;
                }
            }
            else
            {
                TextVisible = Text.Substring(MessageStartIndex);
                //Get the real cursor position.
                MessageCursorPosition = fntText.MeasureString(TextVisible.Substring(0, MessageCursorIndex - MessageStartIndex)).X;
            }
        }

        private void EreaseText(string InputMessage)
        {
            Text = "";
            TextVisible = Text;
            MessageCursorIndex = 0;
            UpdateMessengerCursor();
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (IsActive && IsCursorVisible)
            {
                g.Draw(sprCursor, new Rectangle((int)(TextInputPosition.X + MessageCursorPosition), (int)TextInputPosition.Y + 1, 1, (int)TextInputSize.Y - 2), Color.Black);
            }

            g.DrawString(fntText, TextVisible, new Vector2(TextInputPosition.X, TextInputPosition.Y), Color.Black);
        }
    }
}
