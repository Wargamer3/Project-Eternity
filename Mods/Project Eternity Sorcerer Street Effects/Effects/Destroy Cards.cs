using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyCardsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Destroy Cards";
        public enum Targets { Self, Opponent }
        public enum CardDestroyTypes { All, Random, Specific, SameAsDefeated }

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
            if (_CardDestroyType != CardDestroyTypes.All && _CardDestroyType != CardDestroyTypes.SameAsDefeated)
            {
                _NumberOfCards = BR.ReadByte();
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_CardDestroyType);
            if (_CardDestroyType != CardDestroyTypes.All && _CardDestroyType != CardDestroyTypes.SameAsDefeated)
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
            SorcererStreetBattleContext.BattleCreatureInfo RealTarget = Params.GlobalContext.SelfCreature;
            if (_Target == Targets.Opponent)
            {
                RealTarget = Params.GlobalContext.OpponentCreature;
            }

            if (_CardDestroyType == CardDestroyTypes.All)
            {
                while (RealTarget.Owner.ListCardInHand.Count > 0)
                {
                    RealTarget.Owner.ListCardInHand.RemoveAt(0);
                }
                return "Destroy All Cards";
            }
            else if (_CardDestroyType == CardDestroyTypes.Random)
            {
                for (int C = 0; C < _NumberOfCards && RealTarget.Owner.ListCardInHand.Count > 0; ++C)
                {
                    RealTarget.Owner.ListCardInHand.RemoveAt(RandomHelper.Next(RealTarget.Owner.ListCardInHand.Count));
                }
            }
            else if (_CardDestroyType == CardDestroyTypes.Specific)
            {//User chooses a card from target Cepter's hand and destroys it.
            }

            return "Destroy " + _NumberOfCards + " Cards";
        }

        protected override BaseEffect DoCopy()
        {
            DestroyCardsEffect NewEffect = new DestroyCardsEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._CardDestroyType = _CardDestroyType;
            NewEffect._NumberOfCards = _NumberOfCards;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DestroyCardsEffect NewEffect = (DestroyCardsEffect)Copy;

            _Target = NewEffect._Target;
            _CardDestroyType = NewEffect._CardDestroyType;
            _NumberOfCards = NewEffect._NumberOfCards;
        }

        #region Properties

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

        #endregion
    }
}
