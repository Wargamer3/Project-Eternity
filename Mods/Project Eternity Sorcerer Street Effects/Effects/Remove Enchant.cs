using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantRemoveEnchantEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Remove Enchant";

        public EnchantRemoveEnchantEffect()
            : base(Name, false)
        {
        }

        public EnchantRemoveEnchantEffect(SorcererStreetBattleParams Params)
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
            //Play Activation Animation

            //Enchant Player
            Params.GlobalPlayerContext.ActivePlayer.Enchant = null;
            return "Remove Enchant";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantRemoveEnchantEffect NewEffect = new EnchantRemoveEnchantEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
