using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class PopUpInputBox : GameScreen
    {
        private bool _IsOpen;
        public bool IsOpen { get { return _IsOpen; } }
        public string Text;
        private string HeaderMessage;
        private SpriteFont fntArial12;
        private Keys[] PressedKeysOld;

        private int CursorPos;

        /// <summary>
        /// Do not add this GameScreen to the global list to use it properly.
        /// </summary>
        public PopUpInputBox(string HeaderMessage)
            : base()
        {
            this.HeaderMessage = HeaderMessage;
            PressedKeysOld = new Keys[0];
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            Text = "127.0.0.1";
        }

        public void Open()
        {
            _IsOpen = true;
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            Keys[] pressedKeys = KeyboardHelper.PlayerState.GetPressedKeys();
            //check if any of the previous update's keys are no longer pressed
            foreach (Keys key in PressedKeysOld)
            {
                if (!pressedKeys.Contains(key))
                {
                    switch (key)
                    {
                        case Keys.Right:
                            if (CursorPos < Text.Length)
                                CursorPos++;
                            break;

                        case Keys.Left:
                            if (CursorPos > 0)
                                CursorPos--;
                            break;

                        case Keys.Space:
                            Text += " ";
                            break;

                        case Keys.Back:
                            if (CursorPos > 0)
                                Text = Text.Remove(--CursorPos, 1);
                            break;

                        case Keys.Delete:
                            if (CursorPos < Text.Length)
                                Text = Text.Remove(CursorPos, 1);
                            break;

                        case Keys.LeftControl:
                        case Keys.RightControl:
                        case Keys.LeftAlt:
                        case Keys.RightAlt:
                            break;

                        case Keys.Enter:
                            _IsOpen = false;
                            break;

                        case Keys.Escape:
                            _IsOpen = false;
                            Text = "";
                            break;

                        case Keys.OemPeriod:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += ".";
                            break;

                        case Keys.V:
                            if (pressedKeys.Contains(Keys.LeftControl) || pressedKeys.Contains(Keys.RightControl))
                            {
                                System.Threading.Thread thread = new System.Threading.Thread(CopyClipboard);
                                thread.SetApartmentState(System.Threading.ApartmentState.STA); //Set the thread to STA
                                thread.Start();
                                thread.Join();
                            }
                            break;

                        #region Numbers

                        case Keys.D0:
                        case Keys.NumPad0:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "0";
                            break;

                        case Keys.D1:
                        case Keys.NumPad1:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "1";
                            break;

                        case Keys.D2:
                        case Keys.NumPad2:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "0";
                            break;

                        case Keys.D3:
                        case Keys.NumPad3:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "3";
                            break;

                        case Keys.D4:
                        case Keys.NumPad4:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "4";
                            break;

                        case Keys.D5:
                        case Keys.NumPad5:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "5";
                            break;

                        case Keys.D6:
                        case Keys.NumPad6:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "6";
                            break;

                        case Keys.D7:
                        case Keys.NumPad7:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "7";
                            break;

                        case Keys.D8:
                        case Keys.NumPad8:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "8";
                            break;

                        case Keys.D9:
                        case Keys.NumPad9:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += "9";
                            break;

                        #endregion

                        default:
                            if (CursorPos == Text.Length)
                                CursorPos++;
                            Text += key;
                            break;
                    }
                }
            }
            PressedKeysOld = pressedKeys;
        }

        private void CopyClipboard()
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                string ClipboardText = System.Windows.Forms.Clipboard.GetText();
                if (CursorPos == Text.Length)
                    CursorPos += ClipboardText.Length;
                Text += ClipboardText;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X = 200;
            int Y = 150;

            g.Draw(sprPixel, new Rectangle(X, Y, 200, 150), Color.Navy);
            g.DrawString(fntArial12, HeaderMessage, new Vector2(X + 20, Y + 5), Color.White);

            g.Draw(sprPixel, new Rectangle(X, Y + 120, 200, 30), Color.Blue);
            g.DrawString(fntArial12, Text, new Vector2(X + 15, Y + 125), Color.Black);

            int CursorRealPos = (int)fntArial12.MeasureString(Text.Substring(0, CursorPos)).X;
            g.Draw(sprPixel, new Rectangle(X + 15 + CursorRealPos, Y + 122, 1, 26), Color.Black);
        }
    }
}
