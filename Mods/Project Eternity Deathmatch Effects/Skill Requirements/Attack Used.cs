using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AttackUsedRequirement : DeathmatchSkillRequirement
    {
        private string _AttackName;

        public AttackUsedRequirement()
            : this(null)
        {
        }

        public AttackUsedRequirement(DeathmatchContext Context)
            : base(AttackUsedRequirementName, Context)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _AttackName = BR.ReadString();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_AttackName);
        }

        public override bool CanActivatePassive()
        {
            return Context.EffectOwnerUnit != null && Context.EffectOwnerUnit.CurrentAttack != null && Context.EffectOwnerUnit.CurrentAttack.RelativePath == _AttackName;
        }

        public override BaseSkillRequirement Copy()
        {
            AttackUsedRequirement NewSkillEffect = new AttackUsedRequirement(Context);

            NewSkillEffect._AttackName = _AttackName;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            AttackUsedRequirement NewRequirement = (AttackUsedRequirement)Copy;

            _AttackName = NewRequirement._AttackName;
        }

        [CategoryAttribute("Requirement Attributes"),
        DescriptionAttribute(".")]
        public string AttackName
        {
            get { return _AttackName; }
            set { _AttackName = value; }
        }
    }
}
