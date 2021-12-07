using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class HPLeftRequirement : PassiveSkillRequirement
    {
        public static string Name = "HP Left Requirement";

        private string _HPLeft;
        private Operators.LogicOperators _LogicOperator;
        private Operators.NumberTypes _NumberType;

        public HPLeftRequirement()
            : this(null)
        {
        }

        public HPLeftRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
            _HPLeft = "";
        }

        public override bool CanActivatePassive()
        {
            if (_NumberType == Operators.NumberTypes.Absolute)
            {
                int HPLeftFinal = int.Parse(GlobalContext.ActiveParser.Evaluate(HPLeft), CultureInfo.InvariantCulture);
                return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.HP, HPLeftFinal);
            }
            else
            {
                int ExpectedHPPercent = int.Parse(GlobalContext.ActiveParser.Evaluate(HPLeft), CultureInfo.InvariantCulture);
                int HPPercent = (GlobalContext.EffectOwnerUnit.HP * 100) / GlobalContext.EffectOwnerUnit.MaxHP;
                return Operators.CompareValue(LogicOperator, HPPercent, ExpectedHPPercent);
            }
        }

        protected override void Load(BinaryReader BR)
        {
            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _NumberType = (Operators.NumberTypes)BR.ReadByte();
            _HPLeft = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_LogicOperator);
            BW.Write((byte)_NumberType);
            BW.Write(_HPLeft);
        }

        public override BaseSkillRequirement Copy()
        {
            HPLeftRequirement NewSkillEffect = new HPLeftRequirement(GlobalContext);
            NewSkillEffect._LogicOperator = _LogicOperator;
            NewSkillEffect._NumberType = _NumberType;
            NewSkillEffect._HPLeft = _HPLeft;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            HPLeftRequirement NewRequirement = (HPLeftRequirement)Copy;

            _LogicOperator = NewRequirement._LogicOperator;
            _NumberType = NewRequirement._NumberType;
            _HPLeft = NewRequirement._HPLeft;
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

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public Operators.NumberTypes NumberType
        {
            get { return _NumberType; }
            set { _NumberType = value; }
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string HPLeft
        {
            get { return _HPLeft; }
            set { _HPLeft = value; }
        }

        #endregion
    }
}
