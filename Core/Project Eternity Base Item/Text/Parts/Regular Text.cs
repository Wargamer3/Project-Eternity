using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class RegularText : DynamicTextPart
    {
        public readonly SpriteFont fntTextFont;
        public readonly List<string> ListText = new List<string>();

        public RegularText(DynamicText Owner, string OriginalText)
             : base(Owner, OriginalText, "Text:")
        {
            ListText = new List<string>();
        }

        public RegularText(DynamicText Owner, string OriginalText, string Prefix)
             : base(Owner, OriginalText, Prefix)
        {
            ListText = new List<string>();
        }

        public RegularText(DynamicText Owner, FontsHolder Fonts, string OriginalText, string Prefix)
             : base(Owner, OriginalText, Prefix)
        {
            if (DicSubTag.ContainsKey("Font"))
            {
                switch (DicSubTag["Font"])
                {
                    case "16":
                        fntTextFont = Fonts.fntArial16;
                        break;

                    default:
                        fntTextFont = Fonts.fntDefaultFont;
                        break;
                }
            }
            else
            {
                fntTextFont = Fonts.fntDefaultFont;
            }

            ListText = new List<string>();
        }

        public override Vector2 UpdatePosition()
        {
            bool IsAColumn = false;
            float Offset = 0;
            if (DicSubTag.ContainsKey("MaxWidth"))
            {
                MaxWidth = float.Parse(DicSubTag["MaxWidth"]);
                IsAColumn = true;
            }
            else
            {
                MaxWidth = Owner.TextMaxWidthInPixel;
            }
            if (DicSubTag.ContainsKey("Offset"))
            {
                Offset = float.Parse(DicSubTag["Offset"]);
                IsAColumn = true;
            }
            else
            {
                MaxWidth = Owner.TextMaxWidthInPixel;
            }

            ListText.Clear();
            string[] ArrayWorkingString = OriginalText.Split('\n', '\r');
            Vector2 ActivePosition = Position;
            float TextWidth = 0;

            for (int i = 0; i < ArrayWorkingString.Length; ++i)
            {
                string WorkingString = ArrayWorkingString[i];
                float RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);
                int CurrentChar = WorkingString.Length;

                while (RemainingSpaceOnLine == 0)
                {
                    ActivePosition.Y += Owner.LineHeight;
                    ActivePosition.X = 0;
                    ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                    RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);
                }

                while (WorkingString.Length > 0)
                {
                    string TestString = WorkingString.Substring(0, CurrentChar);

                    TextWidth = MeasureString(TestString);

                    if (TextWidth > RemainingSpaceOnLine)
                    {
                        --CurrentChar;
                        TestString = WorkingString.Substring(0, CurrentChar);
                        TextWidth = MeasureString(TestString);

                        if (TextWidth <= RemainingSpaceOnLine)
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
                            ActivePosition.Y += Owner.LineHeight;
                            ActivePosition.X = 0;
                            ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                            RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);
                        }
                    }
                    else if (TextWidth <= RemainingSpaceOnLine)
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
                            TextWidth = MeasureString(TestString);

                            if (TextWidth > RemainingSpaceOnLine)
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
                                ActivePosition.Y += Owner.LineHeight;
                                ActivePosition.X = 0;
                                ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                                RemainingSpaceOnLine = GetRemainingSpaceOnLine(ActivePosition);
                            }
                        }
                    }
                }

                if (i + 1 < ArrayWorkingString.Length || IsAColumn)
                {
                    ActivePosition.Y += Owner.LineHeight;
                    ActivePosition.X = 0;
                    ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                }
            }

            if (IsAColumn)
            {
                Owner.ListObstacle.Add(new Rectangle((int)Offset, (int)Position.Y, (int)TextWidth, (int)(ActivePosition.Y - Position.Y)));
                if (Offset > 0)
                {
                    return new Vector2(Position.X, Position.Y + MaxHeight);
                }
                else
                {
                    return new Vector2(Position.X + MaxWidth, Position.Y + MaxHeight);
                }
            }
            else
            {
                return new Vector2(ActivePosition.X + TextWidth, ActivePosition.Y);
            }
        }

        protected virtual float MeasureString(string TextToMeasure)
        {
            return fntTextFont.MeasureString(TextToMeasure).X;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            TextHelper.DrawTextMultiline(g, fntTextFont, ListText, TextHelper.TextAligns.Left, Position.X + Offset.X + Owner.TextMaxWidthInPixel / 2, Position.Y + Offset.Y, (int)MaxWidth);
        }
    }
}
