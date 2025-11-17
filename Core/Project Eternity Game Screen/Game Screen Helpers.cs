using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens
{
    public partial class GameScreen
    {
        private static Texture2D _sprPixel;

        private static Texture2D sprClassicBox;
        private static Texture2D sprClassicBoxBackground;
        private static Texture2D sprClassicBoxBackgroundRed;
        private static Texture2D sprClassicBoxBackgroundBlue;
        private static Texture2D sprClassicBoxBackgroundGray;
        private static Texture2D sprClassicBoxBackgroundBlack;

        private static Texture2D sprPureBlack;
        private static Texture2D sprPureWhite;

        public static Texture2D sprPixel { get { return _sprPixel; } }

        public static DebugScreen Debug;

        public static void LoadHelpers(ContentManager Content)
        {
            ContentFallback = Content;
            _sprPixel = Content.Load<Texture2D>("Pixel");

            sprClassicBox = Content.Load<Texture2D>("Title Screen/Main Menu/Box");
            sprClassicBoxBackground = Content.Load<Texture2D>("Title Screen/Main Menu/Background");
            sprClassicBoxBackgroundRed = Content.Load<Texture2D>("Title Screen/Main Menu/Background Red");
            sprClassicBoxBackgroundBlue = Content.Load<Texture2D>("Title Screen/Main Menu/Background Blue");
            sprClassicBoxBackgroundGray = Content.Load<Texture2D>("Title Screen/Main Menu/Background Gray");
            sprClassicBoxBackgroundBlack = Content.Load<Texture2D>("Title Screen/Main Menu/Background Black");

            sprPureBlack = Content.Load<Texture2D>("Title Screen/Main Menu/Popup Pure Black");
            sprPureWhite = Content.Load<Texture2D>("Title Screen/Main Menu/Popup Pure White");

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
            Texture2D Background;
            Texture2D BackgroundBorder;
            bool SameColorForBorder = true;
            bool DrawBackgroundUnderBorder = false;

            int CornerSizeTopOriginWidth = 7;
            int CornerSizeTopOriginHeight = 7;
            int CornerSizeBottomOriginWidth = 7;
            int CornerSizeBottomOriginHeight = 7;

            if (Constants.BoxStyle == Constants.BoxStyles.PureBlack)
            {
                Background = sprPureBlack;
                BackgroundBorder = sprPureBlack;

                CornerSizeTopOriginWidth = 25;
                CornerSizeTopOriginHeight = 25;
                CornerSizeBottomOriginWidth = 56;
                CornerSizeBottomOriginHeight = 25;
            }
            else if (Constants.BoxStyle == Constants.BoxStyles.PureWhite)
            {
                Background = sprPureWhite;
                BackgroundBorder = sprPureWhite;

                CornerSizeTopOriginWidth = 25;
                CornerSizeTopOriginHeight = 25;
                CornerSizeBottomOriginWidth = 56;
                CornerSizeBottomOriginHeight = 25;
            }
            else
            {
                SameColorForBorder = false;
                DrawBackgroundUnderBorder = true;
                BackgroundBorder = sprClassicBox;

                if (BackgroundColor == Color.White || BackgroundColor == Color.Green)
                {
                    BackgroundColor = Color.White;
                    Background = sprClassicBoxBackground;
                }
                else if (BackgroundColor == Color.Red)
                {
                    BackgroundColor = Color.White;
                    Background = sprClassicBoxBackgroundRed;
                }
                else if (BackgroundColor == Color.Blue)
                {
                    BackgroundColor = Color.White;
                    Background = sprClassicBoxBackgroundBlue;
                }
                else if (BackgroundColor == Color.Gray)
                {
                    BackgroundColor = Color.White;
                    Background = sprClassicBoxBackgroundGray;
                }
                else if (BackgroundColor == Color.Black)
                {
                    BackgroundColor = Color.White;
                    Background = sprClassicBoxBackgroundBlack;
                }
                else
                {
                    Background = sprClassicBox;
                }
            }

            int BackgroundWidth = BackgroundBorder.Width;
            int BackgroundHeight = BackgroundBorder.Height;
            int CornerSizeTopWidth = CornerSizeTopOriginWidth;
            int CornerSizeTopHeight = CornerSizeTopOriginHeight;
            int CornerSizeBottomWidth = CornerSizeBottomOriginWidth;
            int CornerSizeBottomHeight = CornerSizeBottomOriginHeight;

            if (Width < BackgroundBorder.Width)
            {
                float ScaleX = Width / (float)BackgroundBorder.Width;

                CornerSizeTopWidth = (int)(CornerSizeTopWidth * ScaleX);
                CornerSizeBottomWidth = (int)(CornerSizeBottomWidth * ScaleX);
            }
            if (Height < BackgroundBorder.Height)
            {
                float ScaleY = Height / (float)BackgroundBorder.Height;

                CornerSizeTopHeight = (int)(CornerSizeTopWidth * ScaleY);
                CornerSizeBottomHeight = (int)(CornerSizeBottomWidth * ScaleY);
            }

            if (DrawBackgroundUnderBorder)
            {
                g.Draw(Background, new Rectangle((int)Position.X, (int)Position.Y, Width, Height),
                    new Rectangle(0, 0, Background.Width, Background.Height), BackgroundColor);
            }
            else
            {
                g.Draw(Background, new Rectangle((int)Position.X + CornerSizeTopWidth, (int)Position.Y + CornerSizeTopHeight, Width - CornerSizeTopWidth * 2, Height - CornerSizeTopHeight  * 2),
                    new Rectangle(CornerSizeTopOriginWidth, CornerSizeTopOriginHeight, Background.Width - CornerSizeTopOriginWidth * 2, Background.Height - CornerSizeTopOriginHeight * 2), BackgroundColor);
            }
            if (!SameColorForBorder)
            {
                BackgroundColor = Color.White;
            }

            //Top Center.
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X + CornerSizeTopWidth, (int)Position.Y, Width - CornerSizeTopWidth * 2, CornerSizeTopHeight),
                new Rectangle(CornerSizeTopOriginWidth, 0, BackgroundWidth - CornerSizeTopOriginWidth * 2, CornerSizeTopOriginHeight), BackgroundColor);
            //Bottom Center.
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X + CornerSizeBottomWidth, (int)Position.Y + Height - CornerSizeBottomHeight, Width - CornerSizeBottomWidth * 2, CornerSizeBottomHeight),
                new Rectangle(CornerSizeBottomOriginWidth, BackgroundHeight - CornerSizeBottomOriginHeight, BackgroundWidth - CornerSizeBottomOriginWidth * 2, CornerSizeBottomOriginHeight), BackgroundColor);

            //Left Center
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X, (int)Position.Y + CornerSizeTopHeight, CornerSizeTopWidth, Height - CornerSizeTopHeight - CornerSizeBottomHeight),
                new Rectangle(0, CornerSizeTopOriginHeight, CornerSizeTopOriginHeight, BackgroundHeight - CornerSizeTopOriginHeight - CornerSizeBottomOriginHeight), BackgroundColor);
            //Right Center
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X + Width - CornerSizeTopWidth, (int)Position.Y + CornerSizeTopHeight, CornerSizeTopWidth, Height - CornerSizeTopHeight - CornerSizeBottomHeight),
                new Rectangle(BackgroundWidth - CornerSizeTopOriginWidth, CornerSizeTopOriginHeight, CornerSizeTopOriginWidth, BackgroundHeight - CornerSizeTopOriginHeight - CornerSizeBottomOriginHeight), BackgroundColor);
            
            //Top Left Corner.
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X, (int)Position.Y, CornerSizeTopWidth, CornerSizeTopHeight),
                new Rectangle(0, 0, CornerSizeTopOriginWidth, CornerSizeTopOriginHeight), BackgroundColor);
            //Bottom Left Corner.
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X, (int)(Position.Y) + Height - CornerSizeBottomHeight, CornerSizeBottomWidth, CornerSizeBottomHeight),
                new Rectangle(0, BackgroundHeight - CornerSizeBottomOriginHeight, CornerSizeBottomOriginWidth, CornerSizeBottomOriginHeight), BackgroundColor);
            
            //Right Top
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X + Width - CornerSizeTopWidth, (int)Position.Y, CornerSizeTopWidth, CornerSizeTopHeight),
                new Rectangle(BackgroundWidth - CornerSizeTopOriginWidth, 0, CornerSizeTopOriginWidth, CornerSizeTopOriginWidth), BackgroundColor);
            //Right Bottom
            g.Draw(BackgroundBorder, new Rectangle((int)Position.X + Width - CornerSizeBottomWidth, (int)Position.Y + Height - CornerSizeBottomHeight, CornerSizeBottomWidth, CornerSizeBottomHeight),
                new Rectangle(BackgroundWidth - CornerSizeBottomOriginWidth, BackgroundHeight - CornerSizeBottomOriginHeight, CornerSizeBottomOriginWidth, CornerSizeBottomOriginHeight), BackgroundColor);
        }

        public static void DrawEmptyBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height)
        {
            int CornerSize = 7;
            int HorizontalLineSize = Math.Min(Width, CornerSize * 4);
            int VerticalLineSize = Math.Min(Height, CornerSize);

            g.Draw(sprPixel, new Rectangle((int)Position.X + 1, (int)Position.Y, HorizontalLineSize, 1), Color.White);
            g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + 1, 1, VerticalLineSize), Color.White);

            g.Draw(sprPixel, new Rectangle((int)Position.X + Width - HorizontalLineSize, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
            g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y + Height - VerticalLineSize, 1, VerticalLineSize), Color.White);
        }

        public static void DrawEmptyBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height, int NumberOfLines, double AnimationProgress)
        {
            int PixelProgress = (int)(AnimationProgress * 50);
            int CornerSize = 7;
            int FullLineSize = CornerSize * 5;
            int MaxSize = Width * 2 + Height * 2;

            int OffsetBetweenLines = MaxSize / NumberOfLines;
            int StartPostion = (OffsetBetweenLines - CornerSize + PixelProgress) % OffsetBetweenLines;

            int TopSideLimit = Width;
            int RightSideLimit = Width + Height;
            int BottomSideLimit = Width + Width + Height;
            int LeftSideLimit = MaxSize;

            for (int i = StartPostion; i < MaxSize; i += OffsetBetweenLines)
            {
                int HorizontalLineSize;
                int VerticalLineSize;
                if (i < TopSideLimit)//Top
                {
                    if (i + FullLineSize - TopSideLimit < 0)
                    {
                        HorizontalLineSize = FullLineSize;
                        VerticalLineSize = 0;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + i, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                    }
                    else//Top Right Corner
                    {
                        HorizontalLineSize = TopSideLimit - i;
                        VerticalLineSize = i + FullLineSize - TopSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + i, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y, 1, VerticalLineSize), Color.White);
                    }
                }
                else if (i < RightSideLimit)//Right
                {
                    if (i + FullLineSize - RightSideLimit < 0)
                    {
                        HorizontalLineSize = 0;
                        VerticalLineSize = FullLineSize;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y + (i - TopSideLimit), 1, VerticalLineSize), Color.White);
                    }
                    else//Bottom Right Corner
                    {
                        VerticalLineSize = RightSideLimit - i;
                        HorizontalLineSize = i + FullLineSize - RightSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y + Height - VerticalLineSize, 1, VerticalLineSize), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width - HorizontalLineSize, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                    }
                }
                else if (i < BottomSideLimit)//Bottom
                {
                    if (i + FullLineSize - BottomSideLimit < 0)
                    {
                        HorizontalLineSize = FullLineSize;
                        VerticalLineSize = 0;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width - (i - RightSideLimit) - HorizontalLineSize, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                    }
                    else//Bottom Left Corner
                    {
                        HorizontalLineSize = BottomSideLimit - i;
                        VerticalLineSize = i + FullLineSize - BottomSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height - VerticalLineSize, 1, VerticalLineSize), Color.White);
                    }
                }
                else//Left
                {
                    if (i + FullLineSize - LeftSideLimit < 0)
                    {
                        HorizontalLineSize = 0;
                        VerticalLineSize = FullLineSize;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height - (i - BottomSideLimit) - VerticalLineSize, 1, VerticalLineSize), Color.White);
                    }
                    else//Top Left Corner
                    {
                        VerticalLineSize = LeftSideLimit - i;
                        HorizontalLineSize = i + FullLineSize - LeftSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y, 1, VerticalLineSize), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                    }
                }
            }
        }

        public static void DrawEmptyBox(CustomSpriteBatch g, Vector2 Position, int Width, int Height, int NumberOfLines, int CornerSize, double AnimationProgress)
        {
            int PixelProgress = (int)(AnimationProgress * 50);
            int FullLineSize = CornerSize * 5;
            int MaxSize = Width * 2 + Height * 2;

            int OffsetBetweenLines = MaxSize / NumberOfLines;
            int StartPostion = (OffsetBetweenLines - CornerSize + PixelProgress) % OffsetBetweenLines;

            int TopSideLimit = Width;
            int RightSideLimit = Width + Height;
            int BottomSideLimit = Width + Width + Height;
            int LeftSideLimit = MaxSize;

            for (int i = StartPostion; i < MaxSize; i += OffsetBetweenLines)
            {
                int HorizontalLineSize;
                int VerticalLineSize;
                if (i < TopSideLimit)//Top
                {
                    if (i + FullLineSize - TopSideLimit < 0)
                    {
                        HorizontalLineSize = FullLineSize;
                        VerticalLineSize = 0;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + i, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                    }
                    else//Top Right Corner
                    {
                        HorizontalLineSize = TopSideLimit - i;
                        VerticalLineSize = i + FullLineSize - TopSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + i, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y, 1, VerticalLineSize), Color.White);
                    }
                }
                else if (i < RightSideLimit)//Right
                {
                    if (i + FullLineSize - RightSideLimit < 0)
                    {
                        HorizontalLineSize = 0;
                        VerticalLineSize = FullLineSize;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y + (i - TopSideLimit), 1, VerticalLineSize), Color.White);
                    }
                    else//Bottom Right Corner
                    {
                        VerticalLineSize = RightSideLimit - i;
                        HorizontalLineSize = i + FullLineSize - RightSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width, (int)Position.Y + Height - VerticalLineSize, 1, VerticalLineSize), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width - HorizontalLineSize, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                    }
                }
                else if (i < BottomSideLimit)//Bottom
                {
                    if (i + FullLineSize - BottomSideLimit < 0)
                    {
                        HorizontalLineSize = FullLineSize;
                        VerticalLineSize = 0;
                        g.Draw(sprPixel, new Rectangle((int)Position.X + Width - (i - RightSideLimit) - HorizontalLineSize, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                    }
                    else//Bottom Left Corner
                    {
                        HorizontalLineSize = BottomSideLimit - i;
                        VerticalLineSize = i + FullLineSize - BottomSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height, HorizontalLineSize, 1), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height - VerticalLineSize, 1, VerticalLineSize), Color.White);
                    }
                }
                else//Left
                {
                    if (i + FullLineSize - LeftSideLimit < 0)
                    {
                        HorizontalLineSize = 0;
                        VerticalLineSize = FullLineSize;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y + Height - (i - BottomSideLimit) - VerticalLineSize, 1, VerticalLineSize), Color.White);
                    }
                    else//Top Left Corner
                    {
                        VerticalLineSize = LeftSideLimit - i;
                        HorizontalLineSize = i + FullLineSize - LeftSideLimit;
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y, 1, VerticalLineSize), Color.White);
                        g.Draw(sprPixel, new Rectangle((int)Position.X, (int)Position.Y, HorizontalLineSize, 1), Color.White);
                    }
                }
            }
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
