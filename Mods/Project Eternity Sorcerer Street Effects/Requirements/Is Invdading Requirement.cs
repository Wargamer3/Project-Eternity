using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetIsInvadingRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Land }

        private Targets _Target;

        public SorcererStreetIsInvadingRequirement()
            : this(null)
        {
        }

        public SorcererStreetIsInvadingRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Is Invading", GlobalContext)
        {
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
            return GlobalContext.SelfCreature == GlobalContext.Invader;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetIsInvadingRequirement NewRequirement = new SorcererStreetIsInvadingRequirement(GlobalContext);

            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetIsInvadingRequirement CopyRequirement = (SorcererStreetIsInvadingRequirement)Copy;
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
