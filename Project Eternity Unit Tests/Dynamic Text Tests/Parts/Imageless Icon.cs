using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class ImagelessIconPart : IconPart
    {
        public ImagelessIconPart(DynamicText Owner, string OriginalText)
            : base(Owner, null, OriginalText)
        {
        }

        public override float GetImageSize()
        {
            return 30;
        }
    }
}
