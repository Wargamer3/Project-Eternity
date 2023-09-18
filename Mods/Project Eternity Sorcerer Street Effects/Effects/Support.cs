using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SupportEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Support";

        public SupportEffect()
            : base(Name, false)
        {
        }

        public SupportEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Abilities.SupportCreature = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            SupportEffect NewEffect = new SupportEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
