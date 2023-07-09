using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class PlayBattleAnimationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Play Battle Animation";

        private int _Damage;

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
            _Damage = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_Damage);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.GlobalContext.Defender.DamageReceived = Damage;
            Params.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleAttackAnimationPhase(Params.Map, Params.GlobalContext.Defender, "", false));

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

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int Damage
        {
            get
            {
                return _Damage;
            }
            set
            {
                _Damage = value;
            }
        }
    }
}
