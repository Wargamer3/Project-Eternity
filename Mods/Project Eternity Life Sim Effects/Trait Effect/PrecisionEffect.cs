using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PrecisionEffect : TraitEffect
    {
        private int _Count = 1;
        private int _Faces = 6;

        public PrecisionEffect()
            : base("Precision")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll != null && Roll.OffGuard)
            {
                Roll.ExtraDamage += Count * Faces;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Count);
            BW.Write(Faces);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            PrecisionEffect Copy = new PrecisionEffect();
            Copy.Count = BR.ReadInt32();
            Copy.Faces = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Damage"), DefaultValueAttribute(1)]
        public int Count { get { return _Count; } set { _Count = value; } }

        [CategoryAttribute("Damage"), DefaultValueAttribute(6)]
        public int Faces { get { return _Faces; } set { _Faces = value; } }
    }
}
