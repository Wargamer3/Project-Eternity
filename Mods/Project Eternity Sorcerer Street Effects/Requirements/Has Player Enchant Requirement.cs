using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasPlayerEnchantRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Territory }

        private Targets _Target;

        public SorcererStreetHasPlayerEnchantRequirement()
            : this(null)
        {
        }

        public SorcererStreetHasPlayerEnchantRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Has Player Enchant", GlobalContext)
        {
            _Target = Targets.Territory;
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
            if (_Target == Targets.Self)
            {
                return GlobalContext.SelfCreature.Owner.Enchant != null;
            }
            else if (_Target == Targets.Opponent)
            {
                return GlobalContext.OpponentCreature.Owner.Enchant != null;
            }
            else
            {
                return GlobalContext.ActiveTerrain.PlayerOwner.Enchant != null;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasPlayerEnchantRequirement NewRequirement = new SorcererStreetHasPlayerEnchantRequirement(GlobalContext);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasPlayerEnchantRequirement CopyRequirement = Copy as SorcererStreetHasPlayerEnchantRequirement;

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
