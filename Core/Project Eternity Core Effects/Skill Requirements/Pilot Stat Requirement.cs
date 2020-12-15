using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class PilotStatRequirement : PassiveSkillRequirement
    {
        public static string Name = "Pilot Stat Requirement";

        private string _EffectValue;
        private StatusTypes _StatusType;
        private Operators.LogicOperators _LogicOperator;

        public PilotStatRequirement()
            : this(null)
        {
        }

        public PilotStatRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
            _EffectValue = "";
        }

        public override bool CanActivatePassive()
        {
            int PilotStat = int.Parse(FormulaParser.ActiveParser.Evaluate(EffectValue), CultureInfo.InvariantCulture);
            switch (StatusType)
            {
                case StatusTypes.MEL:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.MEL, PilotStat);

                case StatusTypes.RNG:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.RNG, PilotStat);

                case StatusTypes.DEF:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.DEF, PilotStat);

                case StatusTypes.SKL:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.SKL, PilotStat);

                case StatusTypes.EVA:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.EVA, PilotStat);

                case StatusTypes.HIT:
                    return Operators.CompareValue(LogicOperator, GlobalContext.EffectOwnerUnit.Pilot.HIT, PilotStat);
            }

            return false;
        }

        protected override void Load(BinaryReader BR)
        {
            _StatusType = (StatusTypes)BR.ReadByte();
            _EffectValue = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_StatusType);
            BW.Write(_EffectValue);
        }

        public override BaseSkillRequirement Copy()
        {
            PilotStatRequirement NewSkillEffect = new PilotStatRequirement(GlobalContext);

            NewSkillEffect._EffectValue = _EffectValue;
            NewSkillEffect._StatusType = _StatusType;

            return NewSkillEffect;
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

        [TypeConverter(typeof(StatusTypeConverter)),
        CategoryAttribute("Requirement Attributes")]
        public StatusTypes StatusType
        {
            get { return _StatusType; }
            set { _StatusType = value; }
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string EffectValue
        {
            get { return _EffectValue; }
            set { _EffectValue = value; }
        }

        #endregion
    }
}
