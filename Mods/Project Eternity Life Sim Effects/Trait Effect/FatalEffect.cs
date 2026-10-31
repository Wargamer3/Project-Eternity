using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class FatalEffect : TraitEffect
    {
        private int _Faces = 12;

        public FatalEffect()
            : base("Fatal")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll == null || !Roll.Critical || Faces < 1)
            {
                return;
            }

            if (Faces > Roll.DiceFaces)
            {
                Roll.DiceFaces = Faces;
            }

            if (!Roll.FatalDieAdded)
            {
                Roll.DiceCount += 1;
                Roll.FatalDieAdded = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Faces);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            FatalEffect Copy = new FatalEffect();
            Copy.Faces = BR.ReadInt32();
            return Copy;
        }

        #region Properties

        [CategoryAttribute("Damage"),
        DescriptionAttribute("On a critical hit the weapon die grows to this size, and one extra die of that size is rolled before the damage is doubled."),
        DefaultValueAttribute(12)]
        public int Faces
        {
            get { return _Faces; }
            set { _Faces = value; }
        }

        #endregion
    }
}
