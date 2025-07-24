using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Prevents changing of levels in any territories in the target area for 2 rounds.
    public sealed class EnchantDesertStormEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Desert Storm";

        public EnchantDesertStormEffect()
            : base(Name, false)
        {
        }

        public EnchantDesertStormEffect(SorcererStreetBattleParams Params)
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
            NewLandLevelLock.Lifetime[0].LifetimeTypeValue = 2;
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewLandLevelLock, IconHolder.Icons.sprCreatureDrought);

            return "Desert Storm";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantDesertStormEffect NewEffect = new EnchantDesertStormEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
