
using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GainSymbolsEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent }
        public enum SymbolTypeTypes { Terrain, Random, Specific }

        public static string Name = "Sorcerer Street Gain Symbols";

        private Targets _Target;
        private SymbolTypeTypes _SymbolTypeType;
        private byte _NumberOfSymbols;

        public GainSymbolsEffect()
            : base(Name, false)
        {
        }

        public GainSymbolsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _SymbolTypeType = (SymbolTypeTypes)BR.ReadByte();
            _NumberOfSymbols = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_SymbolTypeType);
            BW.Write(_NumberOfSymbols);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            GainSymbolsEffect NewEffect = new GainSymbolsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        [CategoryAttribute(""),
        DescriptionAttribute("The Target."),
        DefaultValueAttribute(0)]
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

        [CategoryAttribute(""),
        DescriptionAttribute("How to destroy cards."),
        DefaultValueAttribute(0)]
        public SymbolTypeTypes SymbolTypeType
        {
            get
            {
                return _SymbolTypeType;
            }
            set
            {
                _SymbolTypeType = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("Number of symbols to gain."),
        DefaultValueAttribute(0)]
        public byte NumberOfSymbols
        {
            get
            {
                return _NumberOfSymbols;
            }
            set
            {
                _NumberOfSymbols = value;
            }
        }
    }
}
