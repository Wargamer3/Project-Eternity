using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//User gains 50% of tolls collected by other Players until user reaches the castle.
    public sealed class EnchantDreamTerrainEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Dream Terrain";

        public EnchantDreamTerrainEffect()
            : base(Name, false)
        {
        }

        public EnchantDreamTerrainEffect(SorcererStreetBattleParams Params)
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
            TollGainShareEffect NewStealEffect = new TollGainShareEffect(Params);
            NewStealEffect.SignOperator = Core.Operators.SignOperators.Equal;
            NewStealEffect.Value = "50";
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewStealEffect, IconHolder.Icons.sprPlayerMovement);
            return "Dream Terrain";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantDreamTerrainEffect NewEffect = new EnchantDreamTerrainEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
