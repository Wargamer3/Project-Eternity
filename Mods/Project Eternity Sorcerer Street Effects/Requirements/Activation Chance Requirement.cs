using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAgainstActivationChanceRequirement : SorcererStreetBattleRequirement
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
            return RandomHelper.Next(100) < _ActivationChance;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetAgainstActivationChanceRequirement NewRequirement = new SorcererStreetAgainstActivationChanceRequirement(GlobalContext);

            NewRequirement._ActivationChance = _ActivationChance;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetAgainstActivationChanceRequirement CopyRequirement = (SorcererStreetAgainstActivationChanceRequirement)Copy;

            _ActivationChance = CopyRequirement._ActivationChance;
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
