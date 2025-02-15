using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Adds Enlightenment to user for 2 rounds.
    public sealed class EnchantRevelationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Revelation";

        public EnchantRevelationEffect()
            : base(Name, false)
        {
        }

        public EnchantRevelationEffect(SorcererStreetBattleParams Params)
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
            TerritoryCommandEffect NewTerritoryCommandEffect = new TerritoryCommandEffect(Params);
            NewTerritoryCommandEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewTerritoryCommandEffect.Lifetime[0].LifetimeTypeValue = 2;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetOnCreateRequirement(), NewTerritoryCommandEffect, IconHolder.Icons.sprPlayerTerritory);
            return "Revelation";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantRevelationEffect NewEffect = new EnchantRevelationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
