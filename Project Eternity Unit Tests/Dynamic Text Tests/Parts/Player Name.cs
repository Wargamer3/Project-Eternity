using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Item
{
    public class PlayerNamePart : RegularText
    {
        public PlayerNamePart(DynamicText Owner, string OriginalText)
             : base(Owner, OriginalText, "Player:")
        {
        }

        public override void OnTextRead(string TextRead)
        {
            if (TextRead == "Self")
            {
                OriginalText = "Success";
            }
            else if (TextRead == "Ally")
            {
                OriginalText = "Friend";
            }
            else
            {
                OriginalText = TextRead;
            }
        }

        protected override float MeasureString(string TextToMeasure)
        {
            return 16 * TextToMeasure.Length;
        }
    }
}
