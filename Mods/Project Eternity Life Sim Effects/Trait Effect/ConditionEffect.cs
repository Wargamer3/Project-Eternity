using System.IO;
using System.ComponentModel;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ConditionEffect : TraitEffect
    {
        private string _Name = "Frightened";
        private int _Value = 1;
        private int _Floor;
        private string _When = "Failure";
        private string _Scope = "Target";
        private string _Activity = "";

        public ConditionEffect()
            : base("Condition")
        {
        }

        public override void AlterAttack(StrikeRoll Roll)
        {
            if (Roll == null || Roll.Trigger == "List")
            {
                return;
            }

            if (!string.IsNullOrEmpty(Activity) && Roll.Activity != Activity)
            {
                return;
            }

            RollCondition Condition = new RollCondition();
            Condition.Name = Name;
            Condition.Value = Value;
            Condition.Floor = Floor;
            Condition.When = When;
            Condition.Scope = Scope;
            Roll.Conditions.Add(Condition);
        }

        protected override void DoWrite(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(Value);
            BW.Write(Floor);
            BW.Write(When);
            BW.Write(Scope);
            BW.Write(Activity);
        }

        public override TraitEffect LoadCopy(BinaryReader BR)
        {
            ConditionEffect Copy = new ConditionEffect();
            Copy.Name = BR.ReadString();
            Copy.Value = BR.ReadInt32();
            Copy.Floor = BR.ReadInt32();
            Copy.When = BR.ReadString();
            Copy.Scope = BR.ReadString();
            Copy.Activity = BR.ReadString();
            return Copy;
        }

        [CategoryAttribute("Condition"), DescriptionAttribute("Frightened, Enfeebled, Off-guard, Prone, Grabbed, Concealed, Muzzled, or Slowed."), DefaultValueAttribute("Frightened")]
        public string Name { get { return _Name; } set { _Name = value; } }

        [CategoryAttribute("Condition"), DefaultValueAttribute(1)]
        public int Value { get { return _Value; } set { _Value = value; } }

        [CategoryAttribute("Condition"), DescriptionAttribute("The condition cannot drop below this value."), DefaultValueAttribute(0)]
        public int Floor { get { return _Floor; } set { _Floor = value; } }

        [CategoryAttribute("Condition"), DescriptionAttribute("Hit, Failure, CriticalFailure, Critical, or Always."), DefaultValueAttribute("Failure")]
        public string When { get { return _When; } set { _When = value; } }

        [CategoryAttribute("Condition"), DescriptionAttribute("Target or Sight."), DefaultValueAttribute("Target")]
        public string Scope { get { return _Scope; } set { _Scope = value; } }

        [CategoryAttribute("Condition"), DescriptionAttribute("Only during this activity. Empty means any."), DefaultValueAttribute("")]
        public string Activity { get { return _Activity; } set { _Activity = value; } }
    }
}
