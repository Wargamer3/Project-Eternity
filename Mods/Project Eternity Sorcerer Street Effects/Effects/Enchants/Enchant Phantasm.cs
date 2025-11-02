using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature's HP and MHP cannot be altered by spells or territory abilities.
    public sealed class EnchantPhantasmEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Phantasm";

        public EnchantPhantasmEffect()
            : base(Name, false)
        {
        }

        public EnchantPhantasmEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreatePassiveEnchant(Name, new HPProtectionEffect(Params), IconHolder.Icons.sprCreaturePhantasm);
            return "Phantasm";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantPhantasmEffect NewEffect = new EnchantPhantasmEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
