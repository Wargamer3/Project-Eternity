using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class PilotSkillFoundRequirement : PassiveSkillRequirement
    {
        public static string Name = "Pilot Skill Found Requirement";

        private string _PilotSkill;

        public PilotSkillFoundRequirement()
            : this(null)
        {
        }

        public PilotSkillFoundRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _PilotSkill = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_PilotSkill);
        }

        public override bool CanActivatePassive()
        {
            for (int S = GlobalContext.EffectTargetUnit.Pilot.ArrayPilotSkill.Length - 1; S >= 0; --S)
            {
                if (GlobalContext.EffectTargetUnit.Pilot.ArrayPilotSkill[S].Name == PilotSkill)
                {
                    return true;
                }
            }
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            PilotSkillFoundRequirement NewSkillEffect = new PilotSkillFoundRequirement(GlobalContext);

            NewSkillEffect._PilotSkill = _PilotSkill;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            PilotSkillFoundRequirement NewRequirement = (PilotSkillFoundRequirement)Copy;

            _PilotSkill = NewRequirement._PilotSkill;
        }

        #region Properties

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string PilotSkill
        {
            get { return _PilotSkill; }
            set { _PilotSkill = value; }
        }

        #endregion
    }
}
