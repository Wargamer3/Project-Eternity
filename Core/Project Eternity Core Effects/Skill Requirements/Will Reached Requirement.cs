using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class WillReachedRequirement : PassiveSkillRequirement
    {
        public static string Name = "Will Reached Requirement";

        private string _WillNeeded;
        private Operators.LogicOperators _LogicOperator;

        public WillReachedRequirement()
            : this(null)
        {
        }

        public WillReachedRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
            _WillNeeded = string.Empty;
        }

        public override bool CanActivatePassive()
        {
            int WillFinal = int.Parse(GlobalContext.ActiveParser.Evaluate(Will), CultureInfo.InvariantCulture);
            return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.Will, WillFinal);
        }

        protected override void Load(BinaryReader BR)
        {
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _WillNeeded = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_LogicOperator);
            BW.Write(_WillNeeded);
        }

        public override BaseSkillRequirement Copy()
        {
            WillReachedRequirement NewSkillEffect = new WillReachedRequirement(GlobalContext);
            NewSkillEffect._WillNeeded = _WillNeeded;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            WillReachedRequirement NewRequirement = (WillReachedRequirement)Copy;

            _WillNeeded = NewRequirement._WillNeeded;
        }

        #region Properties

        [TypeConverter(typeof(LogicOperatorConverter)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.LogicOperators LogicOperator
        {
            get { return _LogicOperator; }
            set { _LogicOperator = value; }
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string Will
        {
            get { return _WillNeeded; }
            set { _WillNeeded = value; }
        }

        #endregion
    }
}
