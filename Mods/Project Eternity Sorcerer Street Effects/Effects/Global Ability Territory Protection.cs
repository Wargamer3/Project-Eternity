using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //All territories are prevented from undergoing land transformation via spells or territory abilities and are immune to effects that lower land levels.
    public sealed class GlobalAbilityTerritoryProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Territory Protection";
        public GlobalAbilityTerritoryProtectionEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityTerritoryProtectionEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityTerritoryProtectionEffect NewEffect = new GlobalAbilityTerritoryProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
