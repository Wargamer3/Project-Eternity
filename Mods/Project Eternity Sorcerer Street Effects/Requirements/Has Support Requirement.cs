using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasSupportRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent }

        private Targets _Target;

        public SorcererStreetHasSupportRequirement()
            : this(null)
        {
            _Target = Targets.Opponent;
        }

        public SorcererStreetHasSupportRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Has Support", GlobalContext)
        {
            _Target = Targets.Opponent;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasSupportRequirement NewRequirement = new SorcererStreetHasSupportRequirement(GlobalContext);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasSupportRequirement CopyRequirement = (SorcererStreetHasSupportRequirement)Copy;
            _Target = CopyRequirement._Target;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        #endregion
    }
}
