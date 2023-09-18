using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ItemCreatureEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Item Creature";

        public ItemCreatureEffect()
            : base(Name, false)
        {
        }

        public ItemCreatureEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Abilities.ItemCreature = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ItemCreatureEffect NewEffect = new ItemCreatureEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
