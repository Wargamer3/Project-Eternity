using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ResistanceEffect : TraitEffect
    {
        private string _Type = "Fire";
        private int _Amount = 5;
        private string _Trigger = "Fire";

        public ResistanceEffect()
            : base("Resistance")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Trigger) && Roll.Trigger != Trigger && Roll.Trigger != "Cast")
            {
                return;
            }

            Roll.GrantResistance = Amount;
            Roll.ResistanceType = Type;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Type);
            BW.Write(Amount);
            BW.Write(Trigger);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ResistanceEffect Copy = new ResistanceEffect();
            Copy.Type = BR.ReadString();
            Copy.Amount = BR.ReadInt32();
            Copy.Trigger = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Defense"), DefaultValueAttribute("Fire")]
        public string Type { get { return _Type; } set { _Type = value; } }

        [CategoryAttribute("Defense"), DefaultValueAttribute(5)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }

        [CategoryAttribute("Defense"), DescriptionAttribute("Fire, or Cast to grant it immediately."), DefaultValueAttribute("Fire")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }
    }
}
