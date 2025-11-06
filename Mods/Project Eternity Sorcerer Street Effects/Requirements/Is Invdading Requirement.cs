using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetIsInvadingRequirement : SorcererStreetRequirement
    {
        public SorcererStreetIsInvadingRequirement()
            : this(null)
        {
        }

        public SorcererStreetIsInvadingRequirement(SorcererStreetBattleParams Params)
            : base("Sorcerer Street Is Invading", Params)
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
            return Params.GlobalContext.SelfCreature.Creature != Params.GlobalContext.ActiveTerrain.DefendingCreature;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetIsInvadingRequirement NewRequirement = new SorcererStreetIsInvadingRequirement(Params);

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
