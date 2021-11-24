using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core
{
    public static class TextHelper
    {
        public enum TextAligns { Left, Center, Right, Justified }

        private static SpriteFont fntWhiteFont;
        private static SpriteFont _fntShadowFont;

        public static SpriteFont fntShadowFont { get { return _fntShadowFont; } }

        public static void LoadHelpers(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            fntWhiteFont = Content.Load<SpriteFont>("Fonts/arialCustom10-5");
            fntWhiteFont.Spacing = -7;
            _fntShadowFont = Content.Load<SpriteFont>("Fonts/arialCustom10-5S");
            _fntShadowFont.Spacing = -7;
        }

        public static List<string> FitToWidth(SpriteFont TextFont, string Text, int TextMaxWidthInPixel)
        {
            List<string> ListText = new List<string>();

            string[] ArrayWorkingString = Text.Split('\n', '\r');

            for (int i = 0; i < ArrayWorkingString.Length; ++i)
            {
                string WorkingString = ArrayWorkingString[i];
                int CurrentChar = Math.Min(TextMaxWidthInPixel / 10, WorkingString.Length);

                while (WorkingString.Length > 0)
                {
                    string TestString = WorkingString.Substring(0, CurrentChar);

                    if (TextFont.MeasureString(TestString).X > TextMaxWidthInPixel)
                    {
                        --CurrentChar;
                        TestString = WorkingString.Substring(0, CurrentChar);
                        if (TextFont.MeasureString(TestString).X <= TextMaxWidthInPixel)
                        {
                            int LastSpace = TestString.LastIndexOf(' ') + 1;
                            if (LastSpace > 0)
                            {
                                CurrentChar = LastSpace;
                            }

                            TestString = WorkingString.Substring(0, CurrentChar);
                            ListText.Add(TestString);
                            WorkingString = WorkingString.Remove(0, CurrentChar);
                            CurrentChar = 0;
                        }
                    }
                    else if (TextFont.MeasureString(TestString).X <= TextMaxWidthInPixel)
                    {
                        ++CurrentChar;
                        if (CurrentChar >= WorkingString.Length)
                        {
                            TestString = WorkingString;
                            ListText.Add(TestString);
                            WorkingString = string.Empty;
                            CurrentChar = 0;
                        }
                        else
                        {
                            TestString = WorkingString.Substring(0, CurrentChar);
                            if (TextFont.MeasureString(TestString).X > TextMaxWidthInPixel)
                            {
                                int LastSpace = TestString.LastIndexOf(' ') + 1;
                                if (LastSpace > 0)
                                {
                                    CurrentChar = LastSpace;
                                }

                                TestString = WorkingString.Substring(0, CurrentChar);
                                ListText.Add(TestString);
                                WorkingString = WorkingString.Remove(0, CurrentChar);
                                CurrentChar = 0;
                            }
                        }
                    }
                }
            }

            return ListText;
        }

        public static void DrawText(CustomSpriteBatch g, string Text, Vector2 Position, Color TextColor)
        {
            g.DrawString(_fntShadowFont, Text, Position, Color.Black);
            g.DrawString(fntWhiteFont, Text, Position, TextColor);
        }

        public static void DrawTextRightAligned(CustomSpriteBatch g, string Text, Vector2 Position, Color TextColor)
        {
            int TextWidth = (int)fntShadowFont.MeasureString(Text).X;
            g.DrawString(fntShadowFont, Text, Position, Color.Black, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 1);
            g.DrawString(fntWhiteFont, Text, Position, TextColor, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 0);
        }

        public static void DrawTextMiddleAligned(CustomSpriteBatch g, string Text, Vector2 Position, Color TextColor)
        {
            int TextWidth = (int)fntShadowFont.MeasureString(Text).X / 2;
            g.DrawString(fntShadowFont, Text, Position, Color.Black, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 1);
            g.DrawString(fntWhiteFont, Text, Position, TextColor, 0, new Vector2(TextWidth, 0), 1, SpriteEffects.None, 0);
        }

        public static void DrawTextMultiline(CustomSpriteBatch g, List<string> ListText, TextAligns TextAlign, float XPos, float YPos, int TextMaxWidthInPixel)
        {
            float YOffset = 0;

            if (TextAlign == TextAligns.Left)
            {
                XPos -= TextMaxWidthInPixel / 2;
                foreach (string ActiveLine in ListText)
                {
                    DrawText(g, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White);
                    YOffset += fntShadowFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Right)
            {
                XPos += TextMaxWidthInPixel / 2;
                foreach (string ActiveLine in ListText)
                {
                    DrawTextRightAligned(g, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White);
                    YOffset += fntShadowFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Center)
            {
                foreach (string ActiveLine in ListText)
                {
                    DrawTextMiddleAligned(g, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White);
                    YOffset += fntShadowFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Justified)
            {
                XPos -= TextMaxWidthInPixel / 2;

                foreach (string ActiveLine in ListText)
                {
                    float TextWidth = fntShadowFont.MeasureString(ActiveLine).X;
                    float ScaleFactor = TextMaxWidthInPixel / TextWidth;
                    for (int C = 0; C < ActiveLine.Length; ++C)
                    {
                        float Offset = fntShadowFont.MeasureString(ActiveLine.Substring(0, C)).X;
                        DrawText(g, ActiveLine[C].ToString(), new Vector2(XPos + Offset * ScaleFactor, YPos + YOffset), Color.White);
                    }
                    YOffset += fntShadowFont.LineSpacing;
                }
            }
        }

        public static void DrawTextMultiline(CustomSpriteBatch g, SpriteFont TextFont, List<string> ListText, TextAligns TextAlign, float XPos, float YPos, int TextMaxWidthInPixel)
        {
            float YOffset = 0;

            if (TextAlign == TextAligns.Left)
            {
                XPos -= TextMaxWidthInPixel / 2;
                foreach (string ActiveLine in ListText)
                {
                    g.DrawString(TextFont, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White);
                    YOffset += TextFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Right)
            {
                XPos += TextMaxWidthInPixel / 2;
                foreach (string ActiveLine in ListText)
                {
                    int TextWidth = (int)TextFont.MeasureString(ActiveLine).X;
                    g.DrawString(TextFont, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White, 0f, new Vector2(TextWidth, 0), 1f, SpriteEffects.None, 0f);
                    YOffset += TextFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Center)
            {
                foreach (string ActiveLine in ListText)
                {
                    int TextWidth = (int)TextFont.MeasureString(ActiveLine).X / 2;
                    g.DrawString(TextFont, ActiveLine, new Vector2(XPos, YPos + YOffset), Color.White, 0f, new Vector2(TextWidth, 0), 1f, SpriteEffects.None, 0f);
                    YOffset += TextFont.LineSpacing;
                }
            }
            else if (TextAlign == TextAligns.Justified)
            {
                XPos -= TextMaxWidthInPixel / 2;

                foreach (string ActiveLine in ListText)
                {
                    float TextWidth = TextFont.MeasureString(ActiveLine).X;
                    float ScaleFactor = TextMaxWidthInPixel / TextWidth;
                    for (int C = 0; C < ActiveLine.Length; ++C)
                    {
                        float Offset = TextFont.MeasureString(ActiveLine.Substring(0, C)).X;
                        g.DrawString(TextFont, ActiveLine[C].ToString(), new Vector2(XPos + Offset * ScaleFactor, YPos + YOffset), Color.White);
                    }
                    YOffset += TextFont.LineSpacing;
                }
            }
        }
    }
}
