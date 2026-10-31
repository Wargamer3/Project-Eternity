using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AimEffect : TraitEffect
    {
        private string _Activity = "Deadly Aim";
        private int _Penalty;
        private int _Bonus;
        private int _Circumstance;

        public AimEffect()
            : base("Aim")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Activity != Activity)
            {
                return;
            }

            Roll.AimPenalty = Penalty;
            Roll.AimDamage = Bonus;
            Roll.RaiseCircumstanceBonus(Circumstance);
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Activity);
            BW.Write(Penalty);
            BW.Write(Bonus);
            BW.Write(Circumstance);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            AimEffect Copy = new AimEffect();
            Copy.Activity = BR.ReadString();
            Copy.Penalty = BR.ReadInt32();
            Copy.Bonus = BR.ReadInt32();
            Copy.Circumstance = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Attack"), DefaultValueAttribute("Deadly Aim")]
        public string Activity { get { return _Activity; } set { _Activity = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute(0)]
        public int Penalty { get { return _Penalty; } set { _Penalty = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute(0)]
        public int Bonus { get { return _Bonus; } set { _Bonus = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute(0)]
        public int Circumstance { get { return _Circumstance; } set { _Circumstance = value; } }
    }
}
