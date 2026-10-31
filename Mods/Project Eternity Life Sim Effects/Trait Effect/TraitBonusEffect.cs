using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class TraitBonusEffect : TraitEffect
    {
        private string _CreatureTrait = "Fey";
        private int _Amount = 1;

        public TraitBonusEffect()
            : base("Trait bonus")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.TargetTrait == CreatureTrait)
            {
                Roll.RaiseCircumstanceBonus(Amount);
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(CreatureTrait);
            BW.Write(Amount);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            TraitBonusEffect Copy = new TraitBonusEffect();
            Copy.CreatureTrait = BR.ReadString();
            Copy.Amount = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Attack"), DefaultValueAttribute("Fey")]
        public string CreatureTrait { get { return _CreatureTrait; } set { _CreatureTrait = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute(1)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }
    }
}
