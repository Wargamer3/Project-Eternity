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

        public SorcererStreetIsInvadingRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Is Invading", GlobalContext)
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
            return GlobalContext.SelfCreature == GlobalContext.Invader;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetIsInvadingRequirement NewRequirement = new SorcererStreetIsInvadingRequirement(GlobalContext);

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
