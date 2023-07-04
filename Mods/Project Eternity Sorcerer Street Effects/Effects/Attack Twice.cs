using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class AttackTwiceEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Attack Twice";

        public AttackTwiceEffect()
            : base(Name, false)
        {
        }

        public AttackTwiceEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.BonusAbilities.AttackTwice = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            AttackTwiceEffect NewEffect = new AttackTwiceEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
