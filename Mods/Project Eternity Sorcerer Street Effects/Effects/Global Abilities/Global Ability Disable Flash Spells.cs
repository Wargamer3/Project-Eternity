using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//All Players cannot be targeted by Flash spells or Flash territory abilities (except those that dispel Enchantment effects).
    public sealed class GlobalAbilityDisableFlashSpellsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Disable Flash Spells";

        public GlobalAbilityDisableFlashSpellsEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableFlashSpellsEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityDisableFlashSpellsEffect NewEffect = new GlobalAbilityDisableFlashSpellsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
