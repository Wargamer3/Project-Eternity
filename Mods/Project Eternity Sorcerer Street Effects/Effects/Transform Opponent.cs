using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TransformOpponentEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Transform Opponent";

        private string _CreatureName;

        public TransformOpponentEffect()
            : base(Name, false)
        {
            _CreatureName = string.Empty;
        }

        public TransformOpponentEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _CreatureName = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _CreatureName = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_CreatureName);
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
            TransformOpponentEffect NewEffect = new TransformOpponentEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
