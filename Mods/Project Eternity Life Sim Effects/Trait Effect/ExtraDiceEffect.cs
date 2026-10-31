using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ExtraDiceEffect : TraitEffect
    {
        private int _Count = 1;
        private string _Activity = "Vicious Swing";

        public ExtraDiceEffect()
            : base("Extra dice")
        {
        }

        public override void AlterDamage(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Activity == Activity)
            {
                Roll.DiceCount += Count;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Count);
            BW.Write(Activity);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ExtraDiceEffect Copy = new ExtraDiceEffect();
            Copy.Count = BR.ReadInt32();
            Copy.Activity = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Damage"), DefaultValueAttribute(1)]
        public int Count { get { return _Count; } set { _Count = value; } }

        [CategoryAttribute("Damage"), DefaultValueAttribute("Vicious Swing")]
        public string Activity { get { return _Activity; } set { _Activity = value; } }
    }
}
