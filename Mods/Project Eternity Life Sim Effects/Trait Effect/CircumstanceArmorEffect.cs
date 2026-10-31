using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CircumstanceArmorEffect : TraitEffect
    {
        private int _Amount = 1;
        private string _Trigger = "Cast";
        private bool _RaiseShield;

        public CircumstanceArmorEffect()
            : base("Circumstance armor")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger != Trigger)
            {
                return;
            }

            if (Amount > Roll.GrantArmor)
            {
                Roll.GrantArmor = Amount;
            }

            if (RaiseShield)
            {
                Roll.RaiseShield = true;
            }

            if (Trigger == "Targeted" || Trigger == "Ranged")
            {
                Roll.SpendReaction = true;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Amount);
            BW.Write(Trigger);
            BW.Write(RaiseShield);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            CircumstanceArmorEffect Copy = new CircumstanceArmorEffect();
            Copy.Amount = BR.ReadInt32();
            Copy.Trigger = BR.ReadString();
            Copy.RaiseShield = BR.ReadBoolean();
            return Copy;
        }

        [CategoryAttribute("Defense"), DefaultValueAttribute(1)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }

        [CategoryAttribute("Defense"), DescriptionAttribute("Cast, Targeted, or Ranged."), DefaultValueAttribute("Cast")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }

        [CategoryAttribute("Defense"), DefaultValueAttribute(false)]
        public bool RaiseShield { get { return _RaiseShield; } set { _RaiseShield = value; } }
    }
}
