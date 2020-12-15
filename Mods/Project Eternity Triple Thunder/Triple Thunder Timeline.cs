using System.IO;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class TripleThunderTimeline : VisibleTimeline
    {
        protected TripleThunderTimeline(string TimelineType, string Name)
            : base(TimelineType, Name)
        {
        }

        public TripleThunderTimeline(BinaryReader BR, string TimelineType)
            : base(BR, TimelineType)
        {
        }
    }
}
