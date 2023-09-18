using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Disables Move and Exchange commands.
    public sealed class GlobalAbilityDisableMoveCommandEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Disable Move Command";
        public GlobalAbilityDisableMoveCommandEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableMoveCommandEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityDisableMoveCommandEffect NewEffect = new GlobalAbilityDisableMoveCommandEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
