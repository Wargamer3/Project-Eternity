using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ReplaceTripEffect : TraitEffect
    {
        private string _Activity = "Slam Down";

        public ReplaceTripEffect()
            : base("Replace trip")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Activity == Activity)
            {
                Roll.TripIsCritical = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Activity);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ReplaceTripEffect Copy = new ReplaceTripEffect();
            Copy.Activity = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Activity"), DefaultValueAttribute("Slam Down")]
        public string Activity { get { return _Activity; } set { _Activity = value; } }
    }
}
