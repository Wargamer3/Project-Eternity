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

        public SorcererStreetIsDefendingRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Is Defending", Params)
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
            return Params.GlobalContext.SelfCreature == Params.GlobalContext.OpponentCreature;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetIsDefendingRequirement NewRequirement = new SorcererStreetIsDefendingRequirement(Params);

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
