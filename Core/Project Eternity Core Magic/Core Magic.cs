using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Magic
{
    public class MagicCoreOffset : ProjectileMagicCore
    {
        public int NumberOfExecutions = 0;
        private float OffsetX;
        private float OffsetY;

        public MagicCoreOffset()
            : this(new MagicUserParams(new MagicUserContext()), new Projectile2DParams(new Projectile2DContext()))
        {
        }

        public MagicCoreOffset(MagicUserParams Params, Projectile2DParams AttackParams)
            : this(Params, AttackParams, 0, 0)
        {
        }

        public MagicCoreOffset(MagicUserParams Params, Projectile2DParams AttackParams, float OffsetX, float OffsetY)
            : base("Offset Core", 0, 0, 40, Params, AttackParams)
        {
            SetAttributes(
                  new MagicElementAttributeSlider("Offset X", OffsetX, -100d, 100d, (X) => this.OffsetX = (float)X),
                  new MagicElementAttributeSlider("Offset Y", OffsetY, -100d, 100d, (Y) => this.OffsetY = (float)Y));

            this.OffsetX = OffsetX;
            this.OffsetY = OffsetY;
        }

        public override MagicEffect GetSpellEffect()
        {
            return new OffsetEffect(MagicParams, AttackParams, this, new Vector2(OffsetX, OffsetY));
        }

        public override MagicElement Copy()
        {
            return new MagicCoreOffset(MagicParams, AttackParams, OffsetX, OffsetY);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    internal class OffsetEffect : MagicEffect
    {
        private Projectile2DParams AttackParams;
        private MagicCoreOffset Owner;
        public static int Key = 1;
        private Vector2 Offset;

        public OffsetEffect(MagicUserParams MagicParams, Projectile2DParams AttackParams, MagicCoreOffset Owner, Vector2 Offset)
            : base("Offset Effect " + Key++, false, MagicParams)
        {
            IsStacking = true;
            MaximumStack = -1;//Allow a user to have more than one effect.
            this.AttackParams = new Projectile2DParams(AttackParams);
            this.Owner = Owner;
            this.Offset = Offset;
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new OffsetEffect(MagicParams, AttackParams, Owner, Offset);
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        public override void SetupParamsBeforeCopy()
        {
        }

        protected override string DoExecuteEffect()
        {
            AttackParams.LocalContext.OwnerProjectile.Offset(Offset);

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }
    }

    internal class Fireball : MagicProjectile
    {
        private IMagicUser Parent;
        private Projectile2DParams Params;

        public Fireball(MagicUserParams MagicParams, Projectile2DParams Params, IMagicUser Parent)
            : base(MagicParams, 500)
        {
            this.Params = Params;
            this.Parent = Parent;

            Damage = 5;
            Speed = new Vector2(-(float)Math.Cos(Params.SharedParams.OwnerAngle) * 6, (float)Math.Sin(Params.SharedParams.OwnerAngle) * 6);

            Rectangle CollisionBox = new Rectangle(0, 0, 50, 32);

            float MinX = Params.SharedParams.OwnerPosition.X - CollisionBox.Width / 2f;
            float MinY = Params.SharedParams.OwnerPosition.Y - CollisionBox.Height / 2f;
            float MaxX = MinX + CollisionBox.Width;
            float MaxY = MinY + CollisionBox.Height;

            Polygon NewPolygon = new Polygon();
            NewPolygon.ArrayVertex = new Vector2[4];
            NewPolygon.ArrayVertex[0] = new Vector2(MinX, MinY);
            NewPolygon.ArrayVertex[1] = new Vector2(MinX, MaxY);
            NewPolygon.ArrayVertex[2] = new Vector2(MaxX, MaxY);
            NewPolygon.ArrayVertex[3] = new Vector2(MaxX, MinY);

            NewPolygon.ComputePerpendicularAxis();
            NewPolygon.ComputerCenter();

            Collision.ListCollisionPolygon = new List<Polygon>(1) { NewPolygon };
            MagicParams.SetMagicUser(Parent);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            MagicParams.SetMagicUser(Parent);
            Params.SharedParams.OwnerPosition = Collision.ListCollisionPolygon[0].Center;
            Params.SharedParams.OwnerAngle = (float)Math.Atan2(Speed.Y, Speed.X);
            Collision.ListCollisionPolygon[0].Offset(Speed.X, Speed.Y);
        }
    }

    public class MagicCoreFireball : ProjectileMagicCore
    {
        public int NumberOfExecutions = 0;
        public double ExtraAngleInDegrees;

        public MagicCoreFireball()
            : this(new MagicUserParams(new MagicUserContext()), new Projectile2DParams(new Projectile2DContext()))
        {
        }

        public MagicCoreFireball(MagicUserParams MagicParams, Projectile2DParams Params)
            : base("Fireball Core", 1, 10, 40, MagicParams, Params)
        {
            SetAttributes(new MagicElementAttribute[] { new MagicElementAttributeSlider("Extra Angle", 0, -180, 180, (NewAngle) => ExtraAngleInDegrees = NewAngle) });
        }

        public override MagicEffect GetSpellEffect()
        {
            return new CreateFireballEffect(ExtraAngleInDegrees, MagicParams, AttackParams, this);
        }

        public override MagicElement Copy()
        {
            return new MagicCoreFireball(MagicParams, AttackParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
            ExtraAngleInDegrees = BR.ReadDouble();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ExtraAngleInDegrees);
        }
    }

    public class CreateFireballEffect : MagicEffect
    {
        private MagicCoreFireball Owner;
        private Projectile2DParams Params;
        private double ExtraAngleInDegrees;

        public CreateFireballEffect(double ExtraAngleInDegrees, MagicUserParams MagicParams, Projectile2DParams Params, MagicCoreFireball Owner)
            : base("Fireball Effect", false, MagicParams)
        {
            this.ExtraAngleInDegrees = ExtraAngleInDegrees;
            this.Params = new Projectile2DParams(Params);
            this.Owner = Owner;

            IsStacking = true;
            MaximumStack = -1;//Allow a user to have more than one fireball.
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new CreateFireballEffect(ExtraAngleInDegrees, MagicParams, Params, Owner);
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
            Params.SharedParams.OwnerAngle += MathHelper.ToRadians((float)ExtraAngleInDegrees);

            Fireball NewFireball = new Fireball(MagicParams, Params, this);

            //Clone the following skills so they are not share by every bullets.
            NewFireball.ListActiveSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);
            foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
            {
                MagicParams.SetMagicUser(this);
                BaseAutomaticSkill NewSkill = new BaseAutomaticSkill(ActiveFollowingSkill);
                NewFireball.ListActiveSkill.Add(NewSkill);
            }

            Params.LocalContext.OwnerSandbox.AddProjectile(NewFireball);

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }
    }

    public class MagicCoreEnchantlLevel1 : MagicCore
    {
        private class CreateEnchantEffect : MagicEffect
        {
            public CreateEnchantEffect()
                : base("Enchant", false)
            {
            }

            public override bool CanActivate()
            {
                return true;
            }

            protected override BaseEffect DoCopy()
            {
                return new CreateEnchantEffect();
            }

            protected override void DoCopyMembers(BaseEffect Copy)
            {
            }

            protected override string DoExecuteEffect()
            {
                return null;
            }

            protected override void Load(BinaryReader BR)
            {
            }

            protected override void Save(BinaryWriter BW)
            {
            }
        }

        public MagicCoreEnchantlLevel1()
            : this(null)
        {

        }

        public MagicCoreEnchantlLevel1(MagicUserParams Params)
            : base("Enchant", 1, 1, 1, Params)
        {

        }

        public override MagicEffect GetSpellEffect()
        {
            return new CreateEnchantEffect();
        }

        public override MagicElement Copy()
        {
            return new MagicCoreEnchantlLevel1(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    //Use external mana source to power up the spell
    public class ChannelExternalManaSource : MagicElement
    {
        private int ChannelSpeed;

        public ChannelExternalManaSource()
            : this(null)
        {

        }

        public ChannelExternalManaSource(MagicUserParams Params)
            : base("Channel External Mana Source", false, 40, Params)
        {
        }

        public override void Compute(MagicCoreAttributes Attributes, MagicUserContext GlobalContext)
        {
            base.Compute(Attributes, GlobalContext);

            Attributes.CurrentSkill.CurrentSkillLevel.ListActivation[0].ListEffect.Add(new ChannelExternalManaEffect(MagicParams));
            Attributes.CurrentSkill.CurrentSkillLevel.ListActivation[0].ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });
        }

        public override MagicElement Copy()
        {
            return new ChannelExternalManaSource(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
            ChannelSpeed = BR.ReadInt32();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ChannelSpeed);
        }
    }

    //Use own mana source to power up the spell
    public class ChannelInternalManaSource : MagicElement
    {
        private int ChannelSpeed;

        public ChannelInternalManaSource()
            : this(null)
        {

        }

        public ChannelInternalManaSource(MagicUserParams Params)
            : base("Channel Internal Mana Source", false, 40, Params)
        {
        }

        public override void Compute(MagicCoreAttributes Attributes, MagicUserContext GlobalContext)
        {
            base.Compute(Attributes, GlobalContext);

            Attributes.CurrentSkill.CurrentSkillLevel.ListActivation[0].ListEffect.Add(new ChannelInternalManaEffect(MagicParams));
            Attributes.CurrentSkill.CurrentSkillLevel.ListActivation[0].ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });
        }

        public override MagicElement Copy()
        {
            return new ChannelInternalManaSource(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
            ChannelSpeed = BR.ReadInt32();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ChannelSpeed);
        }
    }

    public class IncreasePower : MagicElement
    {
        public IncreasePower()
            : this(null)
        {

        }

        public IncreasePower(MagicUserParams Params)
            : base("Increase Power", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new IncreasePower(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class IncreaseRotationSpeed : MagicElement
    {
        public IncreaseRotationSpeed()
            : this(null)
        {

        }

        public IncreaseRotationSpeed(MagicUserParams Params)
            : base("Increase Rotation Speed", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new IncreaseRotationSpeed(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class SetRotation : MagicElement
    {
        public SetRotation()
            : this(null)
        {

        }

        public SetRotation(MagicUserParams Params)
            : base("Set Rotation", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new SetRotation(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class IncreaseLaunchSpeed : MagicElement
    {
        public IncreaseLaunchSpeed()
            : this(null)
        {

        }

        public IncreaseLaunchSpeed(MagicUserParams Params)
            : base("Increase Launch Speed", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new IncreaseLaunchSpeed(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class IncreaseDuration : MagicElement
    {
        public IncreaseDuration()
            : this(null)
        {

        }

        public IncreaseDuration(MagicUserParams Params)
            : base("Increase Duration", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new IncreaseDuration(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class IncreaseResistance : MagicElement
    {
        public IncreaseResistance()
            : this(null)
        {

        }

        public IncreaseResistance(MagicUserParams Params)
            : base("Increase Resistance", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new IncreaseResistance(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class AlignWithParent : MagicElement
    {
        public AlignWithParent()
            : this(null)
        {

        }

        public AlignWithParent(MagicUserParams Params)
            : base("Align With Parent", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new AlignWithParent(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class StopOnCollision : MagicElement
    {
        public StopOnCollision()
            : this(null)
        {

        }

        public StopOnCollision(MagicUserParams Params)
            : base("Stop On Collision", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new StopOnCollision(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class OffsetSpawnLocation : MagicElement
    {
        public OffsetSpawnLocation()
            : this(null)
        {

        }

        public OffsetSpawnLocation(MagicUserParams Params)
            : base("Offset Spawn", false, 40, Params)
        {

        }

        public override MagicElement Copy()
        {
            return new OffsetSpawnLocation(MagicParams);
        }

        protected override void DoLoad(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }
    }

    public class TriggerAfterTimeEllapsed : MagicElement
    {
        public sealed class TimeEllapsedRequirement : BaseSkillRequirement
        {
            public static string Name = "Time Ellapsed";

            private MagicUserParams Params;
            private double TimeToWait;
            private int MaxExecutions;
            private double Delay;
            private double CreateTime;

            public TimeEllapsedRequirement(MagicUserParams Params, double TimeToWait)
                : base(Name)
            {
                this.Params = Params;

                this.TimeToWait = TimeToWait;
                MaxExecutions = 1;
                Delay = 0d;
                CreateTime = Constants.TotalGameTime;
            }

            public override bool CanActivatePassive()
            {
                if (MaxExecutions == -1 || MaxExecutions > 0)
                {
                    bool CanActivate = Constants.TotalGameTime - CreateTime >= TimeToWait;
                    if (CanActivate)
                    {
                        CreateTime = Constants.TotalGameTime + Delay;
                        if (MaxExecutions > 0)
                        {
                            --MaxExecutions;
                        }
                    }

                    return CanActivate;
                }

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
                TimeEllapsedRequirement NewSkillEffect = new TimeEllapsedRequirement(Params, TimeToWait);

                NewSkillEffect.MaxExecutions = MaxExecutions;
                NewSkillEffect.Delay = Delay;

                return NewSkillEffect;
            }

            public override void CopyMembers(BaseSkillRequirement Copy)
            {
                TimeEllapsedRequirement NewRequirement = (TimeEllapsedRequirement)Copy;

                MaxExecutions = NewRequirement.MaxExecutions;
                Delay = NewRequirement.Delay;
            }
        }

        private double SecondsToWait;

        public TriggerAfterTimeEllapsed()
            : this(null)
        {

        }

        public TriggerAfterTimeEllapsed(MagicUserParams Params)
            : this(Params, 0d)
        {
        }

        public TriggerAfterTimeEllapsed(MagicUserParams Params, double SecondsToWait)
            : base("Time Ellapsed", false, 40, Params)
        {
            SetAttributes(new MagicElementAttributeSlider("Seconds to wait", SecondsToWait, 0d, 100d, (NewValue) => this.SecondsToWait = NewValue));

            this.SecondsToWait = SecondsToWait;
        }

        public override void Compute(MagicCoreAttributes Attributes, MagicUserContext GlobalContext)
        {
            Attributes.CurrentSkill.CurrentSkillLevel.ListActivation[1].ListRequirement.Add(new TimeEllapsedRequirement(MagicParams, SecondsToWait));
        }

        public override MagicElement Copy()
        {
            return new TriggerAfterTimeEllapsed(MagicParams, SecondsToWait);
        }

        protected override void DoLoad(BinaryReader BR)
        {
            SecondsToWait = BR.ReadDouble();
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SecondsToWait);
        }
    }
}
