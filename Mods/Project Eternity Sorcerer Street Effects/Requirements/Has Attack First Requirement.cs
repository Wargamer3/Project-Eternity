using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetHasAttackFirstRequirement : SorcererStreetRequirement
    {
        public enum Targets { Self, Opponent, Territory }

        private Targets _Target;

        public SorcererStreetHasAttackFirstRequirement()
            : this(null)
        {
        }

        public SorcererStreetHasAttackFirstRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Has Attack First", Params)
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
                return Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).AttackFirst;
            }
            else if (_Target == Targets.Opponent)
            {
                return Params.GlobalContext.OpponentCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).AttackFirst;
            }
            else
            {
                return Params.GlobalContext.ActiveTerrain.DefendingCreature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).AttackFirst;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetHasAttackFirstRequirement NewRequirement = new SorcererStreetHasAttackFirstRequirement(Params);

            NewRequirement._Target =_Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetHasAttackFirstRequirement CopyRequirement = Copy as SorcererStreetHasAttackFirstRequirement;

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
