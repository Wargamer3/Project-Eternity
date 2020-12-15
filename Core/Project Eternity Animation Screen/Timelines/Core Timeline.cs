using System.IO;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public abstract class CoreTimeline : VisibleTimeline
    {
        protected CoreTimeline(string TimelineType, string Name)
            : base(TimelineType, Name)
        {
        }

        public CoreTimeline(BinaryReader BR, string TimelineType)
            : base(BR, TimelineType)
        {
        }
    }
}
