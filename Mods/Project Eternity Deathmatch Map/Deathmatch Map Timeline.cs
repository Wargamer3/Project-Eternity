using System.IO;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchMapTimeline : VisibleTimeline
    {
        protected DeathmatchMapTimeline(string TimelineType, string Name)
            : base(TimelineType, Name)
        {
        }

        public DeathmatchMapTimeline(BinaryReader BR, string TimelineType)
            : base(BR, TimelineType)
        {
        }
    }
}
