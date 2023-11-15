using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//All Players draw two cards when drawing instead of one.
    public sealed class GlobalAbilityDraw2CardsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Draw 2 Cards";

        public GlobalAbilityDraw2CardsEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDraw2CardsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
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
            GlobalAbilityDraw2CardsEffect NewEffect = new GlobalAbilityDraw2CardsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
