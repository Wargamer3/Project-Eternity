using System.Collections.Generic;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Units
{
    public class AnimationInfo
    {
        public readonly string AnimationName;

        public AnimationInfo(string AnimationName)
        {
            this.AnimationName = AnimationName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AnimationType"></param>
        /// <returns>Timelines by Key Frames</returns>
        public virtual Dictionary<int, Timeline> GetExtraTimelines(AnimationClass NewAnimation)
        {
            return new Dictionary<int, Timeline>();
        }
    }
}
