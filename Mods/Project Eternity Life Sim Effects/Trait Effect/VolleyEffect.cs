using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class VolleyEffect : TraitEffect
    {
        private int _Feet = 30;
        private int _Penalty = 2;

        public VolleyEffect()
            : base("Volley")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Weapon == null || Roll.Weapon.WeaponType != Weapon.WeaponTypes.Range)
            {
                return;
            }

            float Limit = Feet * PracticeMath.FeetToMeters * PracticeMath.UnitsPerMeter;
            if (Roll.Distance <= Limit)
            {
                Roll.RaiseCircumstancePenalty(Penalty);
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Feet);
            BW.Write(Penalty);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            VolleyEffect Copy = new VolleyEffect();
            Copy.Feet = BR.ReadInt32();
            Copy.Penalty = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Attack"),
        DescriptionAttribute("A target inside this many feet takes the penalty."),
        DefaultValueAttribute(30)]
        public int Feet
        {
            get { return _Feet; }
            set { _Feet = value; }
        }

        [CategoryAttribute("Attack"),
        DescriptionAttribute("Circumstance penalty while the target is inside that range."),
        DefaultValueAttribute(2)]
        public int Penalty
        {
            get { return _Penalty; }
            set { _Penalty = value; }
        }

        #endregion
    }
}
