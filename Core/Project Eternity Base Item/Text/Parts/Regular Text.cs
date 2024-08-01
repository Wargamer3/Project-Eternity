using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public class RegularText : DynamicTextPart
    {
        public SpriteFont fntTextFont;

        public readonly FontsHolder Fonts;
        public readonly Dictionary<Vector2, string> DicTextByPosition;

        public new RegularText Parent;
        public float Offset = 0;
        public bool IsAColumn = false;
        public bool Rainbow;
        public bool Wave;
        public bool Centered;
        public Color TextColor;
        int CurrentValue = 0;

        public RegularText(DynamicText Owner, string OriginalText)
             : base(Owner, OriginalText, "Text:")
        {
            DicTextByPosition = new Dictionary<Vector2, string>();
        }

        public RegularText(DynamicText Owner, string OriginalText, string Prefix)
             : base(Owner, OriginalText, Prefix)
        {
            DicTextByPosition = new Dictionary<Vector2, string>();
        }

        public RegularText(DynamicText Owner, FontsHolder Fonts, string OriginalText, string Prefix)
             : base(Owner, OriginalText, Prefix)
        {
            this.Fonts = Fonts;
            DicTextByPosition = new Dictionary<Vector2, string>();
        }

        public override void SetParent(DynamicTextPart Parent)
        {
            this.Parent = Parent as RegularText;
            base.SetParent(Parent);
        }

        public override Vector2 UpdatePosition()
        {
            ReadTags();

            DicTextByPosition.Clear();
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
                            DicTextByPosition.Add(ActivePosition, TestString);
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
                            DicTextByPosition.Add(ActivePosition, TestString);
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
                                DicTextByPosition.Add(ActivePosition, TestString);
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

                if (i + 1 < ArrayWorkingString.Length || (IsAColumn && ArrayWorkingString[ArrayWorkingString.Length - 1].Length > 1))
                {
                    ActivePosition.Y += Owner.LineHeight;
                    ActivePosition.X = 0;
                    ActivePosition.X = GetStartingXPositionOnLine(ActivePosition);
                }
            }

            Vector2 SubEndPosition = new Vector2(ActivePosition.X + TextWidth, ActivePosition.Y);
            foreach (DynamicTextPart ActiveSubSection in ListSubTextSection)
            {
                ActiveSubSection.Position = SubEndPosition;
                SubEndPosition = ActiveSubSection.UpdatePosition();
            }

            ActivePosition = SubEndPosition;

            if (IsAColumn)
            {
                Owner.ListObstacle.Add(new Rectangle((int)Offset, (int)Position.Y, (int)MaxWidth, (int)(ActivePosition.Y - Position.Y)));
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
                return SubEndPosition;
            }
        }

        private void ReadTags()
        {
            IsAColumn = false;
            if (DicSubTag.ContainsKey("MaxWidth"))
            {
                MaxWidth = float.Parse(DicSubTag["MaxWidth"]);
                IsAColumn = true;
            }
            else if (Parent != null)
            {
                MaxWidth = Parent.MaxWidth;
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
            else if (Parent != null)
            {
                Offset = Parent.Offset;
            }
            else
            {
                Offset = 0;
            }

            if (DicSubTag.ContainsKey("Centered"))
            {
                Centered = true;
            }
            else if (Parent != null)
            {
                Centered = Parent.Centered;
            }
            else
            {
                Centered = false;
            }

            if (DicSubTag.ContainsKey("Rainbow"))
            {
                Rainbow = true;
            }
            else if (Parent != null)
            {
                Rainbow = Parent.Rainbow;
            }
            else
            {
                Rainbow = false;
            }

            if (DicSubTag.ContainsKey("Wave"))
            {
                Wave = true;
            }
            else if (Parent != null)
            {
                Wave = Parent.Wave;
            }
            else
            {
                Wave = false;
            }

            ReadColorTag();
            ReadFontTag();
        }

        protected virtual void ReadFontTag()
        {
            if (DicSubTag.ContainsKey("Font"))
            {
                switch (DicSubTag["Font"])
                {
                    case "16":
                        fntTextFont = Fonts.fntArial16;
                        break;

                    default:
                        fntTextFont = Fonts.GetFont(DicSubTag["Font"]);
                        break;
                }
            }
            else if (Parent != null)
            {
                fntTextFont = Parent.fntTextFont;
            }
            else
            {
                fntTextFont = Fonts.fntDefaultFont;
            }
        }

        protected void ReadColorTag()
        {
            if (DicSubTag.ContainsKey("Color"))
            {
                string[] ArrayChannel = DicSubTag["Color"].Split(',');
                TextColor = Color.FromNonPremultiplied(int.Parse(ArrayChannel[0]), int.Parse(ArrayChannel[1]), int.Parse(ArrayChannel[2]), int.Parse(ArrayChannel[3]));
            }
            else if (Parent != null)
            {
                TextColor = Parent.TextColor;
            }
            else
            {
                TextColor = Color.White;
            }
        }

        protected virtual float MeasureString(string TextToMeasure)
        {
            return fntTextFont.MeasureString(TextToMeasure).X;
        }

        public override void Update(GameTime gameTime)
        {
            ++CurrentValue;

            foreach (DynamicTextPart ActiveSubSection in ListSubTextSection)
            {
                ActiveSubSection.Update(gameTime);
            }
        }

        public static Color IncreaseHueBy(Color InputColor, float IncreaseValue)
        {
            float Hue, Saturation, Value;

            RgbToHsv(InputColor.R, InputColor.G, InputColor.B, out Hue, out Saturation, out Value);
            Hue += IncreaseValue;

            float Red, Green, Blue;

            HsvToRgb(Hue, Saturation, Value, out Red, out Green, out Blue);

            InputColor.R = (byte)Red;
            InputColor.G = (byte)Green;
            InputColor.B = (byte)Blue;

            return InputColor;
        }

        static void RgbToHsv(float Red, float Green, float Blue, out float Hue, out float Saturation, out float Value)
        {
            float Min = Math.Min(Math.Min(Red, Green), Blue);
            float Max = Math.Max(Math.Max(Red, Green), Blue);
            Value = Max;
            float Delta = Max - Min;

            if (Max != 0)
            {
                Saturation = Delta / Max;

                if (Red == Max)
                {
                    Hue = (Green - Blue) / Delta;// between yellow & magenta
                }
                else if (Green == Max)
                {
                    Hue = 2 + (Blue - Red) / Delta;// between cyan & yellow
                }
                else
                {
                    Hue = 4 + (Red - Green) / Delta;// between magenta & cyan
                }

                Hue *= 60;// degrees

                if (Hue < 0)
                {
                    Hue += 360;
                }
            }
            else
            {
                Saturation = 0;
                Hue = -1;
            }
        }

        static void HsvToRgb(float Hue, float Saturation, float Value, out float Red, out float Green, out float Blue)
        {
            // Keeps h from going over 360
            Hue = Hue - ((int)(Hue / 360) * 360);

            if (Saturation == 0)
            {
                // achromatic (grey)
                Red = Green = Blue = Value;
                return;
            }

            float NormalizedHue = Hue / 60;// sector 0 to 5

            int Sector = (int)NormalizedHue;
            float HueFraction = NormalizedHue - Sector;
            float p = Value * (1 - Saturation);
            float q = Value * (1 - Saturation * HueFraction);
            float t = Value * (1 - Saturation * (1 - HueFraction));
            switch (Sector)
            {
                case 0:
                    Red = Value;
                    Green = t;
                    Blue = p;
                    break;
                case 1:
                    Red = q;
                    Green = Value;
                    Blue = p;
                    break;
                case 2:
                    Red = p;
                    Green = Value;
                    Blue = t;
                    break;
                case 3:
                    Red = p;
                    Green = q;
                    Blue = Value;
                    break;
                case 4:
                    Red = t;
                    Green = p;
                    Blue = Value;
                    break;
                default:       // case 5:
                    Red = Value;
                    Green = p;
                    Blue = q;
                    break;
            }
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Offset)
        {
            if (Rainbow || Wave)
            {
                foreach (KeyValuePair<Vector2, string> ActiveLine in DicTextByPosition)
                {
                    for (int i = 0; i < ActiveLine.Value.Length; ++i)
                    {
                        string ActiveChar = ActiveLine.Value.Substring(i, 1);
                        Color CurrentColor = Color.White;
                        if (Rainbow)
                        {
                            float ColorValue = CurrentValue + i * 30;
                            CurrentColor = IncreaseHueBy(Color.Red, ColorValue);
                        }

                        float YOffset = 0;

                        if (Wave)
                        {
                            YOffset = (float)Math.Sin(i + (CurrentValue / 10f)) * 10;
                        }

                        g.DrawString(fntTextFont, ActiveChar, ActiveLine.Key + Offset + new Vector2(fntTextFont.MeasureString(ActiveLine.Value.Substring(0, i)).X, YOffset), CurrentColor);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<Vector2, string> ActiveLine in DicTextByPosition)
                {
                    if (Centered)
                    {
                        g.DrawStringCentered(fntTextFont, ActiveLine.Value, ActiveLine.Key + Offset, TextColor);
                    }
                    else
                    {
                        g.DrawString(fntTextFont, ActiveLine.Value, ActiveLine.Key + Offset, TextColor);
                    }
                }
            }

            foreach (DynamicTextPart ActiveSubSection in ListSubTextSection)
            {
                ActiveSubSection.Draw(g, Offset);
            }
        }
    }
}
