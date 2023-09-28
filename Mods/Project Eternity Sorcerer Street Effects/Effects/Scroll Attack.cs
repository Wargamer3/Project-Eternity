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
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ScrollValue = _ST;
            if (_ST > 0)
            {
                return "Scroll Attack ST=" + _ST;
            }

            return "Scroll";
        }

        protected override BaseEffect DoCopy()
        {
            ScrollAttackEffect NewEffect = new ScrollAttackEffect(Params);

            NewEffect._ST = _ST;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ScrollAttackEffect NewEffect = (ScrollAttackEffect)Copy;

            _ST = NewEffect._ST;
        }

        #region Properties

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

        #endregion
    }
}
