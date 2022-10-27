using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetCreatureSummonRequirement : SorcererStreetRequirement
    {
        public SorcererStreetCreatureSummonRequirement()
            : this(null)
        {
        }

        public SorcererStreetCreatureSummonRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Creature Summon", GlobalContext)
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
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetCreatureSummonRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
