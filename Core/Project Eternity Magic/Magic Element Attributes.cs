using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Magic
{
    public abstract class MagicElementAttribute
    {
        public readonly string Name;
        public readonly int Width;
        public readonly int Height;

        public MagicElementAttribute(string Name, int Width, int Height)
        {
            this.Name = Name;
            this.Width = Width;
            this.Height = Height;
        }

        public abstract void InitGraphics(ContentManager Content);

        public abstract void Update(GameTime gameTime, int MouseX, int MouseY);

        public abstract void Draw(CustomSpriteBatch g, Vector2 VisiblePosition);
    }

    public class MagicElementAttributeSlider : MagicElementAttribute
    {
        public delegate void OnSliderChange(double NewValue);

        private OnSliderChange DelegateOnSliderChange;

        private SpriteFont fntSliderText;
        private Texture2D sprPixel;

        private double MinValue;
        private double MaxValue;
        private int SliderX;

        private double Value;

        public MagicElementAttributeSlider(string Name, double DefaultValue, double MinValue, double MaxValue, OnSliderChange DelegateOnSliderChange)
            : base(Name, 200, 30)
        {
            Value = DefaultValue;
            this.MinValue = MinValue;
            this.MaxValue = MaxValue;
            this.DelegateOnSliderChange = DelegateOnSliderChange;

            InitSliderX();
        }

        public override void InitGraphics(ContentManager Content)
        {
            fntSliderText = Content.Load<SpriteFont>("Fonts/Arial10");
            sprPixel = Content.Load<Texture2D>("pixel");
        }

        public override void Update(GameTime gameTime, int MouseX, int MouseY)
        {
            if ((MouseHelper.InputLeftButtonHold() && MouseHelper.MouseMoved()) || MouseHelper.InputLeftButtonPressed())
            {
                double SliderPercentage = MouseX / (double)Width;
                double Interval = MaxValue - MinValue;

                //Hold shift = divide increases by 10
                if (KeyboardHelper.KeyHold(Keys.LeftShift) || KeyboardHelper.KeyHold(Keys.RightShift))
                {
                    double NewValue = MinValue + SliderPercentage * Interval;
                    double ValueIncrease = NewValue - Value;
                    Value += ValueIncrease / 10d;
                }
                else
                {
                    Value = MinValue + SliderPercentage * Interval;
                }

                InitSliderX();

                DelegateOnSliderChange(Value);
            }
        }

        private void InitSliderX()
        {
            double Percent = (Value - MinValue) / (MaxValue - MinValue);
            SliderX = (int)(Percent * Width);
        }

        public override void Draw(CustomSpriteBatch g, Vector2 VisiblePosition)
        {
            g.Draw(sprPixel, new Rectangle((int)VisiblePosition.X, (int)VisiblePosition.Y + 8, Width, 5), Color.White);
            g.Draw(sprPixel, new Rectangle((int)VisiblePosition.X + SliderX, (int)VisiblePosition.Y, 5, 20), Color.Black);
            g.DrawString(fntSliderText, Value.ToString(), new Vector2(VisiblePosition.X + 10, VisiblePosition.Y + 20), Color.White);
        }
    }
}
