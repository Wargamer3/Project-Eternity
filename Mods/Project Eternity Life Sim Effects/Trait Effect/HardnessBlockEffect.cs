using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class HardnessBlockEffect : TraitEffect
    {
        private int _Hardness = 5;
        private int _AtRank3;
        private int _AtRank5;
        private int _Step;
        private int _StepBonus;

        public HardnessBlockEffect()
            : base("Hardness block")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            int Value = Hardness;
            if (Step > 0 && Roll.SpellRank > 1)
            {
                Value += StepBonus * ((Roll.SpellRank - 1) / Step);
            }

            if (Roll.SpellRank >= 3 && AtRank3 > 0)
            {
                Value = AtRank3;
            }

            if (Roll.SpellRank >= 5 && AtRank5 > 0)
            {
                Value = AtRank5;
            }

            Roll.GrantHardness = Value;
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Hardness);
            BW.Write(AtRank3);
            BW.Write(AtRank5);
            BW.Write(Step);
            BW.Write(StepBonus);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            HardnessBlockEffect Copy = new HardnessBlockEffect();
            Copy.Hardness = BR.ReadInt32();
            Copy.AtRank3 = BR.ReadInt32();
            Copy.AtRank5 = BR.ReadInt32();
            Copy.Step = BR.ReadInt32();
            Copy.StepBonus = BR.ReadInt32();
            return Copy;
        }

        [CategoryAttribute("Defense"), DefaultValueAttribute(5)]
        public int Hardness { get { return _Hardness; } set { _Hardness = value; } }

        [CategoryAttribute("Defense"), DefaultValueAttribute(0)]
        public int AtRank3 { get { return _AtRank3; } set { _AtRank3 = value; } }

        [CategoryAttribute("Defense"), DefaultValueAttribute(0)]
        public int AtRank5 { get { return _AtRank5; } set { _AtRank5 = value; } }

        [CategoryAttribute("Defense"), DescriptionAttribute("Ranks between hardness increases."), DefaultValueAttribute(0)]
        public int Step { get { return _Step; } set { _Step = value; } }

        [CategoryAttribute("Defense"), DefaultValueAttribute(0)]
        public int StepBonus { get { return _StepBonus; } set { _StepBonus = value; } }
    }
}
