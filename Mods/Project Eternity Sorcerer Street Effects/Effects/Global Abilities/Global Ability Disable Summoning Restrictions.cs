using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Requirements for summoning creatures other than magic cost are ignored.
    public sealed class GlobalAbilityDisableSummoningRestrictionsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Disable Summoning Restrictions";

        public GlobalAbilityDisableSummoningRestrictionsEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableSummoningRestrictionsEffect(SorcererStreetBattleParams Params)
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
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            GlobalAbilityDisableSummoningRestrictionsEffect NewEffect = new GlobalAbilityDisableSummoningRestrictionsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
