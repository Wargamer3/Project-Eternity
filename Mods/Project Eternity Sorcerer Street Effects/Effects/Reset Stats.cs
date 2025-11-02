using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ResetStatsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Reset Stats";
        public enum Targets { Self, Opponent }

        private Targets _Target;

        public ResetStatsEffect()
            : base(Name, false)
        {
            _Target = Targets.Self;
        }

        public ResetStatsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Self;
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            CreatureCard FinalTarget;
            if (_Target == Targets.Self)
            {
                FinalTarget = Params.GlobalContext.SelfCreature.Creature;
            }
            else
            {
                FinalTarget = Params.GlobalContext.OpponentCreature.Creature;
            }

            FinalTarget.CurrentST = FinalTarget.OriginalST;
            FinalTarget.CurrentHP = FinalTarget.MaxHP = FinalTarget.OriginalMaxHP;

            return "Swapped HP and ST";
        }

        protected override BaseEffect DoCopy()
        {
            ResetStatsEffect NewEffect = new ResetStatsEffect(Params);

            NewEffect._Target = _Target;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ResetStatsEffect CopyRequirement = (ResetStatsEffect)Copy;

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
