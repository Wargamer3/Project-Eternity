using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class PilotedUnitNameRequirement : PassiveSkillRequirement
    {
        public static string Name = "Pilot Unit Name Requirement";

        private string _UnitName;

        public PilotedUnitNameRequirement()
            : this(null)
        {
        }

        public PilotedUnitNameRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _UnitName = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_UnitName);
        }

        public override bool CanActivatePassive()
        {
            if (GlobalContext.EffectTargetUnit.FullName == UnitName)
                return true;
            else
                return false;
        }

        public override BaseSkillRequirement Copy()
        {
            PilotedUnitNameRequirement NewSkillEffect = new PilotedUnitNameRequirement(GlobalContext);
            NewSkillEffect._UnitName = _UnitName;

            return NewSkillEffect;
        }

        #region Properties

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string UnitName
        {
            get { return _UnitName; }
            set { _UnitName = value; }
        }

        #endregion
    }
}
