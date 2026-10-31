using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ForcefulEffect : TraitEffect
    {
        private int _Second = 1;
        private int _Later = 2;

        public ForcefulEffect()
            : base("Forceful")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || Roll.StrikesWithThisWeapon <= 0)
            {
                return;
            }

            int Multiplier = Roll.StrikesWithThisWeapon == 1 ? Second : Later;
            Roll.ExtraDamage += Roll.DiceCount * Multiplier;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Second);
            BW.Write(Later);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ForcefulEffect Copy = new ForcefulEffect();
            Copy.Second = BR.ReadInt32();
            Copy.Later = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Extra damage per weapon die on the second attack with this weapon."),
        DefaultValueAttribute(1)]
        public int Second
        {
            get { return _Second; }
            set { _Second = value; }
        }

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Extra damage per weapon die on later attacks with this weapon."),
        DefaultValueAttribute(2)]
        public int Later
        {
            get { return _Later; }
            set { _Later = value; }
        }

        #endregion
    }
}
