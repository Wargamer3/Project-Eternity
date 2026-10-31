using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SpellAttackEffect : TraitEffect
    {
        private float _RangeFeet = 30f;

        public SpellAttackEffect()
            : base("Spell attack")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            Roll.IsSpellAttack = true;
            Roll.RangeFeet = RangeFeet;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(RangeFeet);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            SpellAttackEffect Copy = new SpellAttackEffect();
            Copy.RangeFeet = BR.ReadSingle();
            return Copy;
        }

        [CategoryAttribute("Attack"), DescriptionAttribute("Maximum range in feet."), DefaultValueAttribute(30f)]
        public float RangeFeet { get { return _RangeFeet; } set { _RangeFeet = value; } }
    }
}
