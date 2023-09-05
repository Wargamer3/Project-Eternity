using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ScrollAttackEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Scroll Attack";

        private byte _ST;

        public ScrollAttackEffect()
            : base(Name, false)
        {
        }

        public ScrollAttackEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _ST = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_ST);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ScrollAttack = true;

            return "Scroll";
        }

        protected override BaseEffect DoCopy()
        {
            ScrollAttackEffect NewEffect = new ScrollAttackEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public byte ST
        {
            get
            {
                return _ST;
            }
            set
            {
                _ST = value;
            }
        }
    }
}
