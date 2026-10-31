using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class HealEffect : TraitEffect
    {
        private int _Amount = 4;
        private bool _Temporary;
        private bool _Halve;
        private string _Recover = "";

        public HealEffect()
            : base("Heal")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            Roll.HealAmount = Amount;
            Roll.TemporaryHeal = Temporary;
            Roll.HalveHeal = Halve;
            Roll.RecoverPersistent = Recover;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Amount);
            BW.Write(Temporary);
            BW.Write(Halve);
            BW.Write(Recover);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            HealEffect Copy = new HealEffect();
            Copy.Amount = BR.ReadInt32();
            Copy.Temporary = BR.ReadBoolean();
            Copy.Halve = BR.ReadBoolean();
            Copy.Recover = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Heal"), DefaultValueAttribute(4)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }

        [CategoryAttribute("Heal"), DefaultValueAttribute(false)]
        public bool Temporary { get { return _Temporary; } set { _Temporary = value; } }

        [CategoryAttribute("Heal"), DescriptionAttribute("Healing inside this effect is halved."), DefaultValueAttribute(false)]
        public bool Halve { get { return _Halve; } set { _Halve = value; } }

        [CategoryAttribute("Heal"), DescriptionAttribute("Persistent damage types that attempt a flat check. Separate names with a comma."), DefaultValueAttribute("")]
        public string Recover { get { return _Recover; } set { _Recover = value; } }
    }
}
