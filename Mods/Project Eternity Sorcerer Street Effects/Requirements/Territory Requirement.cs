using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //Used from placed on a free land
    public sealed class SorcererStreetTerriotryRequirement : SorcererStreetRequirement
    {
        public SorcererStreetTerriotryRequirement()
            : this(null)
        {
        }

        public SorcererStreetTerriotryRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Territory", GlobalContext)
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
            return new SorcererStreetTerriotryRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
