using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DestroyOpponentItemEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Opponent Item";
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
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            DestroyOpponentItemEffect NewEffect = new DestroyOpponentItemEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
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
    }
}
