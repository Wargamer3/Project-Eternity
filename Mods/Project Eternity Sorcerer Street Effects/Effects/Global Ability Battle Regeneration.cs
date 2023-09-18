using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GlobalAbilityBattleRegenerationEffect : SorcererStreetEffect
    {//In battle, all creatures gain Regenerates ability.
        public static string Name = "Sorcerer Street Global Ability Battle Regeneration";
        public GlobalAbilityBattleRegenerationEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityBattleRegenerationEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityBattleRegenerationEffect NewEffect = new GlobalAbilityBattleRegenerationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
