using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens
{
    public partial class GameScreen
    {
        private static Texture2D _sprPixel;

        private static Texture2D sprBox;
        private static Texture2D sprBoxBackground;
        private static Texture2D sprBoxBackgroundRed;
        private static Texture2D sprBoxBackgroundBlue;
        private static Texture2D sprBoxBackgroundGray;
        private static Texture2D sprBoxBackgroundBlack;

        public static Texture2D sprPixel { get { return _sprPixel; } }

        public static DebugScreen Debug;

        public static void LoadHelpers(ContentManager Content)
        {
            ContentFallback = Content;
            _sprPixel = Content.Load<Texture2D>("Pixel");

            sprBox = Content.Load<Texture2D>("Menus/Main Menu/Box");
            sprBoxBackground = Content.Load<Texture2D>("Menus/Main Menu/Background");
            sprBoxBackgroundRed = Content.Load<Texture2D>("Menus/Main Menu/Background Red");
            sprBoxBackgroundBlue = Content.Load<Texture2D>("Menus/Main Menu/Background Blue");
            sprBoxBackgroundGray = Content.Load<Texture2D>("Menus/Main Menu/Background Gray");
            sprBoxBackgroundBlack = Content.Load<Texture2D>("Menus/Main Menu/Background Black");

            Debug = new DebugScreen();
        }

        public static void DrawLine(CustomSpriteBatch g, Vector2 StartPos, Vector2 EndPos, Color ActiveColor, int width = 1)
        {
            Vector2 Axis = StartPos - EndPos;
            Rectangle LineSize = new Rectangle((int)StartPos.X, (int)StartPos.Y, (int)Axis.Length() + width, width);

            Axis.Normalize();
            float Angle = (float)Math.Acos(Vector2.Dot(Axis, -Vector2.UnitX));

            if (StartPos.Y > EndPos.Y)
                Angle = MathHelper.TwoPi - Angle;

            g.Draw(sprPixel, LineSize, null, ActiveColor, Angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(CustomSpriteBatch g, Vector2 StartPos, Vector2 EndPos, Color ActiveColor, int Width = 1)
        {
            DrawLine(g, StartPos, new Vector2(EndPos.X - 1, StartPos.Y), ActiveColor, Width);
            DrawLine(g, StartPos, new Vector2(StartPos.X, EndPos.Y), ActiveColor, Width);
            DrawLine(g, new Vector2(StartPos.X, EndPos.Y), new Vector2(EndPos.X - 1, EndPos.Y), ActiveColor, Width);
            DrawLine(g, new Vector2(EndPos.X, StartPos.Y), EndPos, ActiveColor, Width);
        }

        public static void DrawBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height, Color BackgroundColor)
        {
            int CornerSize = 7;

            if (BackgroundColor == Color.White || BackgroundColor == Color.Green)
                g.Draw(sprBoxBackground, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            else if (BackgroundColor == Color.Red)
                g.Draw(sprBoxBackgroundRed, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            else if (BackgroundColor == Color.Blue)
                g.Draw(sprBoxBackgroundBlue, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            else if (BackgroundColor == Color.Gray)
                g.Draw(sprBoxBackgroundGray, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            else if (BackgroundColor == Color.Black)
                g.Draw(sprBoxBackgroundBlack, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            else
                g.Draw(sprBoxBackground, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), BackgroundColor);
            //Top Center.
            g.Draw(sprBox, new Rectangle((int)Position.X + CornerSize, (int)Position.Y, Width - CornerSize * 2, CornerSize),
                new Rectangle(CornerSize, 0, sprBox.Width - CornerSize * 2, CornerSize), Color.White);
            //Bottom Center.
            g.Draw(sprBox, new Rectangle((int)Position.X + CornerSize, (int)Position.Y + Height - CornerSize, Width - CornerSize * 2, CornerSize),
                new Rectangle(CornerSize, sprBox.Height - CornerSize, sprBox.Width - CornerSize * 2, CornerSize), Color.White);

            g.Draw(sprBox, new Rectangle((int)Position.X, (int)Position.Y + CornerSize, CornerSize, Height - CornerSize * 2),
                new Rectangle(0, CornerSize, CornerSize, sprBox.Height - CornerSize * 2), Color.White);
            g.Draw(sprBox, new Rectangle((int)Position.X + Width - CornerSize, (int)Position.Y + CornerSize, CornerSize, Height - CornerSize * 2),
                new Rectangle(sprBox.Width - CornerSize, CornerSize, CornerSize, sprBox.Height - CornerSize * 2), Color.White);

            //Top Left Corner.
            g.Draw(sprBox, new Vector2(Position.X, Position.Y), new Rectangle(0, 0, CornerSize, CornerSize), Color.White);
            //Bottom Left Corner.
            g.Draw(sprBox, new Vector2(Position.X, Position.Y + Height - CornerSize), new Rectangle(0, sprBox.Height - CornerSize, CornerSize, CornerSize), Color.White);

            g.Draw(sprBox, new Vector2(Position.X + Width - CornerSize, Position.Y), new Rectangle(sprBox.Width - CornerSize, 0, CornerSize, CornerSize), Color.White);
            g.Draw(sprBox, new Vector2(Position.X + Width - CornerSize, Position.Y + Height - CornerSize), new Rectangle(sprBox.Width - CornerSize, sprBox.Height - CornerSize, CornerSize, CornerSize), Color.White);
        }

        public static void DrawBar(CustomSpriteBatch g, Texture2D sprBackground, Texture2D sprBar, Vector2 Position, int Value, float ValueMax)
        {
            g.Draw(sprBackground, new Vector2(Position.X, Position.Y), Color.White);
            g.Draw(sprBar, new Vector2(Position.X, Position.Y), new Rectangle(0, 0, (int)((sprBar.Width / ValueMax) * Value), sprBar.Height), Color.White);
        }

        public static void DrawNumberLeftAligned(CustomSpriteBatch g, Texture2D sprNumberSheet, int NumberToDraw, Vector2 Position)
        {
            int DigitSize = sprNumberSheet.Width / 10;
            string TextNumber = NumberToDraw.ToString();
            for (int D = 0; D < TextNumber.Length; ++D)
            {
                Position.X += DigitSize;
                int DitgitToDraw = int.Parse(TextNumber[D].ToString());
                g.Draw(sprNumberSheet, Position, new Rectangle(DitgitToDraw * DigitSize, 0, DigitSize, sprNumberSheet.Height), Color.White);
            }
        }

        public static void DrawNumberRightAligned(CustomSpriteBatch g, Texture2D sprNumberSheet, int NumberToDraw, Vector2 Position)
        {
            int DigitSize = sprNumberSheet.Width / 10;

            do
            {
                int DitgitToDraw = NumberToDraw % 10;
                NumberToDraw /= 10;
                Position.X -= DigitSize;
                g.Draw(sprNumberSheet, Position, new Rectangle(DitgitToDraw * DigitSize, 0, DigitSize, sprNumberSheet.Height), Color.White);
            }
            while (NumberToDraw > 0) ;
        }

        public static string FitTextToWidth(SpriteFont TextFont, string Text, float MaxWidth)
        {
            float TextMaxWidth = TextFont.MeasureString(Text).X;

            string Output = "";
            int CurrentLength = 0;

            while (TextMaxWidth >= MaxWidth && CurrentLength + 1 < Text.Length)
            {
                if (TextFont.MeasureString(Text.Substring(0, CurrentLength + 1)).X >= MaxWidth)
                {
                    //Check for spaces to split there first.
                    for (int i = CurrentLength - 1; i >= 0; --i)
                    {
                        if (Text[i] == ' ')
                        {
                            Output += Text.Substring(0, i + 1) + "\r\n";
                            Text = Text.Remove(0, i + 1);
                            TextMaxWidth = TextFont.MeasureString(Text).X;
                            CurrentLength = 0;
                            break;
                        }
                    }

                    if (CurrentLength > 0)
                    {
                        Output += Text.Substring(0, CurrentLength) + "\r\n";
                        Text = Text.Remove(0, CurrentLength);
                        TextMaxWidth = TextFont.MeasureString(Text).X;
                        CurrentLength = 0;
                    }
                }

                ++CurrentLength;
            }
            Output += Text;

            return Output;
        }
    }
}
