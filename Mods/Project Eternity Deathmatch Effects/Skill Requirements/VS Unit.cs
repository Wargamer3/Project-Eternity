using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSUnitRequirement : DeathmatchSkillRequirement
    {
        private string _UnitName;
        private bool _Not;

        public VSUnitRequirement()
            : this(null)
        {
        }

        public VSUnitRequirement(DeathmatchContext Context)
            : base(VSUnitRequirementName, Context)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _UnitName = BR.ReadString();
            _Not = BR.ReadBoolean();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_UnitName);
            BW.Write(_Not);
        }

        public override bool CanActivatePassive()
        {
            return Context.EffectTargetUnit != null && Context.EffectTargetUnit.FullName == _UnitName;
        }

        public override BaseSkillRequirement Copy()
        {
            VSUnitRequirement NewSkillEffect = new VSUnitRequirement(Context);

            NewSkillEffect._UnitName = _UnitName;
            NewSkillEffect._Not = _Not;

            return NewSkillEffect;
        }
        
        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string UnitName
        {
            get { return _UnitName; }
            set { _UnitName = value; }
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute("If false, will activate when VS the specified Unit. If true, will activate when VS a Unit that's not the specified Unit")]
        public bool Not
        {
            get { return _Not; }
            set { _Not = value; }
        }
    }
}
