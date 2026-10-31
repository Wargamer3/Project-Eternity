using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class StrikeBackEffect : TraitEffect
    {
        private string _Trigger = "CriticalFailure";

        public StrikeBackEffect()
            : base("Strike back")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger == Trigger)
            {
                Roll.StrikeBack = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Trigger);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            StrikeBackEffect Copy = new StrikeBackEffect();
            Copy.Trigger = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Reaction"), DefaultValueAttribute("CriticalFailure")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }
    }
}
