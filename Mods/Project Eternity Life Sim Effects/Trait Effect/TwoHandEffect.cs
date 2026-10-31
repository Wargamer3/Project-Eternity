using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class TwoHandEffect : TraitEffect
    {
        private int _Faces = 12;

        public TwoHandEffect()
            : base("Two-hand")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Weapon == null)
            {
                return;
            }

            if (Roll.Weapon.HandType != Weapon.HandTypes.TwoHands)
            {
                return;
            }

            if (Faces > Roll.DiceFaces)
            {
                Roll.DiceFaces = Faces;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Faces);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            TwoHandEffect Copy = new TwoHandEffect();
            Copy.Faces = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Damage die used while the weapon is held in two hands."),
        DefaultValueAttribute(12)]
        public int Faces
        {
            get { return _Faces; }
            set { _Faces = value; }
        }

        #endregion
    }
}
