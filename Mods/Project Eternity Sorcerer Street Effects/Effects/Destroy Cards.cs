using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyCardsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Destroy Cards";
        public enum Targets { Self, Opponent }
        public enum CardDestroyTypes { All, Random, Specific, SameAsDefeated, DuplicateGlobal }
        public enum CardTypes { All, Creature, Item, Armor, Weapon, Scroll, Tool, Spell }
        public enum CardLocations { Hand, Book, Both }

        private Targets _Target;
        private CardDestroyTypes _CardDestroyType;
        private CardTypes _CardType;
        private CardLocations _CardLocation;
        private bool _RedrawCards;
        private byte _NumberOfCards;
        private Operators.LogicOperators _LogicOperator;
        private int _MagicCost;

        public DestroyCardsEffect()
            : base(Name, false)
        {
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
        }

        public DestroyCardsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _CardDestroyType = (CardDestroyTypes)BR.ReadByte();
            _CardType = (CardTypes)BR.ReadByte();
            _CardLocation = (CardLocations)BR.ReadByte();
            _RedrawCards = BR.ReadBoolean();

            if (_CardDestroyType == CardDestroyTypes.Random && _CardDestroyType == CardDestroyTypes.Specific)
            {
                _NumberOfCards = BR.ReadByte();
            }
            if (_CardDestroyType == CardDestroyTypes.Random || _CardDestroyType == CardDestroyTypes.All)
            {
                _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
                _MagicCost = BR.ReadInt32();
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_CardDestroyType);
            BW.Write((byte)_CardType);
            BW.Write((byte)_CardLocation);
            BW.Write(_RedrawCards);

            if (_CardDestroyType == CardDestroyTypes.Random && _CardDestroyType == CardDestroyTypes.Specific)
            {
                BW.Write(_NumberOfCards);
            }
            if (_CardDestroyType == CardDestroyTypes.Random || _CardDestroyType == CardDestroyTypes.All)
            {
                BW.Write((byte)_LogicOperator);
                BW.Write(_MagicCost);
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

            List<Card> ListCard = new List<Card>();

            for (int I = RealTarget.Owner.ListCardInHand.Count - 1; I >= 0; --I)
            {
                if (_MagicCost > 0 && !Operators.CompareValue(LogicOperator, RealTarget.Owner.ListCardInHand[I].MagicCost, _MagicCost))
                {
                    continue;
                }

                switch (_CardType)
                {
                    case CardTypes.All:
                        break;

                    case CardTypes.Item:
                        if (RealTarget.Owner.ListCardInHand[I].CardType != ItemCard.ItemCardType)
                        {
                            continue;
                        }
                        break;

                    case CardTypes.Armor:
                        if (RealTarget.Owner.ListCardInHand[I].CardType == ItemCard.ItemCardType)
                        {
                            ItemCard ItemCard = (ItemCard)RealTarget.Owner.ListCardInHand[I];
                            if (ItemCard.ItemType != ItemCard.ItemTypes.Armor)
                            {
                                continue;
                            }
                        }
                        break;

                    case CardTypes.Scroll:
                        if (RealTarget.Owner.ListCardInHand[I].CardType == ItemCard.ItemCardType)
                        {
                            ItemCard ItemCard = (ItemCard)RealTarget.Owner.ListCardInHand[I];
                            if (ItemCard.ItemType != ItemCard.ItemTypes.Scrolls)
                            {
                                continue;
                            }
                        }
                        break;

                    case CardTypes.Weapon:
                        if (RealTarget.Owner.ListCardInHand[I].CardType == ItemCard.ItemCardType)
                        {
                            ItemCard ItemCard = (ItemCard)RealTarget.Owner.ListCardInHand[I];
                            if (ItemCard.ItemType != ItemCard.ItemTypes.Weapon)
                            {
                                continue;
                            }
                        }
                        break;

                    case CardTypes.Tool:
                        if (RealTarget.Owner.ListCardInHand[I].CardType == ItemCard.ItemCardType)
                        {
                            ItemCard ItemCard = (ItemCard)RealTarget.Owner.ListCardInHand[I];
                            if (ItemCard.ItemType != ItemCard.ItemTypes.Tools)
                            {
                                continue;
                            }
                        }
                        break;

                    case CardTypes.Spell:
                        if (RealTarget.Owner.ListCardInHand[I].CardType != SpellCard.SpellCardType)
                        {
                            continue;
                        }
                        break;

                    case CardTypes.Creature:
                        if (RealTarget.Owner.ListCardInHand[I].CardType != CreatureCard.CreatureCardType)
                        {
                            continue;
                        }
                        break;
                }

                ListCard.Add(RealTarget.Owner.ListCardInHand[I]);
            }

            if (_CardDestroyType == CardDestroyTypes.All)
            {
                for (int I = ListCard.Count - 1; I >= 0; --I)
                {
                    RealTarget.Owner.ListCardInHand.Remove(ListCard[I]);
                }
                return "Destroy All Cards";
            }
            else if (_CardDestroyType == CardDestroyTypes.Random)
            {
                for (int C = 0; C < _NumberOfCards && RealTarget.Owner.ListCardInHand.Count >= 0; ++C)
                {
                    int RandomIndex = RandomHelper.Next(ListCard.Count);
                    RealTarget.Owner.ListCardInHand.Remove(ListCard[RandomIndex]);
                    ListCard.Remove(ListCard[RandomIndex]);
                }
            }
            else if (_CardDestroyType == CardDestroyTypes.Specific)
            {//User chooses a card from target Player's hand and destroys it.
            }
            else if (_CardDestroyType == CardDestroyTypes.DuplicateGlobal)
            {//If any card exists more than once across all Players' hands, said copies of that card are destroyed.
                Dictionary<string, int> DicCardCount = new Dictionary<string, int>();
                foreach (Player ActivePlayer in Params.Map.ListPlayer)
                {
                    if (ActivePlayer.OnlinePlayerType == OnlinePlayerBase.PlayerTypeNA)
                    {
                        continue;
                    }

                    foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
                    {
                        if (!DicCardCount.ContainsKey(ActiveCard.Path))
                        {
                            DicCardCount.Add(ActiveCard.Path, 0);
                        }
                        DicCardCount[ActiveCard.Path] = DicCardCount[ActiveCard.Path] + 1;
                    }
                }

                foreach (Player ActivePlayer in Params.Map.ListPlayer)
                {
                    if (ActivePlayer.OnlinePlayerType == OnlinePlayerBase.PlayerTypeNA)
                    {
                        continue;
                    }

                    for (int C = ActivePlayer.ListCardInHand.Count - 1; C >= 0; C--)
                    {
                        if (DicCardCount[ActivePlayer.ListCardInHand[C].Path] > 1)
                        {
                            ActivePlayer.ListCardInHand.RemoveAt(C);
                        }
                    }
                }
            }

            return "Destroy " + _NumberOfCards + " Cards";
        }

        protected override BaseEffect DoCopy()
        {
            DestroyCardsEffect NewEffect = new DestroyCardsEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._CardDestroyType = _CardDestroyType;
            NewEffect._CardType = _CardType;
            NewEffect._CardLocation = _CardLocation;
            NewEffect._RedrawCards = _RedrawCards;
            NewEffect._NumberOfCards = _NumberOfCards;
            NewEffect._LogicOperator = _LogicOperator;
            NewEffect._MagicCost = _MagicCost;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DestroyCardsEffect NewEffect = (DestroyCardsEffect)Copy;

            _Target = NewEffect._Target;
            _CardDestroyType = NewEffect._CardDestroyType;
            _CardType = NewEffect._CardType;
            _CardLocation = NewEffect._CardLocation;
            _RedrawCards = NewEffect._RedrawCards;
            _NumberOfCards = NewEffect._NumberOfCards;
            _LogicOperator = NewEffect._LogicOperator;
            _MagicCost = NewEffect._MagicCost;
        }

        #region Properties

        [CategoryAttribute("Default"),
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

        [CategoryAttribute("Default"),
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

        [CategoryAttribute("Default"),
        DescriptionAttribute("Where the cards to destroy are."),
        DefaultValueAttribute(0)]
        public CardLocations CardLocation
        {
            get
            {
                return _CardLocation;
            }
            set
            {
                _CardLocation = value;
            }
        }

        [CategoryAttribute("Default"),
        DescriptionAttribute("Replace destroyed cards."),
        DefaultValueAttribute(0)]
        public bool RedrawCards
        {
            get
            {
                return _RedrawCards;
            }
            set
            {
                _RedrawCards = value;
            }
        }

        [CategoryAttribute("Default"),
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

        [CategoryAttribute("Destruction Limit"),
        DescriptionAttribute("Limit of the magical cost of the card."),
        DefaultValueAttribute(0)]
        public int MagicCost
        {
            get
            {
                return _MagicCost;
            }
            set
            {
                _MagicCost = value;
            }
        }

        [CategoryAttribute("Destruction Limit"),
        DescriptionAttribute("How to interpret the magic cost."),
        DefaultValueAttribute("")]
        public Operators.LogicOperators LogicOperator
        {
            get
            {
                return _LogicOperator;
            }
            set
            {
                _LogicOperator = value;
            }
        }

        #endregion
    }
}
