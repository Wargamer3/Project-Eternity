using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyCreatureEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Destroy Creature";
        public enum Targets { Self, Opponent }

        private Targets _Target;

        public DestroyCreatureEffect()
            : base(Name, false)
        {
        }

        public DestroyCreatureEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
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
            return "Destroy Creature " + string.Join(",", _Target);
        }

        protected override BaseEffect DoCopy()
        {
            DestroyCreatureEffect NewEffect = new DestroyCreatureEffect(Params);

            NewEffect._Target = _Target;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
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
