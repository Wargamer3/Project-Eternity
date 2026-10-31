using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class DeadlyEffect : TraitEffect
    {
        private int _Faces = 8;
        private int _Count = 1;

        public DeadlyEffect()
            : base("Deadly")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || !Roll.Critical || Faces < 1 || Count < 1)
            {
                return;
            }

            int Pool = Count * Faces;
            int Have = Roll.ExtraDiceCount * Roll.ExtraFaces;
            if (Pool > Have)
            {
                Roll.ExtraDiceCount = Count;
                Roll.ExtraFaces = Faces;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Faces);
            BW.Write(Count);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            DeadlyEffect Copy = new DeadlyEffect();
            Copy.Faces = BR.ReadInt32();
            Copy.Count = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("Size of the die added after a critical hit is doubled."),
        DefaultValueAttribute(8)]
        public int Faces
        {
            get { return _Faces; }
            set { _Faces = value; }
        }

        [CategoryAttribute("Damage"),
        DescriptionAttribute("How many of those dice to add."),
        DefaultValueAttribute(1)]
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        #endregion
    }
}
