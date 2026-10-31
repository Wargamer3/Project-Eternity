using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class IgnoreCircumstanceArmorEffect : TraitEffect
    {
        private int _Amount = 2;

        public IgnoreCircumstanceArmorEffect()
            : base("Ignore circumstance armor")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null)
            {
                return;
            }

            if (Amount > Roll.IgnoreCircumstance)
            {
                Roll.IgnoreCircumstance = Amount;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Amount);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            IgnoreCircumstanceArmorEffect Copy = new IgnoreCircumstanceArmorEffect();
            Copy.Amount = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Attack"), DescriptionAttribute("Circumstance bonus to armor class this attack ignores."), DefaultValueAttribute(2)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }
    }
}
