using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class RegenerateEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Regenerate";

        public RegenerateEffect()
            : base(Name, false)
        {
        }

        public RegenerateEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.Regenerate = true;
            return "Regenerate";
        }

        protected override BaseEffect DoCopy()
        {
            RegenerateEffect NewEffect = new RegenerateEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
