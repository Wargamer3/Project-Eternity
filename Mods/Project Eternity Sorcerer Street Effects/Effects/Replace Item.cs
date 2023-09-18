using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ReplaceItemEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Replace Item";

        public ReplaceItemEffect()
            : base(Name, false)
        {
        }

        public ReplaceItemEffect(SorcererStreetBattleParams Params)
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
            if (Params.GlobalContext.SelfCreature.Item != null && Params.GlobalContext.OpponentCreature.Item != null)
            {
                Params.GlobalContext.OpponentCreature.Item = Params.GlobalContext.SelfCreature.Item;
                Params.GlobalContext.SelfCreature.Item = null;
            }

            return "Replace Item";
        }

        protected override BaseEffect DoCopy()
        {
            ReplaceItemEffect NewEffect = new ReplaceItemEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
