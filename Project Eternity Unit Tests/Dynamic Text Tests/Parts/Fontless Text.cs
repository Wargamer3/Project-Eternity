using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class FontlessText : RegularText
    {
        public FontlessText(DynamicText Owner, string OriginalText)
            : base(Owner, OriginalText)
        {
        }

        public FontlessText(DynamicText Owner, string OriginalText, string Prefix)
            : base(Owner, OriginalText, Prefix)
        {
        }

        protected override float MeasureString(string TextToMeasure)
        {
            return 10 * TextToMeasure.Length;
        }
    }
}
