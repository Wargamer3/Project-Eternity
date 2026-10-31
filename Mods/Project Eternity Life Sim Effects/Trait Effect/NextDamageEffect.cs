using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class NextDamageEffect : TraitEffect
    {
        private int _Amount = 2;
        private string _Group = "Crossbow";
        private string _Trigger = "Hunt Prey";

        public NextDamageEffect()
            : base("Next damage")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll != null && Roll.Trigger == Trigger)
            {
                Roll.NextDamage = Amount;
                Roll.NextDamageGroup = Group;
            }
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Amount);
            BW.Write(Group);
            BW.Write(Trigger);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            NextDamageEffect Copy = new NextDamageEffect();
            Copy.Amount = BR.ReadInt32();
            Copy.Group = BR.ReadString();
            Copy.Trigger = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Attack"), DefaultValueAttribute(2)]
        public int Amount { get { return _Amount; } set { _Amount = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute("Crossbow")]
        public string Group { get { return _Group; } set { _Group = value; } }

        [CategoryAttribute("Attack"), DefaultValueAttribute("Hunt Prey")]
        public string Trigger { get { return _Trigger; } set { _Trigger = value; } }
    }
}
