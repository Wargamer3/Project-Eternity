using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //Allow all territory command after taking a land (either vacant or after a battle)
    public sealed class ImmediateEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Immediate";

        public ImmediateEffect()
            : base(Name, false)
        {
        }

        public ImmediateEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).Immediate = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ImmediateEffect NewEffect = new ImmediateEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
