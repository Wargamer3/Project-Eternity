using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class AttackFirstEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Attack First";

        public AttackFirstEffect()
            : base(Name, false)
        {
        }

        public AttackFirstEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).AttackFirst = true;
            return "Attack First";
        }

        protected override BaseEffect DoCopy()
        {
            AttackFirstEffect NewEffect = new AttackFirstEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
