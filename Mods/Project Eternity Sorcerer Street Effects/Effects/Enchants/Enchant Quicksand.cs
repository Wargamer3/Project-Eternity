using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Forces any Player that steps onto target territory to stop. Effect ends when triggered or after 2 rounds.
    public sealed class EnchantQuicksandEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Quicksand";

        public EnchantQuicksandEffect()
            : base(Name, false)
        {
        }

        public EnchantQuicksandEffect(SorcererStreetBattleParams Params)
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
            ForceStopEffect NewForceStopEffect = new ForceStopEffect(Params);
            NewForceStopEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewForceStopEffect.Lifetime[0].LifetimeTypeValue = 3;
            NewForceStopEffect.Lifetime.Add(new BaseEffectLifetime(SkillEffect.LifetimeTypeTurns, 2));

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetOnCreateRequirement(), NewForceStopEffect, IconHolder.Icons.sprCreatureQuicksand);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantQuicksandEffect NewEffect = new EnchantQuicksandEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
