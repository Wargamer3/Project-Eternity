﻿using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AfterGettingAttackedRequirement : DeathmatchSkillRequirement
    {
        public AfterGettingAttackedRequirement()
            : this(null)
        {
        }

        public AfterGettingAttackedRequirement(DeathmatchParams Params)
            : base(AfterGettingAttackedRequirementName, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override BaseSkillRequirement Copy()
        {
            AfterGettingAttackedRequirement NewSkillEffect = new AfterGettingAttackedRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
