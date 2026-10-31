using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class FreeStrikeEffect : TraitEffect
    {
        private string _Trigger = "Targeted";

        public FreeStrikeEffect()
            : base("Free strike")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger == Trigger)
            {
                Roll.FreeStrike = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Trigger);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            FreeStrikeEffect Copy = new FreeStrikeEffect();
            Copy.Trigger = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Reaction"), DefaultValueAttribute("Targeted")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }
    }
}
