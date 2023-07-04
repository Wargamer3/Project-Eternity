using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DrawCardsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Draw Cards";
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private byte _NumberOfCards;

        public DrawCardsEffect()
            : base(Name, false)
        {
        }

        public DrawCardsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _NumberOfCards = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write(_NumberOfCards);
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
            DrawCardsEffect NewEffect = new DrawCardsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        [CategoryAttribute(""),
        DescriptionAttribute("Number of cards to draw."),
        DefaultValueAttribute(0)]
        public byte NumberOfCards
        {
            get
            {
                return _NumberOfCards;
            }
            set
            {
                _NumberOfCards = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("Target."),
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
    }
}
