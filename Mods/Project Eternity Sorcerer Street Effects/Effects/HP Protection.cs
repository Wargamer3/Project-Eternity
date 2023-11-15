using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class HPProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street HP Protection";

        public HPProtectionEffect()
            : base(Name, false)
        {
        }

        public HPProtectionEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).HPProtection = true;
            return "HP & MHP cannot be altered by spells or territory abilities.";
        }

        protected override BaseEffect DoCopy()
        {
            HPProtectionEffect NewEffect = new HPProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
