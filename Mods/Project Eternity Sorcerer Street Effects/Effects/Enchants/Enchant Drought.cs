using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Level of target Player's lands cannot be changed for 3 rounds.
    public sealed class EnchantDroughtEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Drought";

        public EnchantDroughtEffect()
            : base(Name, false)
        {
        }

        public EnchantDroughtEffect(SorcererStreetBattleParams Params)
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
            LandLevelLockEffect NewLandLevelLock = new LandLevelLockEffect(Params);
            NewLandLevelLock.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewLandLevelLock.Lifetime[0].LifetimeTypeValue = 3;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewLandLevelLock, IconHolder.Icons.sprCreatureDrought);
            return "Drought";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantDroughtEffect NewEffect = new EnchantDroughtEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
