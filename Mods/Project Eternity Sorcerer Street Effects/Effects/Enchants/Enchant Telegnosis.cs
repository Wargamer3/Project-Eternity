using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Adds Enlightenment to user until next round.
    public sealed class EnchantTelegnosisEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Telegnosis";

        public EnchantTelegnosisEffect()
            : base(Name, false)
        {
        }

        public EnchantTelegnosisEffect(SorcererStreetBattleParams Params)
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
            TerritoryCommandEffect NewTerritoryCommandEffect = new TerritoryCommandEffect();
            NewTerritoryCommandEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewTerritoryCommandEffect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetOnCreateRequirement(), NewTerritoryCommandEffect, IconHolder.Icons.sprPlayerTerritory);
            return "Telegnosis";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantTelegnosisEffect NewEffect = new EnchantTelegnosisEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
