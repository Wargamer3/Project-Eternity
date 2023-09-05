using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core;

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
            SorcererStreetBattleContext.BattleCreatureInfo RealTarget = Params.GlobalContext.SelfCreature;
            if (_Target == Targets.Opponent)
            {
                RealTarget = Params.GlobalContext.OpponentCreature;
            }

            if (RealTarget.Owner.ListCardInDeck.Count == 0)//Check empty deck for battle tester
            {
                while (RealTarget.Owner.ListCardInHand.Count < _NumberOfCards)
                {
                    Params.GlobalContext.Defender.Owner.ListCardInHand.Add(new CreatureCard("Dummy"));
                }
            }
            else
            {
                while (RealTarget.Owner.ListCardInHand.Count < _NumberOfCards)
                {
                    if (RealTarget.Owner.ListRemainingCardInDeck.Count == 0)
                    {
                        Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelRefillDeckPhase(Params.Map, Params.Map.ListPlayer.IndexOf(RealTarget.Owner)));
                    }

                    int RandomCardIndex = RandomHelper.Next(RealTarget.Owner.ListRemainingCardInDeck.Count);

                    Card DrawnCard = RealTarget.Owner.ListRemainingCardInDeck[RandomCardIndex];

                    RealTarget.Owner.ListCardInHand.Add(DrawnCard);

                    RealTarget.Owner.ListRemainingCardInDeck.RemoveAt(RandomCardIndex);
                }
            }

            return "Draw Cards";
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
