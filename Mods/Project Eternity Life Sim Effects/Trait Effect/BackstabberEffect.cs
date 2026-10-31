using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class BackstabberEffect : TraitEffect
    {
        private int _Normal = 1;
        private int _Improved = 2;

        public BackstabberEffect()
            : base("Backstabber")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || !Roll.OffGuard)
            {
                return;
            }

            Roll.ExtraDamage += Roll.Potency >= 3 ? Improved : Normal;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Normal);
            BW.Write(Improved);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            BackstabberEffect Copy = new BackstabberEffect();
            Copy.Normal = BR.ReadInt32();
            Copy.Improved = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Precision damage when the target is off-guard."),
        DefaultValueAttribute(1)]
        public int Normal
        {
            get { return _Normal; }
            set { _Normal = value; }
        }

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Precision damage when the weapon potency is 3 or higher."),
        DefaultValueAttribute(2)]
        public int Improved
        {
            get { return _Improved; }
            set { _Improved = value; }
        }

        #endregion
    }
}
