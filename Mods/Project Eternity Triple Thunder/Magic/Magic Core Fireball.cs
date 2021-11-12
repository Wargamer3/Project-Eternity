using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen.Magic
{
    public class InvisibleFireball : HitscanBox
    {
        MagicUserParams MagicParams;
        public IMagicUser Parent;

        public InvisibleFireball(float Damage, ExplosionOptions ExplosionAttributes, RobotAnimation Owner, Vector2 Position, float Angle,
            MagicUserParams MagicParams, IMagicUser Parent)
            : base(Damage, ExplosionAttributes, Owner, Position, Angle)
        {
            this.MagicParams = MagicParams;
            this.Parent = Parent;
            MagicParams.SetMagicUser(Parent);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            MagicParams.SetMagicUser(Parent);
            base.DoUpdate(gameTime);
        }
    }

    public class InvisibleMagicCoreFireball : TripleThunderMagicCore
    {
        public int NumberOfExecutions = 0;

        public InvisibleMagicCoreFireball(MagicUserParams Params, TripleThunderAttackParams AttackParams)
            : base("Fireball Core", 1, 10, 40, Params, AttackParams)
        {

        }

        public override MagicEffect GetSpellEffect()
        {
            return new CreateFireballEffectTripleThunder(MagicParams, AttackParams, this);
        }

        public override MagicElement Copy()
        {
            return new InvisibleMagicCoreFireball(MagicParams, AttackParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class CreateFireballEffectTripleThunder : MagicEffect
    {
        private TripleThunderAttackParams AttackParams;
        private InvisibleMagicCoreFireball Owner;

        public CreateFireballEffectTripleThunder(MagicUserParams MagicParams, TripleThunderAttackParams AttackParams, InvisibleMagicCoreFireball Owner)
            : base("Fireball Effect", false, MagicParams)
        {
            IsStacking = true;
            MaximumStack = -1;//Allow a user to have more than one effect.
            this.AttackParams = AttackParams;
            this.Owner = Owner;
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new CreateFireballEffectTripleThunder(MagicParams, AttackParams, Owner);
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        public override void SetupParamsBeforeCopy()
        {
            MagicParams.SetMagicTarget(this);
        }

        protected override string DoExecuteEffect()
        {
            Owner.NumberOfExecutions++;
            InvisibleFireball NewFireball = new InvisibleFireball(5, new ExplosionOptions(), AttackParams.LocalContext.Owner, AttackParams.SharedParams.OwnerPosition, 0,
                MagicParams, this);

            //Clone the following skills so they are not share by every bullets.
            NewFireball.ListActiveSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);
            foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
            {
                AttackParams.LocalContext.Owner.SetAttackContext(NewFireball, AttackParams.LocalContext.Owner, AttackParams.SharedParams.OwnerAngle, AttackParams.SharedParams.OwnerPosition);
                MagicParams.SetMagicUser(this);
                BaseAutomaticSkill NewSkill = new BaseAutomaticSkill(ActiveFollowingSkill);
                NewFireball.ListActiveSkill.Add(NewSkill);
            }

            ListFollowingSkill = NewFireball.ListActiveSkill;
            AttackParams.LocalContext.OwnerSandbox.AddProjectile(NewFireball);

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }
    }
}
