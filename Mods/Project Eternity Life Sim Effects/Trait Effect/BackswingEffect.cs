using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class BackswingEffect : TraitEffect
    {
        private int _Bonus = 1;

        public BackswingEffect()
            : base("Backswing")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || !Roll.MissedWithThisWeapon)
            {
                return;
            }

            Roll.RaiseCircumstanceBonus(Bonus);
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Bonus);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            BackswingEffect Copy = new BackswingEffect();
            Copy.Bonus = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Attack"),
        DescriptionAttribute("Circumstance bonus on the next attack after a miss with this weapon."),
        DefaultValueAttribute(1)]
        public int Bonus
        {
            get { return _Bonus; }
            set { _Bonus = value; }
        }

        #endregion
    }
}
