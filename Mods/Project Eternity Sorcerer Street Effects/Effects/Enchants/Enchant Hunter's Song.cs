using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: Target Player gains 200G magic for every creature they destroy in battle for 4 rounds.
    public sealed class EnchantHuntersSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Hunter's Song";

        public EnchantHuntersSongEffect()
            : base(Name, false)
        {
        }

        public EnchantHuntersSongEffect(SorcererStreetBattleParams Params)
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
            GainGoldEffect NewGainGoldEffect = new GainGoldEffect(Params);
            NewGainGoldEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewGainGoldEffect.Lifetime[0].LifetimeTypeValue = 4;
            NewGainGoldEffect.Target = GainGoldEffect.Targets.Self;
            NewGainGoldEffect.Value = "200";
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewGainGoldEffect, IconHolder.Icons.sprPlayerFairyLight);
            return "Hunter's Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHuntersSongEffect NewEffect = new EnchantHuntersSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
