using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Adds Enlightenment to user until next round.
    public sealed class EnchantTelegnosisEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Telegnosis";

        public EnchantTelegnosisEffect()
            : base(Name, false)
        {
        }

        public EnchantTelegnosisEffect(SorcererStreetBattleParams Params)
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
            return "Telegnosis";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantTelegnosisEffect NewEffect = new EnchantTelegnosisEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
