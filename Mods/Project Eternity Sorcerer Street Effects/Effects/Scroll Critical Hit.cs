using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ScrollCriticalHitEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Scroll Critical Hit";

        public ScrollCriticalHitEffect()
            : base(Name, false)
        {
        }

        public ScrollCriticalHitEffect(SorcererStreetBattleParams Params)
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
            return "Scroll";
        }

        protected override BaseEffect DoCopy()
        {
            ScrollCriticalHitEffect NewEffect = new ScrollCriticalHitEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
