using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAgainstActivationChanceRequirement : SorcererStreetRequirement
    {
        private byte _ActivationChance;

        public SorcererStreetAgainstActivationChanceRequirement()
            : this(null)
        {
        }

        public SorcererStreetAgainstActivationChanceRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Activation Chance", GlobalContext)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_ActivationChance);
        }

        protected override void Load(BinaryReader BR)
        {
            _ActivationChance = BR.ReadByte();
        }

        public override bool CanActivatePassive()
        {
            return true;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetAgainstActivationChanceRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public byte ActivationChance
        {
            get
            {
                return _ActivationChance;
            }
            set
            {
                _ActivationChance = value;
            }
        }
    }
}
