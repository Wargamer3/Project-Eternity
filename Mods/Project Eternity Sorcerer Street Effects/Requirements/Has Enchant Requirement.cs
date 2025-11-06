using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasEnchantRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Territory }

        private Targets _Target;

        public SorcererStreetHasEnchantRequirement()
            : this(null)
        {
        }

        public SorcererStreetHasEnchantRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Has Enchant", Params)
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
                return Params.GlobalContext.SelfCreature.Creature.Enchant != null;
            }
            else if (_Target == Targets.Opponent)
            {
                return Params.GlobalContext.OpponentCreature.Creature.Enchant != null;
            }
            else
            {
                return Params.GlobalContext.ActiveTerrain.DefendingCreature.Enchant != null;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasEnchantRequirement NewRequirement = new SorcererStreetHasEnchantRequirement(Params);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasEnchantRequirement CopyRequirement = Copy as SorcererStreetHasEnchantRequirement;

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
