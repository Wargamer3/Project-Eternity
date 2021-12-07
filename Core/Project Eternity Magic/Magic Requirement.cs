using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Magic
{
    public abstract class MagicRequirement : BaseSkillRequirement
    {
        protected MagicUserParams MagicParams;

        public MagicRequirement(string SkillRequirementName, MagicUserParams MagicParams)
            : base(SkillRequirementName)
        {
            if (MagicParams != null)
            {
                this.MagicParams = new MagicUserParams(MagicParams);
            }
        }
    }

    public abstract class MagicEffect : BaseEffect, IMagicUser
    {
        public float MaxMana;
        public float CurrentMana { get; set; }
        public float ChanneledMana { get; set; }
        public float ManaReserves { get; set; }
        public EffectHolder Effects { get; }
        public List<IMagicUser> ListLinkedUser { get; }
        protected MagicUserParams MagicParams;

        public MagicEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Effects = new EffectHolder();
            ListLinkedUser = new List<IMagicUser>();

        }

        public MagicEffect(string EffectTypeName, bool IsPassive, MagicUserParams MagicParams)
            : base(EffectTypeName, IsPassive)
        {
            Effects = new EffectHolder();
            ListLinkedUser = new List<IMagicUser>();

            if (MagicParams != null)
            {
                this.MagicParams = new MagicUserParams(MagicParams);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR, FormulaParser ActiveParser)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ManaChanneledRequirement : MagicRequirement
    {
        public static string Name = "Mana channeled";

        private float ManaRequired;

        public ManaChanneledRequirement(float ManaRequired, MagicUserParams MagicParams)
            : base(Name, MagicParams)
        {
            this.ManaRequired = 10;
        }

        public override bool CanActivatePassive()
        {
            return MagicParams.LocalContext.ActiveTarget.ChanneledMana >= ManaRequired;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ManaRequired);
        }

        protected override void Load(BinaryReader BR)
        {
            ManaRequired = BR.ReadSingle();
        }

        public override BaseSkillRequirement Copy()
        {
            return new ManaChanneledRequirement(ManaRequired, MagicParams);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }

    public class ChannelExternalManaEffect : MagicEffect
    {
        public static string Name = "Channel External Mana";
        
        public ChannelExternalManaEffect(MagicUserParams Params)
            : base(Name, true, Params)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new ChannelExternalManaEffect(MagicParams);
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        protected override string DoExecuteEffect()
        {
            MagicParams.LocalContext.ActiveTarget.ChanneledMana += 1;

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }
    }

    public class ChannelInternalManaEffect : MagicEffect
    {
        public static string Name = "Channel Internal Mana";

        public ChannelInternalManaEffect(MagicUserParams MagicParams)
            : base(Name, true, MagicParams)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new ChannelInternalManaEffect(MagicParams);
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        protected override string DoExecuteEffect()
        {
            //Always use reserves first.
            if (MagicParams.LocalContext.ActiveUser.ManaReserves > 0)
            {
                MagicParams.LocalContext.ActiveTarget.ChanneledMana += 1;
                MagicParams.LocalContext.ActiveUser.ManaReserves -= 1;
            }
            else if (MagicParams.LocalContext.ActiveUser.CurrentMana > 0)
            {
                MagicParams.LocalContext.ActiveTarget.ChanneledMana += 1;
                MagicParams.LocalContext.ActiveUser.CurrentMana -= 1;
            }

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }
    }

    public class EmptyChanneledManaEffect : MagicEffect
    {
        public static string Name = "Empty Channeled Mana";

        public EmptyChanneledManaEffect(MagicUserParams Params)
            : base(Name, false, Params)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override BaseEffect DoCopy()
        {
            return new EmptyChanneledManaEffect(MagicParams);
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        protected override string DoExecuteEffect()
        {
            MagicParams.LocalContext.ActiveTarget.CurrentMana = MagicParams.LocalContext.ActiveTarget.ChanneledMana;
            MagicParams.LocalContext.ActiveTarget.ChanneledMana = 0;

            return null;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }
    }
}
