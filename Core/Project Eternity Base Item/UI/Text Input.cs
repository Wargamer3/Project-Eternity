using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class TextInput : IUIElement
    {
        public delegate void OnConfirmDelegate(string InputMessage);

        private Texture2D sprPixel;
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
        private int MessageSelectionCursorIndex;
        private float MessageSelectionCursorPosition;
        private double BlinkingTimer;
        private bool IsCursorVisible;

        private Keys[] LastPressedKeys;

        public TextInput(SpriteFont fntText, Texture2D sprPixel, Texture2D sprCursor, Vector2 TextInputPosition, Vector2 TextInputSize, OnConfirmDelegate OnConfirm = null, bool DigitsOnly = false)
        {
            this.fntText = fntText;
            this.sprPixel = sprPixel;
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
                && MouseHelper.MouseStateCurrent.X <= TextInputPosition.X + TextInputSize.X && MouseHelper.MouseStateCurrent.Y <= TextInputPosition.Y + TextInputSize.Y)
            {
                if (MouseHelper.InputLeftButtonPressed())
                {
                    Select();

                    int MouseCursorPosition = GetMouseCursorPosition();
                    MessageCursorIndex = MouseCursorPosition;
                    UpdateMessengerCursor();
                    MessageSelectionCursorIndex = GetMouseCursorPosition();
                    MessageSelectionCursorPosition = GetCursorPosition(MessageSelectionCursorIndex);
                }
                else if (MouseHelper.MouseMoved() && MouseHelper.InputLeftButtonHold())
                {
                    MessageCursorIndex = GetMouseCursorPosition();

                    if (MessageCursorIndex >= TextVisible.Length && MessageStartIndex + MessageCursorIndex + 1 < Text.Length)
                    {
                        ++MessageCursorIndex;
                        ++MessageStartIndex;
                    }
                    else if (MessageCursorIndex <= MessageStartIndex && MessageStartIndex - 1 >= 0)
                    {
                        --MessageCursorIndex;
                        --MessageStartIndex;
                    }

                    MessageCursorPosition = GetCursorPosition(MessageCursorIndex);
                    MessageSelectionCursorPosition = GetCursorPosition(MessageSelectionCursorIndex);
                    UpdateMessengerCursor(false);
                }
            }
            else if (MouseHelper.InputLeftButtonPressed())
            {
                Unselect();
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

        private int GetMouseCursorPosition()
        {
            float MouseX = MouseHelper.MouseStateCurrent.X - TextInputPosition.X;
            float LastTextX = 0;

            for (int i = 1; i <= TextVisible.Length; ++i)
            {
                float TextX = fntText.MeasureString(TextVisible.Substring(0, i)).X;

                if (MouseX <= TextX)
                {
                    float SelectedCharacterSize = TextX - LastTextX;

                    if (MouseX <= TextX - SelectedCharacterSize / 2)
                    {
                        return MessageStartIndex + i - 1;
                    }
                    else
                    {
                        return MessageStartIndex + i;
                    }
                }

                LastTextX = TextX;
            }

            return TextVisible.Length;
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
                            AddText("'");
                            break;

                        case Keys.Enter:
                            if (!string.IsNullOrEmpty(Text))
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
                            AddText(" ");
                            break;

                        case Keys.Back:
                            if (MessageSelectionCursorIndex != MessageCursorIndex)
                            {
                                DeleteSelection();
                            }
                            else
                            {
                                if (MessageCursorIndex > 0)
                                {
                                    Text = Text.Remove(--MessageCursorIndex, 1);
                                }
                            }
                            UpdateMessengerCursor();
                            break;

                        case Keys.Delete:
                            if (MessageSelectionCursorIndex != MessageCursorIndex)
                            {
                                DeleteSelection();
                            }
                            else
                            {
                                if (MessageCursorIndex < Text.Length)
                                {
                                    Text = Text.Remove(MessageCursorIndex, 1);
                                }
                            }
                            UpdateMessengerCursor();
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
                            AddText(ActiveKey.ToString().Replace("D", "").Replace("NumPad", ""));
                            break;

                        case Keys.Divide:
                            AddText("/");
                            break;

                        default:
                            if (DigitsOnly)
                            {
                                break;
                            }

                            if (KeyboardHelper.KeyHold(Keys.LeftShift) || KeyboardHelper.KeyHold(Keys.RightShift))
                            {
                                AddText(ActiveKey.ToString());
                            }
                            else
                            {
                                AddText(ActiveKey.ToString().ToLower());
                            }
                            break;
                    }
                }
            }

            if (PressedKeys.Length > 0)
            {
                IsCursorVisible = true;
                BlinkingTimer = 0d;
            }

            LastPressedKeys = PressedKeys;
        }

        private void AddText(string TextToAdd)
        {
            if (MessageCursorIndex == Text.Length)
            {
                Text += TextToAdd;
            }
            else
            {
                Text = Text.Insert(MessageCursorIndex, TextToAdd);
            }

            TextVisible = Text;
            MessageCursorIndex += TextToAdd.Length;
            UpdateMessengerCursor();
        }

        private void DeleteSelection()
        {
            if (MessageSelectionCursorIndex > MessageCursorIndex)
            {
                Text = Text.Remove(MessageCursorIndex, MessageSelectionCursorIndex - MessageCursorIndex);
            }
            else
            {
                Text = Text.Remove(MessageSelectionCursorIndex, MessageCursorIndex - MessageSelectionCursorIndex);
                MessageCursorIndex -= MessageCursorIndex - MessageSelectionCursorIndex;
            }
        }

        private void UpdateMessengerCursor(bool UpdateCursor = true)
        {
            if (MessageCursorIndex < MessageStartIndex)
            {
                while (MessageCursorIndex < MessageStartIndex)
                {
                    MessageStartIndex--;
                }
            }

            float CursorPosition = fntText.MeasureString(Text.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;

            while (CursorPosition > TextInputSize.X)
            {
                ++MessageStartIndex;
                CursorPosition = fntText.MeasureString(Text.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;
            }

            TextVisible = Text.Substring(MessageStartIndex);

            while (fntText.MeasureString(TextVisible).X > TextInputSize.X)
            {
                TextVisible = TextVisible.Substring(0, TextVisible.Length - 1);
            }

            if (UpdateCursor)
            {
                MessageCursorPosition = GetCursorPosition(MessageCursorIndex);
                MessageSelectionCursorIndex = MessageCursorIndex;
            }
        }

        private float GetCursorPosition(int MessageCursorIndex)
        {
            if (MessageCursorIndex - MessageStartIndex < 0)
            {
                return 0;
            }

            float MessageCursorPosition = fntText.MeasureString(Text.Substring(MessageStartIndex, MessageCursorIndex - MessageStartIndex)).X;

            if (MessageCursorPosition > TextInputSize.X)
            {
                if (MessageCursorIndex - MessageStartIndex >= TextVisible.Length)
                {
                    MessageCursorPosition =  fntText.MeasureString(TextVisible).X;
                }
            }
            else
            {
                if (MessageCursorIndex - MessageStartIndex > TextVisible.Length)
                {
                    MessageCursorPosition = fntText.MeasureString(TextVisible.Substring(0, TextVisible.Length)).X;
                }
                else
                {
                    MessageCursorPosition = fntText.MeasureString(TextVisible.Substring(0, MessageCursorIndex - MessageStartIndex)).X;
                }
            }

            return MessageCursorPosition;
        }

        private void EreaseText(string InputMessage)
        {
            Text = "";
            TextVisible = Text;
            MessageCursorIndex = 0;
            UpdateMessengerCursor();
        }

        public void Select()
        {
            IsActive = true;
            IsCursorVisible = true;
            BlinkingTimer = 0d;
            MessageSelectionCursorIndex = MessageCursorIndex;
        }

        public void Unselect()
        {
            IsActive = false;
            MessageSelectionCursorIndex = MessageCursorIndex;
        }

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (MessageSelectionCursorIndex >= 0 && MessageSelectionCursorIndex != MessageCursorIndex)
            {
                if (MessageSelectionCursorPosition > MessageCursorPosition)
                {
                    g.Draw(sprPixel, new Rectangle((int)(TextInputPosition.X + MessageCursorPosition), (int)TextInputPosition.Y + 1, (int)(MessageSelectionCursorPosition - MessageCursorPosition), (int)TextInputSize.Y - 2), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else
                {
                    g.Draw(sprPixel, new Rectangle((int)(TextInputPosition.X + MessageSelectionCursorPosition), (int)TextInputPosition.Y + 1, (int)(MessageCursorPosition - MessageSelectionCursorPosition), (int)TextInputSize.Y - 2), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }

            if (IsActive && IsCursorVisible)
            {
                g.Draw(sprCursor, new Rectangle((int)(TextInputPosition.X + MessageCursorPosition), (int)TextInputPosition.Y + 1, 1, (int)TextInputSize.Y - 2), Color.Black);
            }

            g.DrawString(fntText, TextVisible, new Vector2(TextInputPosition.X, TextInputPosition.Y), Color.Black);
        }
    }
}
