using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class PlayBattleAnimationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Play Battle Animation";

        public PlayBattleAnimationEffect()
            : base(Name, false)
        {
        }

        public PlayBattleAnimationEffect(SorcererStreetBattleParams Params)
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
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Params.Map, "", false));

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            PlayBattleAnimationEffect NewEffect = new PlayBattleAnimationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
