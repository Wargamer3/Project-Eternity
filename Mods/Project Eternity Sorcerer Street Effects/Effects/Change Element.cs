using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeElementEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Element";
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private ElementalAffinity _Element;

        public ChangeElementEffect()
            : base(Name, false)
        {
            _Target = Targets.Self;
            _Element = ElementalAffinity.Neutral;
        }

        public ChangeElementEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Self;
            _Element = ElementalAffinity.Neutral;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _Element = (ElementalAffinity)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_Element);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return "Change Element " + _Element;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeElementEffect NewEffect = new ChangeElementEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._Element = _Element;

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

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ElementalAffinity Element
        {
            get
            {
                return _Element;
            }
            set
            {
                _Element = value;
            }
        }

        #endregion
    }
}
