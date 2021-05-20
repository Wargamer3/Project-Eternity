using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnGroundCollisionRequirement : TripleThunderAttackRequirement
    {
        public OnGroundCollisionRequirement()
            : this(null)
        {
        }

        public OnGroundCollisionRequirement(TripleThunderAttackContext GlobalContext)
            : base(OnGroundCollisionAttackName, GlobalContext)
        {
        }

        public override bool CanActicateManually(string ManualActivationName)
        {
            return base.CanActicateManually(ManualActivationName) && GlobalContext.OwnerProjectile.Speed.X != 0 && GlobalContext.OwnerProjectile.Speed.Y != 0;
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        public override BaseSkillRequirement Copy()
        {
            OnGroundCollisionRequirement NewSkillEffect = new OnGroundCollisionRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
