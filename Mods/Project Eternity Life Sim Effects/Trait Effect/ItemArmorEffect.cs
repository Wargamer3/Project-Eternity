using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ItemArmorEffect : TraitEffect
    {
        private int _Amount = 1;

        public ItemArmorEffect()
            : base("Item armor")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger != "List")
            {
                Roll.GrantItem = Amount;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Amount);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ItemArmorEffect Copy = new ItemArmorEffect();
            Copy.Amount = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Defense"), DefaultValueAttribute(1)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }
    }
}
