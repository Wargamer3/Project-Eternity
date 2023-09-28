using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    /// <summary>
    /// Need to do at least 1 damage to trigger
    /// </summary>
    public sealed class InstantDeathEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Instant Death";

        private int _ActivationChance;

        public InstantDeathEffect()
            : base(Name, false)
        {
            _ActivationChance = 100;
        }

        public InstantDeathEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _ActivationChance = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_ActivationChance);
        }

        public override bool CanActivate()
        {
            return RandomHelper.Next(100) < _ActivationChance;
        }

        protected override string DoExecuteEffect()
        {
            Params.GlobalContext.OpponentCreature.InstantKill();
            return "Instant Death";
        }

        protected override BaseEffect DoCopy()
        {
            InstantDeathEffect NewEffect = new InstantDeathEffect(Params);

            NewEffect._ActivationChance = _ActivationChance;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int ActivationChance
        {
            get
            {
                return _ActivationChance;
            }
            set
            {
                _ActivationChance = Math.Max(0, Math.Min(100, value));
            }
        }

        #endregion
    }
}
