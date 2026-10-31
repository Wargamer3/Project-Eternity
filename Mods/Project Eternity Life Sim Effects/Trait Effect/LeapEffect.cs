using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LeapEffect : TraitEffect
    {
        private int _Feet = 10;

        public LeapEffect()
            : base("Leap")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.LeapFeet += Feet;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Feet);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            LeapEffect Copy = new LeapEffect();
            Copy.Feet = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Move"), DefaultValueAttribute(10)]
        public int Feet { get { return _Feet; } set { _Feet = value; } }
    }
}
