using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SummonEffect : TraitEffect
    {
        private string _Entry = "Animal";

        public SummonEffect()
            : base("Summon")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.SummonEntry = Entry;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Entry);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            SummonEffect Copy = new SummonEffect();
            Copy.Entry = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Summon"), DefaultValueAttribute("Animal")]
        public string Entry { get { return _Entry; } set { _Entry = value; } }
    }
}
