using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class VersatileEffect : TraitEffect
    {
        public enum DamageTypes { Slashing, Piercing, Bludgeoning }

        private DamageTypes _DamageType = DamageTypes.Slashing;

        public VersatileEffect()
            : base("Versatile")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null)
            {
                return;
            }

            Roll.VersatileType = _DamageType.ToString();

            if (string.IsNullOrEmpty(Roll.RequestedType))
            {
                return;
            }

            if (Roll.RequestedType == DamageType.ToString())
            {
                Roll.DamageType = _DamageType.ToString();
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write((byte)_DamageType);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            VersatileEffect Copy = new VersatileEffect();
            Copy.DamageType = (DamageTypes)BR.ReadByte();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Alternate damage type for this strike."),
        DefaultValueAttribute(DamageTypes.Slashing)]
        public DamageTypes DamageType
        {
            get { return _DamageType; }
            set { _DamageType = value; }
        }

        #endregion
    }
}
