using Microsoft.Xna.Framework;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class TextButton : IUIElement
    {
        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick();
        private readonly OnClick OnClickDelegate;

        private readonly AnimatedSprite Button;
        private readonly AnimatedSprite Icon;
        private readonly float Scale;
        private readonly DynamicText Text;
        private readonly Rectangle ButtonCollsionBox;
        public bool CanBeChecked;
        public bool CanBeUnChecked;

        public bool IsIdle { get { return Button.GetFrame() == 0; } }
        public bool IsDisabled { get { return Button.GetFrame() == 1; } }
        public bool IsHover { get { return Button.GetFrame() == 2; } }
        public bool IsChecked { get { return Button.GetFrame() == 3; } }

        public TextButton(Microsoft.Xna.Framework.Content.ContentManager Content, string Message, string ButtonPath, Vector2 ButtonPosition, int NumberOfLines, int ImagesPerLine, float Scale,
            OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;
            this.Scale = Scale;

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

            ButtonCollsionBox = Button.GetCollisionBox(Scale);
        }

        public void Update(GameTime gameTime)
        {
            if (IsDisabled)
            {
                return;
            }

            Text.Update(gameTime);

            Button.Update(gameTime);

            if (Icon != null && !Icon.IsOnLastFrame)
            {
                Icon.Update(gameTime);
            }

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (ButtonCollsionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
                {
                    Hover();

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
            if (!IsHover && ((CanBeChecked && !IsChecked) || (!CanBeChecked)))
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
            if (IsChecked && CanBeChecked && CanBeUnChecked)
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
            Button.Draw(g, Scale);
            if (Icon != null)
                Icon.Draw(g, Scale);

            Text.Draw(g, new Vector2(Button.Position.X, Button.Position.Y + 10 * Scale));
        }
    }
}
