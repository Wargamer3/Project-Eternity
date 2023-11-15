using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class RecycleEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Recycle";

        public RecycleEffect()
            : base(Name, false)
        {
        }

        public RecycleEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).Recycle = true;
            return "Recycle";
        }

        protected override BaseEffect DoCopy()
        {
            RecycleEffect NewEffect = new RecycleEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
