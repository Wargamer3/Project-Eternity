using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AutomaticDamageEffect : TraitEffect
    {
        private int _Count = 1;

        public AutomaticDamageEffect()
            : base("Automatic damage")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            Roll.Automatic = true;
            Roll.PerAction = 1;
            Roll.AutomaticCount = Count;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Count);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            AutomaticDamageEffect Copy = new AutomaticDamageEffect();
            Copy.Count = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Damage"), DescriptionAttribute("Packets applied for each action spent."), DefaultValueAttribute(1)]
        public int Count { get { return _Count; } set { _Count = value; } }
    }
}
