using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasTerritoryAbilityRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Territory }

        private Targets _Target;

        public SorcererStreetHasTerritoryAbilityRequirement()
            : this(null)
        {
        }

        public SorcererStreetHasTerritoryAbilityRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Has Territory Ability", Params)
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
                return Params.GlobalContext.SelfCreature.Creature.TerritoryAbility != null;
            }
            else if (_Target == Targets.Opponent)
            {
                return Params.GlobalContext.OpponentCreature.Creature.TerritoryAbility != null;
            }
            else
            {
                return Params.GlobalContext.ActiveTerrain.DefendingCreature.TerritoryAbility != null;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasTerritoryAbilityRequirement NewRequirement = new SorcererStreetHasTerritoryAbilityRequirement(Params);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasTerritoryAbilityRequirement CopyRequirement = Copy as SorcererStreetHasTerritoryAbilityRequirement;

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
