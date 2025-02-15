using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetOnLoadRequirement : SorcererStreetBattleRequirement
    {
        public SorcererStreetOnLoadRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnLoadRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street On Load", GlobalContext)
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
            return new SorcererStreetOnLoadRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
