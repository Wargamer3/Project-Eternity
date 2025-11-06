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

        public SorcererStreetHasSupportRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Has Support", Params)
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
            return Params.GlobalContext.SelfCreature.BonusST > 0;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasSupportRequirement NewRequirement = new SorcererStreetHasSupportRequirement(Params);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasSupportRequirement CopyRequirement = Copy as SorcererStreetHasSupportRequirement;

            if (CopyRequirement != null)
            {
                _Target = CopyRequirement._Target;
            }
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
