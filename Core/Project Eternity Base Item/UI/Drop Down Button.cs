using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class DropDownButton : IUIElement
    {
        public enum ButtonStates { Idle, Disabled, Hover, Open }

        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick(string SelectedItem);
        private readonly OnClick OnClickDelegate;

        public static Texture2D sprArrow;
        public static Texture2D sprPixel;

        private readonly AnimatedSprite Button;
        private readonly AnimatedSprite Icon;
        private readonly float Scale;
        private DynamicText Text;
        private readonly DynamicText[] ArrayTextChoice;
        private readonly Rectangle ButtonCollsionBox;
        private string SelectedItem;
        private string[] Choices;

        public bool IsIdle { get { return Button.GetFrame() == 0; } }
        public bool IsDisabled { get { return Button.GetFrame() == 1; } }
        public bool IsHover { get { return Button.GetFrame() == 2; } }
        public bool IsOpen { get { return Button.GetFrame() == 3; } }

        public DropDownButton(Microsoft.Xna.Framework.Content.ContentManager Content, string Message, string[] Choices, string ButtonPath, Vector2 ButtonPosition, int NumberOfLines, int ImagesPerLine, float Scale,
            OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;
            this.Scale = Scale;
            this.SelectedItem = Message;
            this.Choices = Choices;
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;

            sprPixel = Content.Load<Texture2D>("Pixel");

            Button = new AnimatedSprite(Content, ButtonPath, ButtonPosition, 0, NumberOfLines, ImagesPerLine);
            Icon = null;

            Text = new DynamicText();
            Text.TextMaxWidthInPixel = Button.SpriteWidth;
            Text.LineHeight = 20;
            Text.ListProcessor.Add(new RegularTextProcessor(Text));
            Text.ListProcessor.Add(new IconProcessor(Text));
            Text.ListProcessor.Add(new DefaultTextProcessor(Text));
            Text.SetDefaultProcessor(new DefaultTextProcessor(Text));

            Text.Load(Content);
            Text.ParseText(Message);

            ArrayTextChoice = new DynamicText[Choices.Length];
            for (int C = 0; C < Choices.Length; ++C)
            {
                DynamicText NewText = new DynamicText();
                NewText.TextMaxWidthInPixel = Button.SpriteWidth;
                NewText.LineHeight = 20;
                NewText.ListProcessor.Add(new RegularTextProcessor(NewText));
                NewText.ListProcessor.Add(new IconProcessor(NewText));
                NewText.ListProcessor.Add(new DefaultTextProcessor(NewText));
                NewText.SetDefaultProcessor(new DefaultTextProcessor(NewText));

                NewText.Load(Content);
                NewText.ParseText(Choices[C]);

                ArrayTextChoice[C] = NewText;
            }

            ButtonCollsionBox = Button.GetCollisionBox(Scale);
        }

        public void Update(GameTime gameTime)
        {
            if (IsDisabled)
            {
                return;
            }

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (!IsOpen)
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
                                OnClickDelegate(null);
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
                            int Index = (MouseHelper.MouseStateCurrent.Y - ButtonCollsionBox.Y - ButtonCollsionBox.Height) / ButtonCollsionBox.Height;
                            SelectedItem = Choices[Index];
                            Text = ArrayTextChoice[Index];
                            if (OnClickDelegate != null)
                            {
                                OnClickDelegate(SelectedItem);
                            }
                        }

                        Unselect();
                    }
                }
            }
        }

        public void Hover()
        {
            if (!IsHover)
            {
                Button.SetFrame(2);
                if (OnOverDelegate != null)
                {
                    OnOverDelegate();
                }
            }

            if (Icon != null)
            {
                Icon.FramesPerSecond = 8;
            }
        }

        public void Select()
        {
            if (IsOpen)
            {
                Button.SetFrame(0);
            }
            else
            {
                Button.SetFrame(3);
            }
        }

        public void Unselect()
        {
            Button.SetFrame(0);

            if (Icon != null)
            {
                Icon.RestartAnimation();
                Icon.FramesPerSecond = 0;
            }
        }

        public void Disable()
        {
            Button.SetFrame(1);

            if (Icon != null)
            {
                Icon.RestartAnimation();
                Icon.FramesPerSecond = 0;
            }
        }

        public void Enable()
        {
            Button.SetFrame(0);

            if (Icon != null)
            {
                Icon.RestartAnimation();
                Icon.FramesPerSecond = 0;
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (IsIdle)
            {
                Button.Draw(g, Scale);
            }
            else if (IsDisabled)
            {
                Button.Draw(g, Scale);
            }
            else if (IsOpen)
            {
                Button.Draw(g, Scale);

                int DropDownStartTextY = (int)(Button.Position.Y + ButtonCollsionBox.Height + 10 * Scale);
                int DropDownStartBoxY = (int)(DropDownStartTextY - ButtonCollsionBox.Height / 2 - 10 * Scale);

                g.Draw(sprPixel, new Rectangle(ButtonCollsionBox.X,
                    DropDownStartBoxY,
                    ButtonCollsionBox.Width, ButtonCollsionBox.Height * Choices.Length), Color.FromNonPremultiplied(167, 170, 167, 255));

                for (int C = 0; C < Choices.Length; ++C)
                {
                    ArrayTextChoice[C].Draw(g, new Vector2(Button.Position.X, DropDownStartTextY + ButtonCollsionBox.Height * C));

                    if (MouseHelper.MouseStateCurrent.X >= ButtonCollsionBox.X
                        && MouseHelper.MouseStateCurrent.X <= ButtonCollsionBox.X + ButtonCollsionBox.Width
                        && MouseHelper.MouseStateCurrent.Y >= DropDownStartBoxY + ButtonCollsionBox.Height * C
                        && MouseHelper.MouseStateCurrent.Y < DropDownStartBoxY + ButtonCollsionBox.Height * (C + 1))
                    {
                        g.Draw(sprPixel, new Rectangle(ButtonCollsionBox.X + 4,
                            DropDownStartBoxY + 4 + ButtonCollsionBox.Height * C,
                            ButtonCollsionBox.Width - 8, ButtonCollsionBox.Height - 8), Color.FromNonPremultiplied(255, 255, 255, 127));
                    }
                }

            }
            else if (IsHover)
            {
                Button.Draw(g, Scale);
            }

            Text.Draw(g, new Vector2(Button.Position.X, Button.Position.Y + 10 * Scale));
            if (Icon != null)
                Icon.Draw(g, Scale);

        }
    }
}
