using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActivityEffect : TraitEffect
    {
        private string _Activity = "Swipe";

        public ActivityEffect()
            : base("Activity")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger == "List")
            {
                Roll.Activities.Add(Activity);
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Activity);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ActivityEffect Copy = new ActivityEffect();
            Copy.Activity = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Activity"), DefaultValueAttribute("Swipe")]
        public string Activity { get { return _Activity; } set { _Activity = value; } }
    }
}
