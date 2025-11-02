using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetIsDefendingRequirement : SorcererStreetRequirement
    {
        public SorcererStreetIsDefendingRequirement()
            : this(null)
        {
        }

        public SorcererStreetIsDefendingRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Is Defending", GlobalContext)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        public override bool CanActivatePassive()
        {
            return GlobalContext.SelfCreature == GlobalContext.OpponentCreature;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetIsDefendingRequirement NewRequirement = new SorcererStreetIsDefendingRequirement(GlobalContext);

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
