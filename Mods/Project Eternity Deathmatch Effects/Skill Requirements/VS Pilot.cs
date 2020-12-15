using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSPilotRequirement : DeathmatchSkillRequirement
    {
        private string _PilotName;

        public VSPilotRequirement()
            : this(null)
        {
        }

        public VSPilotRequirement(DeathmatchContext Context)
            : base(VSPilotRequirementName, Context)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _PilotName = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_PilotName);
        }

        public override bool CanActivatePassive()
        {
            return Context.EffectTargetUnit != null && Context.EffectTargetUnit.Pilot != null && Context.EffectTargetUnit.Pilot.Name == _PilotName;
        }

        public override BaseSkillRequirement Copy()
        {
            VSPilotRequirement NewSkillEffect = new VSPilotRequirement(Context);

            NewSkillEffect._PilotName = _PilotName;

            return NewSkillEffect;
        }
        
        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string PilotName
        {
            get { return _PilotName; }
            set { _PilotName = value; }
        }
    }
}
