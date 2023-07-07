using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class AttackLastEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Attack Last";

        public AttackLastEffect()
            : base(Name, false)
        {
        }

        public AttackLastEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.AttackLast = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            AttackLastEffect NewEffect = new AttackLastEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
