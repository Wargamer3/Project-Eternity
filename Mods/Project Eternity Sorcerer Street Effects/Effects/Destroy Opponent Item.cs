using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyOpponentItemEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Destroy Opponent Item";
        public enum CardDestroyTypes { Weapon, Scroll, Tool, Armor, Creature }

        private CardDestroyTypes _CardDestroyType;

        public DestroyOpponentItemEffect()
            : base(Name, false)
        {
        }

        public DestroyOpponentItemEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _CardDestroyType = (CardDestroyTypes)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_CardDestroyType);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (Params.GlobalContext.OpponentCreature.Creature.BattleAbilities.ItemProtection)
            {
                return null;
            }

            ItemCard OppnentCard = Params.GlobalContext.OpponentCreature.Item as ItemCard;

            if (OppnentCard != null)
            {
                if ((CardDestroyType == CardDestroyTypes.Armor && OppnentCard.ItemType == ItemCard.ItemTypes.Armor)
                    || (CardDestroyType == CardDestroyTypes.Scroll && OppnentCard.ItemType == ItemCard.ItemTypes.Scrolls)
                    || (CardDestroyType == CardDestroyTypes.Tool && OppnentCard.ItemType == ItemCard.ItemTypes.Tools)
                    || (CardDestroyType == CardDestroyTypes.Weapon && OppnentCard.ItemType == ItemCard.ItemTypes.Weapon))
                {
                    Params.GlobalContext.OpponentCreature.Item = null;
                }
            }
            else if (CardDestroyType == CardDestroyTypes.Creature && Params.GlobalContext.OpponentCreature.Item is CreatureCard)
            {
                Params.GlobalContext.OpponentCreature.Item = null;
            }

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            DestroyOpponentItemEffect NewEffect = new DestroyOpponentItemEffect(Params);

            NewEffect._CardDestroyType = _CardDestroyType;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            DestroyOpponentItemEffect CopyRequirement = (DestroyOpponentItemEffect)Copy;

            _CardDestroyType = CopyRequirement._CardDestroyType;
        }

        #region Properties

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

        #endregion
    }
}
