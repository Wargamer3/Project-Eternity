using Microsoft.Xna.Framework;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Item
{
    public class InteractiveButton
    {
        public delegate void OnOver();
        private readonly OnOver OnOverDelegate;

        public delegate void OnClick();
        private readonly OnClick OnClickDelegate;

        private readonly AnimatedSprite Button;
        private readonly AnimatedSprite Icon;
        private readonly Rectangle ButtonCollsionBox;
        public bool CanBeChecked;

        public bool IsIdle { get { return Button.GetFrame() == 0; } }
        public bool IsDisabled { get { return Button.GetFrame() == 1; } }
        public bool IsHover { get { return Button.GetFrame() == 2; } }
        public bool IsChecked { get { return Button.GetFrame() == 3; } }

        public InteractiveButton(Microsoft.Xna.Framework.Content.ContentManager Content, string ButtonPath, Vector2 AnimationPosition,
            OnOver OnOverDelegate, OnClick OnClickDelegate)
            : this(Content, ButtonPath, AnimationPosition, 4, OnOverDelegate, OnClickDelegate)
        {

        }

        public InteractiveButton(Microsoft.Xna.Framework.Content.ContentManager Content, string ButtonPath, Vector2 AnimationPosition, int ImagesPerLine,
            OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;

            Button = new AnimatedSprite(Content, ButtonPath, AnimationPosition, 0, 1, ImagesPerLine);
            Icon = null;

            ButtonCollsionBox = Button.GetCollisionBox();
        }

        public InteractiveButton(Microsoft.Xna.Framework.Content.ContentManager Content, string ButtonPath, Vector2 ButtonPosition,
            string IconPath, Vector2 IconOffset, int IconImagesPerLine,
            OnOver OnOverDelegate, OnClick OnClickDelegate)
        {
            this.OnOverDelegate = OnOverDelegate;
            this.OnClickDelegate = OnClickDelegate;

            Button = new AnimatedSprite(Content, ButtonPath, ButtonPosition, 0, 1, 4);
            Icon = new AnimatedSprite(Content, IconPath, ButtonPosition + IconOffset, 0, 1, IconImagesPerLine);

            ButtonCollsionBox = Button.GetCollisionBox();
        }

        public void Update(GameTime gameTime)
        {
            if (IsDisabled)
            {
                return;
            }

            Button.Update(gameTime);

            if (Icon != null && !Icon.IsOnLastFrame)
            {
                Icon.Update(gameTime);
            }

            if (MouseHelper.MouseStateCurrent != MouseHelper.MouseStateLast)
            {
                if (ButtonCollsionBox.Contains(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y))
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
                    if (MouseHelper.InputLeftButtonPressed() && OnClickDelegate != null)
                    {
                        Select();
                    }
                }
                else
                {
                    Unselect();
                }
            }
        }

        public void Select()
        {
            Button.SetFrame(3);
            OnClickDelegate();
        }

        public void Check()
        {
            Button.SetFrame(3);
        }

        public void Unselect()
        {
            if (!CanBeChecked || !IsChecked)
            {
                Button.SetFrame(0);

                if (Icon != null)
                {
                    Icon.RestartAnimation();
                    Icon.FramesPerSecond = 0;
                }
            }
        }

        public void Uncheck()
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
            Button.Draw(g);
            if (Icon != null)
                Icon.Draw(g);
        }
    }
}
