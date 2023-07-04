using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SleepEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Sleep";

        public SleepEffect()
            : base(Name, false)
        {
        }

        public SleepEffect(SorcererStreetBattleParams Params)
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
            SleepEffect NewEffect = new SleepEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
