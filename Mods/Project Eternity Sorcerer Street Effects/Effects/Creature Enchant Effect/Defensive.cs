using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DefensiveEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Defensive";

        public DefensiveEffect()
            : base(Name, false)
        {
        }

        public DefensiveEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).IsDefensive = true;
            return "Defensive";
        }

        protected override BaseEffect DoCopy()
        {
            DefensiveEffect NewEffect = new DefensiveEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
