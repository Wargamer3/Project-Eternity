using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AttackDefendedRequirement : DeathmatchSkillRequirement
    {
        private string _AttackName;

        public AttackDefendedRequirement()
            : this(null)
        {
        }

        public AttackDefendedRequirement(DeathmatchContext Context)
            : base(AttackDefendedRequirementName, Context)
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
            return Context.EffectTargetUnit != null && Context.EffectTargetUnit.CurrentAttack != null && Context.EffectTargetUnit.CurrentAttack.RelativePath == _AttackName;
        }

        public override BaseSkillRequirement Copy()
        {
            AttackDefendedRequirement NewSkillEffect = new AttackDefendedRequirement(Context);

            NewSkillEffect._AttackName = _AttackName;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            AttackDefendedRequirement NewRequirement = (AttackDefendedRequirement)Copy;

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
