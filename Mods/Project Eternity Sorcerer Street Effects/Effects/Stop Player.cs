using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class StopPlayerEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Stop Player";

        public StopPlayerEffect()
            : base(Name, false)
        {
        }

        public StopPlayerEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.StopPlayer();
            return "Scroll";
        }

        protected override BaseEffect DoCopy()
        {
            StopPlayerEffect NewEffect = new StopPlayerEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
