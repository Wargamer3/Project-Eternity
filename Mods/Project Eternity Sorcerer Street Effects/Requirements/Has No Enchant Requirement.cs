using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasNoEnchantRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Territory }

        private Targets _Target;

        public SorcererStreetHasNoEnchantRequirement()
            : this(null)
        {
        }

        public SorcererStreetHasNoEnchantRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Has No Enchant", GlobalContext)
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
                return GlobalContext.SelfCreature.Creature.Enchant == null;
            }
            else if (_Target == Targets.Opponent)
            {
                return GlobalContext.OpponentCreature.Creature.Enchant == null;
            }
            else
            {
                return GlobalContext.Defender.Creature.Enchant == null;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasNoEnchantRequirement NewRequirement = new SorcererStreetHasNoEnchantRequirement(GlobalContext);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasNoEnchantRequirement CopyRequirement = (SorcererStreetHasNoEnchantRequirement)Copy;
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
