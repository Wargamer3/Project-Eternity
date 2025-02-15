using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature can travel to any vacant land within the area when moving.
    public sealed class EnchantSpiritWalkEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Spirit Walk";

        public EnchantSpiritWalkEffect()
            : base(Name, false)
        {
        }

        public EnchantSpiritWalkEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetEnchantPhaseRequirement(), new FreeTravelEffect(), IconHolder.Icons.sprCreatureSpiritWalk);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSpiritWalkEffect NewEffect = new EnchantSpiritWalkEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
