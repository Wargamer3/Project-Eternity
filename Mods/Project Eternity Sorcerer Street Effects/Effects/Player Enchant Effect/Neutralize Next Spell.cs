using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Neutralizes the next spell or territory ability that targets target Player (except those that dispel Enchantments).
    public sealed class NeutralizeNextSpellEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Neutralize Next Spell";

        public NeutralizeNextSpellEffect()
            : base(Name, false)
        {
        }

        public NeutralizeNextSpellEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).NeutralizeNextSpell = true;
            return "Neutralize Next Spell";
        }

        protected override BaseEffect DoCopy()
        {
            NeutralizeNextSpellEffect NewEffect = new NeutralizeNextSpellEffect(Params);


            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
