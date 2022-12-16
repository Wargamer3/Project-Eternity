using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    public class ImagelessIconPart : IconPart
    {
        public static float ImageSizeDefault = 30;

        public ImagelessIconPart(DynamicText Owner, string OriginalText)
            : base(Owner, null, OriginalText)
        {
        }

        public override float GetImageSize()
        {
            return ImageSizeDefault;
        }
    }
}
