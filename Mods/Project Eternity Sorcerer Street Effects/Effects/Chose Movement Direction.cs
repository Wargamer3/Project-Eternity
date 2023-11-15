using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player can choose movement direction next turn.
    public sealed class ChoseMovementDirectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Chose Movement Direction";

        public ChoseMovementDirectionEffect()
            : base(Name, false)
        {
        }

        public ChoseMovementDirectionEffect(SorcererStreetBattleParams Params)
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
            ChoseMovementDirectionEffect NewEffect = new ChoseMovementDirectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
