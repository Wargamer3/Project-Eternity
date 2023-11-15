using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//User gains (target creature's MHP x5G) magic if target creature is destroyed by a spell or territory ability
    public sealed class EnchantSoulHuntEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Soul Hunt";

        public EnchantSoulHuntEffect()
            : base(Name, false)
        {
        }

        public EnchantSoulHuntEffect(SorcererStreetBattleParams Params)
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
            return "Soul Hunt";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSoulHuntEffect NewEffect = new EnchantSoulHuntEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
