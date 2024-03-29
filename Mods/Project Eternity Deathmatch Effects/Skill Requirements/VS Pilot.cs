﻿using System.IO;
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

        public VSPilotRequirement(DeathmatchParams Params)
            : base(VSPilotRequirementName, Params)
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
            return Params.GlobalContext.EffectTargetUnit != null && Params.GlobalContext.EffectTargetUnit.Pilot != null && Params.GlobalContext.EffectTargetUnit.Pilot.Name == _PilotName;
        }

        public override BaseSkillRequirement Copy()
        {
            VSPilotRequirement NewSkillEffect = new VSPilotRequirement(Params);

            NewSkillEffect._PilotName = _PilotName;

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            VSPilotRequirement NewRequirement = (VSPilotRequirement)Copy;

            _PilotName = NewRequirement._PilotName;
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
