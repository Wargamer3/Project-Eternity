using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyCardsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Destroy Cards";
        public enum Targets { Self, Opponent }
        public enum CardDestroyTypes { All, Random, Specific }

        private Targets _Target;
        private CardDestroyTypes _CardDestroyType;
        private byte _NumberOfCards;

        public DestroyCardsEffect()
            : base(Name, false)
        {
        }

        public DestroyCardsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _CardDestroyType = (CardDestroyTypes)BR.ReadByte();
            if (_CardDestroyType != CardDestroyTypes.All)
            {
                _NumberOfCards = BR.ReadByte();
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_CardDestroyType);
            if (_CardDestroyType != CardDestroyTypes.All)
            {
                BW.Write(_NumberOfCards);
            }
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
            DestroyCardsEffect NewEffect = new DestroyCardsEffect(Params);

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
        public CardDestroyTypes CardDestroyType
        {
            get
            {
                return _CardDestroyType;
            }
            set
            {
                _CardDestroyType = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("Number of cards to destroy if not using All."),
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
    }
}
