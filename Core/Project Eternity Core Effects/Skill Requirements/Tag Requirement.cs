using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class TagRequirement : PassiveSkillRequirement
    {
        public static string Name = "Tag Requirement";

        private string _Tag;

        public TagRequirement()
            : this(null)
        {
        }

        public TagRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return GlobalContext.EffectOwnerUnit.Pilot.Tags.Contains(_Tag);
        }

        protected override void Load(BinaryReader BR)
        {
            _Tag = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_Tag);
        }

        public override BaseSkillRequirement Copy()
        {
            TagRequirement NewSkillEffect = new TagRequirement(GlobalContext);
            NewSkillEffect._Tag = _Tag;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            TagRequirement NewRequirement = (TagRequirement)Copy;

            _Tag = NewRequirement._Tag;
        }

        #region Properties

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        #endregion
    }
}
