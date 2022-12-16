using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class FontlessText : RegularText
    {
        public static float CharacterWidthDefault = 10;
        public static float CharacterWidthSize16 = 16;

        public new FontlessText Parent;
        public float CharacterWidthSize;

        public FontlessText(DynamicText Owner, string OriginalText)
            : base(Owner, OriginalText)
        {
        }

        public FontlessText(DynamicText Owner, string OriginalText, string Prefix)
            : base(Owner, OriginalText, Prefix)
        {
        }

        public override void SetParent(DynamicTextPart Parent)
        {
            this.Parent = Parent as FontlessText;
            base.SetParent(Parent);
        }

        protected override void ReadFontTag()
        {
            if (DicSubTag.ContainsKey("Font"))
            {
                switch (DicSubTag["Font"])
                {
                    case "16":
                        CharacterWidthSize = CharacterWidthSize16;
                        break;

                    default:
                        CharacterWidthSize = CharacterWidthDefault;
                        break;
                }
            }
            else if (Parent != null)
            {
                CharacterWidthSize = Parent.CharacterWidthSize;
            }
            else
            {
                CharacterWidthSize = CharacterWidthDefault;
            }
        }

        protected override float MeasureString(string TextToMeasure)
        {
            return CharacterWidthSize * TextToMeasure.Length;
        }
    }
}
