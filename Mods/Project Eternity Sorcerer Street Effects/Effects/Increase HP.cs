using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class IncreaseHPEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Increase HP";

        public IncreaseHPEffect()
            : base(Name, false)
        {
        }

        public IncreaseHPEffect(SorcererStreetBattleParams Params)
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
            Params.IncreaseSelfHP(30);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            IncreaseHPEffect NewEffect = new IncreaseHPEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
